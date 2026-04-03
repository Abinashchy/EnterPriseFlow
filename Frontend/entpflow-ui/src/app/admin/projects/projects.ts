import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { UserService } from '../../core/services/user.service';
import { ProjectService } from '../../core/services/project.service';
import {
  DepartmentOption,
  RoleOption,
  UpdateUserRequest,
  UserRow
} from '../../core/models/user.model';
import { GetProjects } from '../../core/models/project.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './projects.html',
  styleUrl: './projects.css',
})

export class Projects implements OnInit {
  projects: GetProjects[] = [];
  users: UserRow[] = [];
  roles: RoleOption[] = [];
  departments: DepartmentOption[] = [];
  // editingUser: UpdateUserRequest & { userId: number } | null = null;
  isLoading = true;
  errorMessage = '';
  canCreateProjects = false;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private projectService: ProjectService,
    private router: Router,
    private cdr: ChangeDetectorRef

  ) {}

  ngOnInit(): void {
    this.canCreateProjects = this.authService.hasRole('Admin', 'Manager');
    console.log(this.canCreateProjects);
    this.loadPageData();
  }

  loadPageData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      projects: this.projectService.getProjects(),
      users: this.userService.getUsers(),
      roles: this.userService.getRoles(),
      departments: this.userService.getDepartments()
    }).subscribe({
      next: ({projects, users, roles, departments }) => {
        console.log('project response:', projects);
        console.log('users response:', users.length);
        console.log('roles response:', roles);
        console.log('departments response:', departments);

        this.projects = projects;
        this.users = users;
        this.roles = roles;
        this.departments = departments;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Unable to load user data.';
        this.isLoading = false;
      }
    });
  }

  // editUser(user: UserRow): void {
  //   this.editingUser = {
  //     userId: user.userId,
  //     employeeId: user.employeeId,
  //     name: user.name,
  //     email: user.email,
  //     role: user.role,
  //     department: user.department,
  //     isActive: user.isActive
  //   };
  // }

  // updateUser(): void {
  //   if (!this.editingUser) {
  //     return;
  //   }

  //   const { userId, ...payload } = this.editingUser;

  //   this.userService.updateUser(userId, payload).subscribe({
  //     next: () => {
  //       this.editingUser = null;
  //       this.loadPageData();
  //     },
  //     error: () => {
  //       this.errorMessage = 'Unable to update user.';
  //     }
  //   });
  // }

  // cancelEdit(): void {
  //   this.editingUser = null;
  // }

  // deleteUser(id: number): void {
  //   if (!confirm('Delete this user?')) {
  //     return;
  //   }

  //   this.userService.deleteUser(id).subscribe({
  //     next: () => this.loadPageData(),
  //     error: () => {
  //       this.errorMessage = 'Unable to delete user.';
  //     }
  //   });
  // }

  navigateToCreate(): void {
    this.router.navigate(['/create-projects']);
  }
}
