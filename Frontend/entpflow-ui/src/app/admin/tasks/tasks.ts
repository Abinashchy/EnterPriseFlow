import { ChangeDetectorRef ,Component, OnInit } from '@angular/core';
import { TasksService } from '../../core/services/tasks.service';
import { GetTasks } from '../../core/models/Tasks.model';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../../core/services/project.service';
import { GetProjects, ProjectMembers } from '../../core/models/project.model';

@Component({
  selector: 'app-tasks',
  imports: [FormsModule],
  templateUrl: './tasks.html',
  styleUrl: './tasks.css',
})
export class Tasks implements OnInit{

  tasks: any[] = [];
  projects: GetProjects[] = [];
  // filteredProjects: any[] = [];
  projectMembers: ProjectMembers[] = [];
  isLoading = true;
  isTaskModalOpen = false;
  isEditMode = false;
  searchProjectName = '';
  showProjectDropdown = false;
  selectedProject: any = null;
  editingTaskId: number | null = null;


  taskForm = {
    title: '',
    description: '',
    status: 'created',
    priority: '',
    projectId: 0,
    assignedTo: 0,
  };


  constructor(private readonly taskService: TasksService, private router: Router, private cdRef: ChangeDetectorRef, private readonly projectService: ProjectService) {}

  ngOnInit(): void {
    this.loadTasks();
    this.loadProjects();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe((tasks: GetTasks[]) => {
      this.tasks = tasks;
      console.log('Tasks loaded:', this.tasks);
      this.isLoading = false;
      this.cdRef.detectChanges();
    });
  }

  goToTask(task: any): void {
    this.router.navigate(['/tasks', task.id], { state: { task } });
  }

    loadProjects(): void {
    this.projectService.getProjects().subscribe({
      next: (res: any[]) => {
        this.projects = res || [];
        // this.filteredProjects = [...this.projects];
        console.log('Projects loaded:', this.projects);
      },
      error: (err: any) => {
        console.error('Failed to load projects', err);
      }
    });
  }


  onProjectChange(projectId: number): void {
    this.taskForm.assignedTo = 0;
    this.projectMembers = [];
    if (projectId) {
      this.projectService.getMembers(projectId).subscribe({
        next: (members) => {
          this.projectMembers = members;
          this.cdRef.detectChanges();
        },
        error: (err) => console.error('Failed to load project members', err)
      });
    }
  }

  openCreateTaskModal(){
    this.isTaskModalOpen = true;
    this.isEditMode = false;
    this.editingTaskId = null;
    this.projectMembers = [];
    this.taskForm = {
      title: '',
      description: '',
      status: 'created',
      priority: '',
      projectId: 0,
      assignedTo: 0,
    };
  }

  openEditTaskModal(task: GetTasks){
    this.isTaskModalOpen = true;
    this.isEditMode = true;
    this.editingTaskId = task.id;
    this.taskForm = {
      title: task.title,
      description: task.description,
      status: task.status,
      priority: task.priority,
      projectId: task.projectId,
      assignedTo: task.assignedTo,
    };
    this.onProjectChange(task.projectId);
  }

  closeTaskModal(){
    this.isTaskModalOpen = false;
  }

  submitTaskForm(){
    const payload = {
      title: this.taskForm.title,
      description: this.taskForm.description,
      status: this.taskForm.status,
      priority: this.taskForm.priority,
      projectId: this.taskForm.projectId,
      assignedTo: this.taskForm.assignedTo,
    };

    console.log('Submitting task form with payload:', payload);

    if(this.isEditMode && this.editingTaskId !== null){
      // Update existing task
      console.log('Updating task with ID:', this.editingTaskId);
      this.taskService.updateTask(this.editingTaskId, payload).subscribe({
        next: (res) => {
          console.log('Task updated successfully:', res);
          this.closeTaskModal();
          this.loadTasks();
        },
        error: (err) => console.error('Failed to update task', err)
      });
    } else {
      // Create new task
      console.log('Creating new task');
      // Call create API here
      this.taskService.createTask(payload).subscribe({
        next: (res) => {
          console.log('Task created successfully:', res);
          this.closeTaskModal();
          this.loadTasks();
        },
        error: (err) => console.error('Failed to create task', err)
      });
    }
  }
  deleteTask(taskId: number): void {
    if (!confirm('Are you sure you want to delete this task?')) return;
    this.taskService.deleteTask(taskId).subscribe({
      next: () => this.loadTasks(),
      error: (err) => console.error('Failed to delete task', err)
    });
  }
}
