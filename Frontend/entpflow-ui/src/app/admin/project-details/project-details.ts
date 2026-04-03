import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ProjectMembers } from '../../core/models/project.model';
import { GetTasks } from '../../core/models/Tasks.model';
import { ProjectService } from '../../core/services/project.service';
import { TasksService } from '../../core/services/tasks.service';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';
import { UserService } from '../../core/services/user.service';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-project-details',
  imports: [CommonModule, FormsModule],
  templateUrl: './project-details.html',
  styleUrl: './project-details.css',
})
export class ProjectDetails implements OnInit{
  isLoading = true;
  errorMessage = '';
  project: any = null;
  projectMembers: ProjectMembers[] = [];
  tasks: GetTasks[] = [];
  availableUsers: any[] = [];
  selectedUserId: number | null = null;
  showAddMember = false;

  constructor(
  private readonly memberService : ProjectService,
  private readonly taskService : TasksService,
  private readonly userService: UserService,
  private authService: AuthService,
  private router: Router,
  private cdr: ChangeDetectorRef
  )
  {}
  ngOnInit(): void {
    this.loadPageData();

  }

  toggleAddMember() {
  this.showAddMember = !this.showAddMember;
}

filterUsersByDepartment(users: any[], members: any[]) {
  if (!this.project?.departmentId) return [];
  const memberIds = members.map((m: any) => Number(m.userId));

  const filtered = users.filter((user: any) => {
    return (user.department) ==(this.project?.departmentName)
      && !memberIds.includes(Number(user.userId));
  });

  console.log('filtered users:', filtered);
  return filtered;
}

addMember() {
  if (!this.selectedUserId || !this.project?.id) return;

  const payload = {
    projectId: this.project.id,
    userId: this.selectedUserId
  };
  console.log('Add members called');


  this.memberService.addMembers(payload).subscribe({
    next: () => {
      this.selectedUserId = null;
      this.showAddMember = false;
      this.loadPageData();
      console.log('Add members called servce');
    },
    error: err => {
      console.error('Add member failed', err);
    }
  });
}

removeMember(userId: number) {
  console.log('remove user called');
  this.memberService.removeMember(this.project.id, userId).subscribe({
    next: () => {
      this.loadPageData();
      console.log('remove user executed');

    },
    error: err => {
      console.error('Remove failed', err);
    }
  });
}


  loadPageData(){
    this.project = history.state?.project || null;
    console.log(this.project);
    this.isLoading =true;
    this.errorMessage = '';

    forkJoin({
      users: this.userService.getUsers(),
      projectMembers: this.memberService.getMembers(this.project.id),
      tasks: this.taskService.getTasksByProjectId(this.project.id)
    }).subscribe({
      next: ({users, projectMembers, tasks}) => {
        console.log('members: ', projectMembers);
        console.log('tasks: ', tasks);
        console.log('all users: ', users);
        console.log('my project: ', this.project);
        console.log('project.departmentId:', this.project?.departmentId);
        this.availableUsers = this.filterUsersByDepartment(users, projectMembers);
        console.log('availableUsers: ', this.availableUsers);
        this.projectMembers = projectMembers;
        this.tasks = tasks;
        this.isLoading = false;
        this.cdr.detectChanges();
      },

      error: () => {
        this.errorMessage = 'Unable to load user data.';
        this.isLoading = false;
      }

    })
  }








}
