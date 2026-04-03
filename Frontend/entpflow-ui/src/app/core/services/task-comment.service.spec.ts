import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TaskCommentService } from './task-comment.service';

describe('TaskCommentService', () => {
  let service: TaskCommentService;
  let httpMock: HttpTestingController;
  const api = 'https://localhost:7033/api/TaskComments';

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(TaskCommentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getComments', () => {
    it('should GET comments by taskId', () => {
      const mockComments = [
        { id: 1, taskId: 5, userId: 1, comment: 'test', createdAt: '', userName: 'User1' },
      ];

      service.getComments(5).subscribe((comments) => {
        expect(comments.length).toBe(1);
        expect(comments[0].comment).toBe('test');
      });

      const req = httpMock.expectOne(`${api}/5`);
      expect(req.request.method).toBe('GET');
      req.flush(mockComments);
    });
  });

  describe('addComment', () => {
    it('should POST a new comment', () => {
      const payload = { taskId: 5, userId: 1, comment: 'new comment' };

      service.addComment(payload).subscribe();

      const req = httpMock.expectOne(api);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(payload);
      req.flush({});
    });
  });

  describe('updateComment', () => {
    it('should PUT updated comment', () => {
      const payload = { comment: 'updated', userId: 1 };

      service.updateComment(10, payload).subscribe();

      const req = httpMock.expectOne(`${api}/10`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(payload);
      req.flush({});
    });
  });

  describe('deleteComment', () => {
    it('should DELETE comment with userId param', () => {
      service.deleteComment(10, 1).subscribe();

      const req = httpMock.expectOne(`${api}/10?userId=1`);
      expect(req.request.method).toBe('DELETE');
      req.flush({});
    });
  });
});
