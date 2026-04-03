import { Component, OnInit } from '@angular/core';
import { CreateProjectRequest } from '../../core/models/project.model';
import { UserService } from '../../core/services/user.service';
import { Router, RouterModule } from '@angular/router';
import { ProjectService } from '../../core/services/project.service';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from "@angular/forms";
import { finalize } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-create-projects',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './create-projects.html',
  styleUrl: './create-projects.css',
})
export class CreateProjects implements OnInit{

  departments: any[] = [];
  issubmitting   = false;
  errorMessage   = '';
  successMessage = '';

  newProject: CreateProjectRequest = {
    name: '',
    description:'',
    departmentId: null

  }

  constructor(private userService: UserService, private projectService: ProjectService, private router: Router, public authService: AuthService) {}

  ngOnInit(): void {

    departments: this.userService.getDepartments().subscribe({
        next: (departments) => {
          this.departments = departments;
          const currUser = this.authService.getCurrentUser();

          if (this.authService.hasRole('Manager')) {
            const matchedDepartment = this.departments.find(
              d => d.name?.trim().toLowerCase() === currUser?.department?.trim().toLowerCase()
            );

            this.newProject.departmentId = matchedDepartment?.id ?? null;

            if (!this.newProject.departmentId) {
              this.errorMessage = 'Your department could not be mapped.';
            }
          }
        },
      error: (err) => {
        console.error('Error loading departments:', err);
        this.errorMessage = 'Unable to load departments.';
      }
      });
  }

  createProject(): void {
    this.errorMessage   = '';
    this.successMessage = '';

    if (!this.newProject.name?.trim()) {
      this.errorMessage = 'Project name is required.';
      return;
    }
    this.issubmitting = true;
    this.projectService.createProject(this.newProject)
    .pipe(finalize(() => {
        this.issubmitting = false;
      })
    )
    .subscribe({
      next: () => {
        if (this.authService.hasRole('Manager')) {
            this.successMessage = 'Project created successfully. You were added as a project member.';
        } else {
            this.successMessage = 'Project created successfully.';
          }

        this.router.navigate(['/projects']);
      },
      error: (err: HttpErrorResponse) => {
        console.error('Create project failed:', err);

        if (typeof err.error === 'string' && err.error.trim()) {
          this.errorMessage = err.error;
        } else if (err.error?.message) {
          this.errorMessage = err.error.message;
        } else if (err.error?.title) {
          this.errorMessage = err.error.title;
        } else if (err.status === 403) {
          this.errorMessage = 'You cannot create a project in this department.';
        } else if (err.status === 400) {
          this.errorMessage = 'Invalid project data. Please check the form.';
        } else {
          this.errorMessage = 'Unable to create project. Please try again.';
        }
      }
    });



  }
}
