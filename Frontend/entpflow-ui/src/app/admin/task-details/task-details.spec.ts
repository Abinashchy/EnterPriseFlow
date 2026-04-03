import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideRouter, Router } from '@angular/router';
import { PLATFORM_ID } from '@angular/core';
import { vi } from 'vitest';
import { TaskDetails } from './task-details';

describe('TaskDetails', () => {
  let component: TaskDetails;
  let fixture: ComponentFixture<TaskDetails>;
  let httpMock: HttpTestingController;
  let router: Router;

  const mockTask = {
    id: 1, title: 'Test Task', description: 'Desc', status: 'Created',
    priority: 'High', projectId: 1, projectName: 'Proj',
    assignedTo: 2, assignedToName: 'User', createdBy: 1,
    createdByName: 'Admin', createdAt: '2026-01-01',
  };

  const mockComments = [
    { id: 1, taskId: 1, userId: 1, comment: 'First comment', createdAt: '2026-01-01T00:00:00', userName: 'Admin' },
    { id: 2, taskId: 1, userId: 2, comment: 'Second comment', createdAt: '2026-01-02T00:00:00', userName: 'User' },
  ];

  function setupWithTask(task: any = mockTask) {
    // Set history.state before creating component
    vi.spyOn(history, 'state', 'get').mockReturnValue({ task });
    localStorage.setItem('entpflow.user', JSON.stringify({ userId: 1, name: 'Admin', role: 'Admin' }));

    fixture = TestBed.createComponent(TaskDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [TaskDetails],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        { provide: PLATFORM_ID, useValue: 'browser' },
      ],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should create and load comments', () => {
    setupWithTask();
    const req = httpMock.expectOne('https://localhost:7033/api/TaskComments/1');
    req.flush(mockComments);

    expect(component).toBeTruthy();
    expect(component.task).toEqual(mockTask);
    expect(component.comments.length).toBe(2);
  });

  it('should redirect to /tasks if no task in state', () => {
    const spy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    vi.spyOn(history, 'state', 'get').mockReturnValue({});
    localStorage.setItem('entpflow.user', JSON.stringify({ userId: 1 }));

    fixture = TestBed.createComponent(TaskDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(spy).toHaveBeenCalledWith(['/tasks']);
  });

  it('should set currentUserId from auth service', () => {
    setupWithTask();
    httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);

    expect(component.currentUserId).toBe(1);
  });

  describe('addComment', () => {
    it('should POST new comment and reload', () => {
      setupWithTask();
      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);

      component.newComment = 'New comment';
      component.addComment();

      const postReq = httpMock.expectOne('https://localhost:7033/api/TaskComments');
      expect(postReq.request.method).toBe('POST');
      expect(postReq.request.body).toEqual({ taskId: 1, userId: 1, comment: 'New comment' });
      postReq.flush({});

      // Reload comments
      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush(mockComments);
      expect(component.newComment).toBe('');
    });

    it('should not POST if comment is empty', () => {
      setupWithTask();
      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);

      component.newComment = '   ';
      component.addComment();

      httpMock.expectNone('https://localhost:7033/api/TaskComments');
    });
  });

  describe('startEdit', () => {
    it('should set editing state', () => {
      setupWithTask();
      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush(mockComments);

      component.startEdit(mockComments[0] as any);
      expect(component.editingCommentId).toBe(1);
      expect(component.editingCommentText).toBe('First comment');
    });
  });

  describe('cancelEdit', () => {
    it('should clear editing state', () => {
      setupWithTask();
      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);

      component.editingCommentId = 1;
      component.editingCommentText = 'test';
      component.cancelEdit();

      expect(component.editingCommentId).toBeNull();
      expect(component.editingCommentText).toBe('');
    });
  });

  describe('saveEdit', () => {
    it('should PUT updated comment and reload', () => {
      setupWithTask();
      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);

      component.editingCommentText = 'Updated comment';
      component.saveEdit(5);

      const putReq = httpMock.expectOne('https://localhost:7033/api/TaskComments/5');
      expect(putReq.request.method).toBe('PUT');
      expect(putReq.request.body).toEqual({ comment: 'Updated comment', userId: 1 });
      putReq.flush({});

      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);
      expect(component.editingCommentId).toBeNull();
    });

    it('should not PUT if edit text is empty', () => {
      setupWithTask();
      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);

      component.editingCommentText = '  ';
      component.saveEdit(5);

      httpMock.expectNone('https://localhost:7033/api/TaskComments/5');
    });
  });

  describe('deleteComment', () => {
    it('should DELETE comment and reload', () => {
      setupWithTask();
      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);

      component.deleteComment(5);

      const delReq = httpMock.expectOne('https://localhost:7033/api/TaskComments/5?userId=1');
      expect(delReq.request.method).toBe('DELETE');
      delReq.flush({});

      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);
    });
  });

  describe('goBack', () => {
    it('should navigate to /tasks', () => {
      setupWithTask();
      httpMock.expectOne('https://localhost:7033/api/TaskComments/1').flush([]);

      const spy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
      component.goBack();
      expect(spy).toHaveBeenCalledWith(['/tasks']);
    });
  });
});
