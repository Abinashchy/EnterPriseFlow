import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = '/api/users';

  constructor(private http: HttpClient){}

  getUsers(){
    console.log("Service Called")
    return this.http.get<any[]>(this.apiUrl);
  }

  getRoles(){
    return this.http.get<any[]>('/api/roles');
  }

  getDepartments(){
    return this.http.get<any[]>('/api/department');
  }

  createUser(user:any){
    console.log("Post Method Called")
    return this.http.post(this.apiUrl, user);
  }

  updateUser(id:number,user:any){
    return this.http.put(`/api/users/${id}`,user);
  }

  deleteUser(id:number){
    return this.http.delete(`/api/users/${id}`);
  }

}
