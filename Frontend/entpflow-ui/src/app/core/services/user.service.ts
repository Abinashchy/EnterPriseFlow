import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import {
  CreateUserRequest,
  DepartmentOption,
  RoleOption,
  UpdateUserRequest,
  UserRow
} from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly usersApi = '/api/users';
  private readonly rolesApi = '/api/roles';
  private readonly departmentsApi = '/api/department';

  constructor(private http: HttpClient) {}

  getUsers(): Observable<UserRow[]> {
    return this.http.get<UserRow[]>(this.usersApi).pipe(
    tap(res => console.log('GET /api/users', res))
  );
  }

  getRoles(): Observable<RoleOption[]> {
    return this.http.get<RoleOption[]>(this.rolesApi).pipe(
    tap(res => console.log('GET /api/roles', res))
  );
  }

  getDepartments(): Observable<DepartmentOption[]> {
    return this.http.get<DepartmentOption[]>(this.departmentsApi).pipe(
    tap(res => console.log('Callig service GET /api/department', res))
  );
  }

  createUser(payload: CreateUserRequest): Observable<UserRow> {
    return this.http.post<UserRow>(this.usersApi, payload);
  }

  updateUser(id: number, payload: UpdateUserRequest): Observable<void> {
    return this.http.put<void>(`${this.usersApi}/${id}`, payload);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.usersApi}/${id}`);
  }
}
