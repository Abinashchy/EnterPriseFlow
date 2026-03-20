import { Routes } from '@angular/router';
import { AdminLayout } from './layout/admin-layout/admin-layout';
import { Dashboard } from './admin/dashboard/dashboard';
import { Users } from './admin/users/users';
import { CreateUsers } from './admin/create-users/create-users';


export const routes: Routes = [
{
  path:'',
  component:AdminLayout,
  children: [
    { path : 'dashboard', component: Dashboard },
    { path : 'users',     component : Users},
    { path : 'create-users', component : CreateUsers},
    { path : '', redirectTo: 'dashboard', pathMatch: 'full'}
  ]
}
];
