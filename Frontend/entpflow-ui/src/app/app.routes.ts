import { Routes } from '@angular/router';
import { AdminLayout } from './layout/admin-layout/admin-layout';
import { Dashboard } from './admin/dashboard/dashboard';
import { Users } from './admin/users/users';
import { CreateUsers } from './admin/create-users/create-users';
import { LoginComponent } from './pages/login/login';
import { UnauthorizedComponent } from './pages/unauthorized/unauthorized';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { Projects } from './admin/projects/projects';
import { CreateProjects } from './admin/create-projects/create-projects';
import { ProjectDetails } from './admin/project-details/project-details';
import { Tasks } from './admin/tasks/tasks';
import { TaskDetails } from './admin/task-details/task-details';
import { TaskBoard } from './admin/task-board/task-board';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'unauthorized',
    component: UnauthorizedComponent
  },
  {
    path: '',
    component: AdminLayout,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: Dashboard },
      {
        path: 'users',
        component: Users,
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'Manager'] }
      },
      {
        path: 'create-users',
        component: CreateUsers,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },
      {
        path: 'projects',
        component: Projects,
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'Manager'] }
      },

      {
        path: 'create-projects',
        component: CreateProjects,
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'Manager'] }
      },
      {
        path: 'projects/:id',
        component: ProjectDetails,
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'Manager'] }
      },

      {
        path: 'tasks',
        component: Tasks
      },

      {
        path: 'task-board',
        component: TaskBoard
      },

      {
        path: 'tasks/:id',
        component: TaskDetails
      },


      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
