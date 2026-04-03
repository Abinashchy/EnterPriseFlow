import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { CurrentUser } from '../../core/models/user.model';
import { title } from 'node:process';
import { Users } from '../users/users';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard {
  currentUser: CurrentUser | null;

  constructor(private authService: AuthService) {
    this.currentUser = this.authService.getCurrentUser();
  }

  get quickStats() {
    return [
      {
        title: 'Security model',
        value: 'Jwt',
        description: 'Authenticated sessions are secured through bearer tokens.'
      },
      {
        title: 'Access control',
        value: this.currentUser?.role ?? 'N/A',
        description: 'Role-aware navigation and route protection are active.'
      },
      {
        title: 'Department scope',
        value: this.currentUser?.department ?? 'Unassigned',
        description: 'Backend RBAC and department filters define visible data.'
      }

    ];
  }
}
