FROM node:20-bullseye AS build
WORKDIR /app

RUN apt-get update && apt-get install -y ca-certificates curl && update-ca-certificates

COPY package*.json ./

RUN npm config set fund false \
 && npm config set audit false \
 && npm config set strict-ssl false \
 && npm install --no-audit --no-fund --prefer-offline

COPY . .
RUN npm run build

FROM nginx:alpine
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build /app/dist/entpflow-ui/browser/ /usr/share/nginx/html/
COPY nginx.conf.template /etc/nginx/nginx.conf.template
RUN if [ -f /usr/share/nginx/html/index.csr.html ]; then mv /usr/share/nginx/html/index.csr.html /usr/share/nginx/html/index.html; fi

EXPOSE 80
CMD ["/bin/sh", "-c", "envsubst '${BACKEND_URL}' < /etc/nginx/nginx.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
