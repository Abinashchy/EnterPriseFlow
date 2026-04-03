import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { UserService } from '../../core/services/user.service';
import {
  CreateUserRequest,
  DepartmentOption,
  RoleOption
} from '../../core/models/user.model';

@Component({
  selector: 'app-create-users',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './create-users.html',
  styleUrl: './create-users.css'
})
export class CreateUsers implements OnInit {
  roles: RoleOption[] = [];
  departments: DepartmentOption[] = [];
  isSubmitting = false;
  errorMessage = '';

  newUser: CreateUserRequest = {
    employeeId: '',
    name: '',
    email: '',
    password: '',
    role: '',
    department: ''
  };

  constructor(
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    forkJoin({
      roles: this.userService.getRoles(),
      departments: this.userService.getDepartments()
    }).subscribe({
      next: ({ roles, departments }) => {
        this.roles = roles;
        this.departments = departments;

      },
      error: () => {
        this.errorMessage = 'Unable to load form data.';
      }
    });
  }

  createUser(): void {
    this.errorMessage = '';
    this.isSubmitting = true;


    this.userService.createUser(this.newUser).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/users']);
      },
      error: () => {
        this.isSubmitting = false;
        this.errorMessage = 'Unable to create user.';
      }
    });
  }
}
