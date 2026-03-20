import { Component } from '@angular/core';
import { UserService } from '../../services/user';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';



@Component({
  selector: 'app-create-users',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './create-users.html',
  styleUrl: './create-users.css',
})
export class CreateUsers {

  newUser: any = {
    employeeId: '',
    name: '',
    email: '',
    passwordHash: '',
    role: '',
    department:''
  };
  constructor(private userService: UserService, private router: Router){}

  createUser(){
    console.log("Create  User clicked");
    console.log(this.newUser);
    this.userService.createUser(this.newUser).subscribe(() => {

      alert("User Created Successfully");
      this.router.navigateByUrl('/users');
    });
  }

}
