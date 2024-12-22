import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { LogParameters } from '../SharedClass/logParameters-view-model';
import { ResponsAPI } from '../SharedClass/respons-api';
import { environment } from '../../../enviroment';
import { map, Observable } from 'rxjs';
import { LoginViewModel } from '../SharedClass/login-view-model';
import { LogViewModel } from '../SharedClass/log-view-model';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root',
})
export class LogService {
  private userService = inject(UserService);
  constructor(private _http: HttpClient) {}
  httpOptions = {
    headers: new HttpHeaders({
      Authorization: `Bearer ${this.userService.currenUser()}`,
      'Content-Type': 'application/json',
    }),
  };

  getLogs(logParameters: LogParameters): Observable<ResponsAPI<LogViewModel>> {
    let params = new HttpParams();
    if (logParameters.Service) {
      params = params.set('service', logParameters.Service);
    }
    if (logParameters.Level) {
      params = params.set('level', logParameters.Level);
    }
    if (logParameters.StartTime) {
      params = params.set('startTime', logParameters.StartTime);
    }
    if (logParameters.EndTime) {
      params = params.set('endTime', logParameters.EndTime);
    }
    if (logParameters.PageNumber) {
      params = params.set('pageNumber', logParameters.PageNumber.toString());
    }
    if (logParameters.PageSize) {
      params = params.set('pageSize', logParameters.PageSize.toString());
    }
    return this._http.get<ResponsAPI<LogViewModel>>(
      `${environment.apiUrl}v1/logs`,
      {
        params: params,
        headers: this.httpOptions.headers,
      }
    );
  }
  getLogById(id: number): Observable<ResponsAPI<object>> {
    let params = new HttpParams();
    if (id != null) {
      params = params.set('id', id);
    }
    return this._http.get<ResponsAPI<object>>(
      `${environment.apiUrl}v1/logs/GetLogById`,
      {
        params: params,
        headers: this.httpOptions.headers,
      }
    );
  }
  AddLog(logViewModel: LogViewModel): Observable<ResponsAPI<object>> {
    return this._http.post<ResponsAPI<object>>(
      `${environment.apiUrl}v1/logs`,
      logViewModel,
      { headers: this.httpOptions.headers }
    );
  }
}
