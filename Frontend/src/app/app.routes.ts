import { Routes } from '@angular/router';
import { LogComponent } from './log/log.component';
import { AddLogComponent } from './add-log/add-log.component';
import { RegisterComponent } from './register/register.component';
import { HomeComponent } from './home/home.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { LogInComponent } from './log-in/log-in.component';
import { LogdetailsComponent } from './logdetails/logdetails.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'Log', component: LogComponent },
  { path: 'AddLog', component: AddLogComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LogInComponent },
  { path: 'unauthorized', component: UnauthorizedComponent },
  { path: 'log/:id', component: LogdetailsComponent },
  { path: 'log/:serviceName', component: LogdetailsComponent },
  { path: '**', redirectTo: '/' },
];
