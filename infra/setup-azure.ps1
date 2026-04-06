# ============================================================
# EntpFlow - Azure Infrastructure Setup
# Configures existing resources + creates frontend app.
# After this, CI/CD handles deployments automatically.
# ============================================================

param(
    [string]$ResourceGroup    = "entoflow-rg",
    [string]$Location         = "centralindia",
    [string]$AcrName          = "entpflowacr096",
    [string]$ContainerEnv     = "managedEnvironment-entoflowrg-b49b",
    [string]$SqlServerName    = "entpflow-sql-096",
    [string]$SqlDbName        = "entpflow-db096",
    [string]$SqlAdminUser     = "entpflowadmin"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " EntpFlow Azure Infrastructure Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# --- Prompt for secrets ---
$SqlPassword = Read-Host -Prompt "Enter SQL Server admin password"
$JwtKey = -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object { [char]$_ })
Write-Host "Generated JWT Key: $JwtKey" -ForegroundColor Gray

$ConnectionString = "Server=tcp:${SqlServerName}.database.windows.net,1433;Database=${SqlDbName};User Id=${SqlAdminUser};Password=${SqlPassword};Encrypt=True;TrustServerCertificate=False;"

# --- 1. Configure Backend Secrets & Env Vars ---
Write-Host "`n[1/5] Configuring Backend secrets..." -ForegroundColor Yellow
az containerapp secret set `
    --name entpflow-backend `
    --resource-group $ResourceGroup `
    --secrets "sql-conn=$ConnectionString" "jwt-key=$JwtKey" `
    --output none

Write-Host "[2/5] Configuring Backend environment variables..." -ForegroundColor Yellow
az containerapp update `
    --name entpflow-backend `
    --resource-group $ResourceGroup `
    --set-env-vars `
        "ASPNETCORE_ENVIRONMENT=Production" `
        "ConnectionStrings__DefaultConnection=secretref:sql-conn" `
        "Jwt__Key=secretref:jwt-key" `
        "Jwt__Issuer=EntpFlowApi" `
        "Jwt__Audience=EntpFlowClient" `
        "Jwt__ExpiryMinutes=60" `
    --output none

# --- 2. Create Frontend Container App ---
Write-Host "[3/5] Creating Frontend Container App..." -ForegroundColor Yellow
$BackendFqdn = az containerapp show `
    --name entpflow-backend `
    --resource-group $ResourceGroup `
    --query "properties.configuration.ingress.fqdn" -o tsv

az containerapp create `
    --name entpflow-frontend `
    --resource-group $ResourceGroup `
    --environment $ContainerEnv `
    --image "mcr.microsoft.com/k8se/quickstart:latest" `
    --target-port 80 `
    --ingress external `
    --min-replicas 1 `
    --max-replicas 3 `
    --cpu 0.5 `
    --memory 1.0Gi `
    --env-vars "BACKEND_URL=https://$BackendFqdn" `
    --output none

# --- 3. ACR Integration (Managed Identity) ---
Write-Host "[4/5] Configuring ACR access for Container Apps..." -ForegroundColor Yellow
$AcrId = az acr show --name $AcrName --query id -o tsv

# Backend - enable system identity and grant ACR pull
az containerapp identity assign --name entpflow-backend --resource-group $ResourceGroup --system-assigned --output none
$BackendPrincipal = az containerapp identity show --name entpflow-backend --resource-group $ResourceGroup --query principalId -o tsv
az role assignment create --assignee $BackendPrincipal --role AcrPull --scope $AcrId --output none
az containerapp registry set --name entpflow-backend --resource-group $ResourceGroup --server "$AcrName.azurecr.io" --identity system --output none

# Frontend - enable system identity and grant ACR pull
az containerapp identity assign --name entpflow-frontend --resource-group $ResourceGroup --system-assigned --output none
$FrontendPrincipal = az containerapp identity show --name entpflow-frontend --resource-group $ResourceGroup --query principalId -o tsv
az role assignment create --assignee $FrontendPrincipal --role AcrPull --scope $AcrId --output none
az containerapp registry set --name entpflow-frontend --resource-group $ResourceGroup --server "$AcrName.azurecr.io" --identity system --output none

# --- 4. Service Principal for GitHub Actions ---
Write-Host "[5/5] Creating Service Principal for GitHub Actions..." -ForegroundColor Yellow
$SubscriptionId = az account show --query id -o tsv
$SpCredentials = az ad sp create-for-rbac `
    --name "entpflow-github-actions" `
    --role Contributor `
    --scopes "/subscriptions/$SubscriptionId/resourceGroups/$ResourceGroup" `
    --sdk-auth

# Grant AcrPush on the container registry
$SpAppId = ($SpCredentials | ConvertFrom-Json).clientId
az role assignment create --assignee $SpAppId --role AcrPush --scope $AcrId --output none

# --- Output ---
$FrontendFqdn = az containerapp show `
    --name entpflow-frontend `
    --resource-group $ResourceGroup `
    --query "properties.configuration.ingress.fqdn" -o tsv

Write-Host "`n========================================" -ForegroundColor Green
Write-Host " Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Frontend URL:  https://$FrontendFqdn" -ForegroundColor White
Write-Host "Backend URL:   https://$BackendFqdn" -ForegroundColor White
Write-Host "SQL Server:    ${SqlServerName}.database.windows.net" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host " GitHub Repository Secrets to Configure" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "Go to: https://github.com/Abinashchy/EnterPriseFlow/settings/secrets/actions" -ForegroundColor Cyan
Write-Host ""
Write-Host "Add this secret:" -ForegroundColor White
Write-Host "  Name:  AZURE_CREDENTIALS" -ForegroundColor White
Write-Host "  Value: (paste the JSON below)" -ForegroundColor White
Write-Host ""
Write-Host $SpCredentials -ForegroundColor Gray
Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host " Next Steps" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "1. Copy the AZURE_CREDENTIALS JSON above" -ForegroundColor White
Write-Host "2. Add it as a GitHub secret (link above)" -ForegroundColor White
Write-Host "3. Push to 'main' branch to trigger deployment" -ForegroundColor White
Write-Host ""
