import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { GetProjects, CreateProjectRequest, ProjectMembers, CreateMembers } from '../models/project.model';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {

  private readonly projectApi = 'api/Project';
  private readonly projectmembersApi = 'https://localhost:7033/api/projectmembers/project';

  constructor(private http: HttpClient){}

  getProjects(): Observable<GetProjects[]> {
    return this.http.get<GetProjects[]>(this.projectApi).pipe(tap(res => console.log('GET /api/Project)', res)
  ));

  }
  createProject(payload: CreateProjectRequest): Observable<GetProjects> {
    return this.http.post<GetProjects>(this.projectApi, payload);
  }

  getMembers(projectId: number): Observable<ProjectMembers[]> {
    return this.http.get<ProjectMembers[]>(`${this.projectmembersApi}/${projectId}`).pipe(tap(res => console.log('GET /api/projectmembers)', res)
  ));

}

  addMembers(payload: CreateMembers): Observable<ProjectMembers> {
    return this.http.post<ProjectMembers>('https://localhost:7033/api/projectmembers', payload);
  }

  removeMember(projectId: number, userId: number): Observable<void> {
    return this.http.delete<void>(`https://localhost:7033/api/projectmembers/project/${projectId}/user/${userId}`);
  }

}
