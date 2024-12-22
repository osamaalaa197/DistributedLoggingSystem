import { Component, inject } from '@angular/core';
import { LogService } from '../Services/log.service';
import { LogParameters } from '../SharedClass/logParameters-view-model';
import { ResponsAPI } from '../SharedClass/respons-api';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../Services/user.service';
import { UnauthorizedComponent } from '../unauthorized/unauthorized.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-log',
  standalone: true,
  imports: [FormsModule, CommonModule, UnauthorizedComponent],
  templateUrl: './log.component.html',
  styleUrl: './log.component.css',
})
export class LogComponent {
  userService = inject(UserService);
  logs: any[] = [];
  totalRecords: number = 0;
  pageNumber: number = 1;
  pageSize: number = 5;
  isLoading: boolean = false;
  logLevels: string[] = ['INFO', 'WARN', 'ERROR'];
  serviceFilter: string = '';
  levelFilter: string = '';
  startTimeFilter: string = '';
  endTimeFilter: string = '';
  constructor(private _logService: LogService, private router: Router) {}
  ngOnInit(): void {
    this.loadLogs();
  }
  loadLogs(): void {
    this.isLoading = true;
    const logParameters: LogParameters = {
      Service: this.serviceFilter,
      Level: this.levelFilter,
      StartTime: this.startTimeFilter,
      EndTime: this.endTimeFilter,
      PageNumber: this.pageNumber,
      PageSize: this.pageSize,
    };
    this._logService.getLogs(logParameters).subscribe(
      (response: ResponsAPI) => {
        if (response.isSuccess) {
          console.log(response);
          this.logs = response.result;
          this.totalRecords = response.totalRecords;
          if (response.PageSize != null) {
            this.pageSize = response.PageSize;
          }
          if (response.PageNumber != null) {
            this.pageNumber = response.PageNumber;
          }
        } else {
          console.error(response.message);
        }
        this.isLoading = false;
      },
      (error) => {
        console.error('Error fetching logs:', error);
        this.isLoading = false;
      }
    );
  }
  changePage(newPage: number): void {
    this.pageNumber = newPage;
    this.loadLogs();
  }
  applyFilters(): void {
    this.pageNumber = 1;
    this.loadLogs();
  }

  viewLogDetails(id: number | null): void {
    if (id != null) {
      this.router.navigate(['/log', id]);
    }
  }
}
