import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CurrentUser } from '../../core/models/user.model';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [ RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css'
})
export class AdminLayout {
  currentUser: CurrentUser | null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    this.currentUser = this.authService.getCurrentUser();
  }

  isAdmin(): boolean {
    return this.authService.hasRole('Admin');
  }

  isManager(): boolean {
    return this.authService.hasRole('Manager');
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
