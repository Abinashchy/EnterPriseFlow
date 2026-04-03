import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const roles = (route.data?.['roles'] as string[] | undefined) ?? [];

  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  if (roles.length === 0 || authService.hasRole(...roles)) {
    return true;
  }

  router.navigate(['/unauthorized']);
  return false;
};
