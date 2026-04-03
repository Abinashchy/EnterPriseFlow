import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  form = {
    email: '',
    password: ''
  };

  isSubmitting = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login(): void {
    this.errorMessage = '';
    this.isSubmitting = true;

    this.authService.login(this.form).subscribe({
      next: () => {
        const currentUser = this.authService.getCurrentUser();
        this.isSubmitting = false;

        // if (currentUser?.role === 'Admin' || currentUser?.role === 'Manager') {
          this.router.navigate(['/dashboard']);
        //   return;
        // }

        // this.router.navigate(['/unauthorized']);
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error?.status === 401
          ? 'Invalid email or password.'
          : 'Unable to sign in right now. Please try again.';
      }
    });
  }
}
