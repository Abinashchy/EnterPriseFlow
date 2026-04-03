import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TasksService } from '../../core/services/tasks.service';

interface BoardColumn {
  key: string;
  label: string;
  tasks: any[];
}

@Component({
  selector: 'app-task-board',
  imports: [],
  templateUrl: './task-board.html',
  styleUrl: './task-board.css',
})
export class TaskBoard implements OnInit {
  columns: BoardColumn[] = [
    { key: 'Created', label: 'Created', tasks: [] },
    { key: 'InProgress', label: 'Implementing', tasks: [] },
    { key: 'CodeReview', label: 'Code Review', tasks: [] },
    { key: 'ReadyForTesting', label: 'Ready for Testing', tasks: [] },
    { key: 'Done', label: 'Done', tasks: [] },
  ];

  isLoading = true;
  draggedTask: any = null;

  constructor(
    private taskService: TasksService,
    private router: Router,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe({
      next: (tasks) => {
        for (const col of this.columns) {
          if (col.key === 'Created') {
            col.tasks = tasks.filter(t => t.status === 'Created' || t.status === 'Assigned');
          } else {
            col.tasks = tasks.filter(t => t.status === col.key);
          }
        }
        this.isLoading = false;
        this.cdRef.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load tasks', err);
        this.isLoading = false;
      }
    });
  }

  onDragStart(event: DragEvent, task: any): void {
    this.draggedTask = task;
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = 'move';
    }
  }

  onDrop(event: DragEvent, column: BoardColumn): void {
    event.preventDefault();
    if (!this.draggedTask || this.draggedTask.status === column.key) {
      this.draggedTask = null;
      return;
    }

    const oldStatus = this.draggedTask.status;
    const oldCol = this.columns.find(c => c.key === oldStatus);
    if (oldCol) {
      oldCol.tasks = oldCol.tasks.filter(t => t.id !== this.draggedTask.id);
    }

    this.draggedTask.status = column.key;
    column.tasks.push(this.draggedTask);
    this.cdRef.detectChanges();

    this.taskService.updateTask(this.draggedTask.id, {
      title: this.draggedTask.title,
      description: this.draggedTask.description,
      status: column.key,
      priority: this.draggedTask.priority,
      projectId: this.draggedTask.projectId,
      assignedTo: this.draggedTask.assignedTo,
    }).subscribe({
      next: () => {},
      error: (err) => {
        console.error('Failed to update task status', err);
        // Revert on error
        column.tasks = column.tasks.filter(t => t.id !== this.draggedTask.id);
        this.draggedTask.status = oldStatus;
        if (oldCol) {
          oldCol.tasks.push(this.draggedTask);
        }
        this.cdRef.detectChanges();
      }
    });

    this.draggedTask = null;
  }

  onDragEnd(): void {
    this.draggedTask = null;
  }

  openTask(task: any): void {
    this.router.navigate(['/tasks', task.id], { state: { task } });
  }

  getPriorityClass(priority: string): string {
    return priority ? 'priority-' + priority.toLowerCase() : '';
  }

  getColumnCount(column: BoardColumn): number {
    return column.tasks.length;
  }
}
