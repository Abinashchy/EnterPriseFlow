import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { GetTasks } from '../models/Tasks.model';

@Injectable({
  providedIn: 'root',
})
export class TasksService {

  private readonly taskApi = '/api/taskItems';

  constructor (private http: HttpClient) {}

  getTasksByProjectId(projectId: number): Observable<GetTasks[]>{
    return this.http.get<GetTasks[]>(`${this.taskApi}/projects/${projectId}`).pipe(tap(res => console.log('GET /api/taskItems/byProject/', res)
    ));
  }

  getTasks(): Observable<GetTasks[]>{
    return this.http.get<GetTasks[]>(this.taskApi).pipe(tap(res => console.log('GET /api/taskItems)', res)
    ));
  }
  createTask(taskData: any): Observable<any> {
    return this.http.post(this.taskApi, taskData).pipe(tap(res => console.log('POST /api/taskItems', res)
    ));
  }

  updateTask(taskId: number, taskData: any): Observable<any> {
    return this.http.put(`${this.taskApi}/${taskId}`, taskData).pipe(tap(res => console.log(`PUT /api/taskItems/${taskId}`, res)
    ));
  }
  deleteTask(taskId: number): Observable<any> {
    return this.http.delete(`${this.taskApi}/${taskId}`).pipe(tap(res => console.log(`DELETE /api/taskItems/${taskId}`, res)
    ));
  }

}
