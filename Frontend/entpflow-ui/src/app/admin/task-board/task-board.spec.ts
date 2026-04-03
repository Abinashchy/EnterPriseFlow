import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideRouter, Router } from '@angular/router';
import { vi } from 'vitest';
import { TaskBoard } from './task-board';

describe('TaskBoard', () => {
  let component: TaskBoard;
  let fixture: ComponentFixture<TaskBoard>;
  let httpMock: HttpTestingController;
  let router: Router;

  const mockTasks = [
    { id: 1, title: 'Task 1', status: 'Created', priority: 'High', projectId: 1, projectName: 'Proj', assignedTo: 1, assignedToName: 'User1' },
    { id: 2, title: 'Task 2', status: 'Assigned', priority: 'Low', projectId: 1, projectName: 'Proj', assignedTo: 2, assignedToName: 'User2' },
    { id: 3, title: 'Task 3', status: 'InProgress', priority: 'Medium', projectId: 1, projectName: 'Proj', assignedTo: 1, assignedToName: 'User1' },
    { id: 4, title: 'Task 4', status: 'CodeReview', priority: 'High', projectId: 1, projectName: 'Proj', assignedTo: 1, assignedToName: 'User1' },
    { id: 5, title: 'Task 5', status: 'Done', priority: 'Low', projectId: 1, projectName: 'Proj', assignedTo: 2, assignedToName: 'User2' },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskBoard],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
      ],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(TaskBoard);
    component = fixture.componentInstance;
  });

  afterEach(() => httpMock.verify());

  function flushTasks(tasks = mockTasks) {
    const req = httpMock.expectOne('https://localhost:7033/api/taskItems');
    req.flush(tasks);
  }

  it('should create', () => {
    flushTasks();
    expect(component).toBeTruthy();
  });

  it('should have 5 columns', () => {
    flushTasks();
    expect(component.columns.length).toBe(5);
    expect(component.columns.map(c => c.key)).toEqual(['Created', 'InProgress', 'CodeReview', 'ReadyForTesting', 'Done']);
  });

  it('should put Created and Assigned tasks in Created column', () => {
    flushTasks();
    const createdCol = component.columns.find(c => c.key === 'Created')!;
    expect(createdCol.tasks.length).toBe(2);
    expect(createdCol.tasks.map(t => t.id)).toContain(1);
    expect(createdCol.tasks.map(t => t.id)).toContain(2);
  });

  it('should put InProgress tasks in Implementing column', () => {
    flushTasks();
    const col = component.columns.find(c => c.key === 'InProgress')!;
    expect(col.tasks.length).toBe(1);
    expect(col.tasks[0].id).toBe(3);
  });

  it('should set isLoading to false after load', () => {
    expect(component.isLoading).toBe(true);
    flushTasks();
    expect(component.isLoading).toBe(false);
  });

  it('should handle load error gracefully', () => {
    const req = httpMock.expectOne('https://localhost:7033/api/taskItems');
    req.error(new ProgressEvent('Network error'));
    expect(component.isLoading).toBe(false);
  });

  describe('onDragStart', () => {
    it('should set draggedTask', () => {
      flushTasks();
      const task = mockTasks[0];
      const event = new DragEvent('dragstart', { dataTransfer: new DataTransfer() });
      component.onDragStart(event, task);
      expect(component.draggedTask).toBe(task);
    });
  });

  describe('onDragEnd', () => {
    it('should clear draggedTask', () => {
      flushTasks();
      component.draggedTask = mockTasks[0];
      component.onDragEnd();
      expect(component.draggedTask).toBeNull();
    });
  });

  describe('onDrop', () => {
    it('should move task to new column and call updateTask', () => {
      flushTasks();
      const task = { ...mockTasks[0], status: 'Created' };
      component.columns[0].tasks = [task];
      component.draggedTask = task;

      const event = new DragEvent('drop');
      const targetCol = component.columns.find(c => c.key === 'InProgress')!;
      component.onDrop(event, targetCol);

      expect(targetCol.tasks).toContain(task);
      expect(task.status).toBe('InProgress');

      const req = httpMock.expectOne(`https://localhost:7033/api/taskItems/${task.id}`);
      expect(req.request.method).toBe('PUT');
      req.flush({});
    });

    it('should not move task if dropped on same column', () => {
      flushTasks();
      const task = { ...mockTasks[0], status: 'Created' };
      component.draggedTask = task;

      const event = new DragEvent('drop');
      const sameCol = component.columns.find(c => c.key === 'Created')!;
      component.onDrop(event, sameCol);

      expect(component.draggedTask).toBeNull();
    });
  });

  describe('openTask', () => {
    it('should navigate to task detail', () => {
      flushTasks();
      const spy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
      component.openTask(mockTasks[0]);
      expect(spy).toHaveBeenCalledWith(['/tasks', 1], { state: { task: mockTasks[0] } });
    });
  });

  describe('getPriorityClass', () => {
    it('should return lowercase priority class', () => {
      flushTasks();
      expect(component.getPriorityClass('High')).toBe('priority-high');
      expect(component.getPriorityClass('Low')).toBe('priority-low');
    });

    it('should return empty string if no priority', () => {
      flushTasks();
      expect(component.getPriorityClass('')).toBe('');
    });
  });

  describe('getColumnCount', () => {
    it('should return number of tasks in column', () => {
      flushTasks();
      const col = component.columns.find(c => c.key === 'Created')!;
      expect(component.getColumnCount(col)).toBe(col.tasks.length);
    });
  });
});
