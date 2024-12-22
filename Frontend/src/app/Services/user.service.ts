import { Injectable, signal } from '@angular/core';
import { LoginViewModel } from '../SharedClass/login-view-model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthViewModel } from '../SharedClass/auth-view-model';
import { ResponsAPI } from '../SharedClass/respons-api';
import { environment } from '../../../enviroment';
import { map } from 'rxjs';
import { RegisterViewModel } from '../SharedClass/register-view-model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private _http: HttpClient) {}
  currenUser = signal<string | null>(null);

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };
  LogIn(data: LoginViewModel) {
    return this._http
      .post<ResponsAPI<AuthViewModel>>(
        `${environment.apiUrl}api/User/LogIn`,
        data,
        this.httpOptions
      )
      .pipe(
        map((response) => {
          if (response) {
            localStorage.setItem('token', response.result.token);
            this.currenUser.set(response.result.token);
          }
        })
      );
  }

  Register(data: RegisterViewModel) {
    return this._http.post<ResponsAPI<object>>(
      `${environment.apiUrl}api/User/Register`,
      data,
      this.httpOptions
    );
  }

  Logout() {
    localStorage.removeItem('token');
    this.currenUser.set(null);
  }
}
