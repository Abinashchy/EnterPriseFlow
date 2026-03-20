import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './users.html',
  styleUrl: './users.css',
})
export class Users implements OnInit {

  users:any[] = [];
  roles:any[] = [];
  departments:any[] = [];

  editingUser:any = null;

  constructor(private userService:UserService, private router: Router){}

  ngOnInit(){
    this.loadUsers();
    this.loadRoles();
    this.loadDepartments();

  }

  loadUsers(){
    this.userService.getUsers().subscribe(data=>{
      this.users = data;
      console.log(data);
    });
  }

  editUser(user:any){
    this.editingUser = { ...user };
  }

  updateUser(){
    const payload = {

      name: this.editingUser.name,
      email: this.editingUser.email,
      role: this.editingUser.role,           // name
      department: this.editingUser.department // name

    };

    console.log(payload); // debug

    this.userService.updateUser(this.editingUser.userId, payload)
      .subscribe(()=>{
        alert('User Updated');
        this.editingUser = null;
        this.loadUsers();
        this.router.navigate(['users'])
      });
  }

  cancelEdit(){
    this.editingUser = null;

  }
  loadRoles(){
  this.userService.getRoles().subscribe(data=>{
    console.log('ROLES:', data);   // 👈 check this
    this.roles = data;
  });
}

loadDepartments(){
  this.userService.getDepartments().subscribe(data=>{
    console.log('DEPARTMENTS:', data); // 👈 check this
    this.departments = data;
  });
}
  deleteUser(id:number){
    if(confirm('Delete user?')){
      this.userService.deleteUser(id).subscribe(()=>{
        alert('User Deleted');
        this.loadUsers();
      });
    }
  }

}
