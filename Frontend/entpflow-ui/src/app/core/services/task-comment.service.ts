import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { TaskComment, CreateComment, UpdateComment } from '../models/task-comment.model';

@Injectable({
  providedIn: 'root',
})
export class TaskCommentService {

  private readonly api = 'https://localhost:7033/api/TaskComments';

  constructor(private http: HttpClient) {}

  getComments(taskId: number): Observable<TaskComment[]> {
    return this.http.get<TaskComment[]>(`${this.api}/${taskId}`);
  }

  addComment(payload: CreateComment): Observable<any> {
    return this.http.post(this.api, payload);
  }

  updateComment(id: number, payload: UpdateComment): Observable<any> {
    return this.http.put(`${this.api}/${id}`, payload);
  }

  deleteComment(id: number, userId: number): Observable<any> {
    return this.http.delete(`${this.api}/${id}`, { params: { userId: userId.toString() } });
  }
}
