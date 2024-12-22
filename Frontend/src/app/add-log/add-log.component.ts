import { Component, inject, Inject } from '@angular/core';
import { LogViewModel } from '../SharedClass/log-view-model';
import { ResponsAPI } from '../SharedClass/respons-api';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LogService } from '../Services/log.service';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../Services/user.service';
import { UnauthorizedComponent } from '../unauthorized/unauthorized.component';
import { Router } from '@angular/router';
@Component({
  selector: 'app-add-log',
  standalone: true,
  imports: [FormsModule, CommonModule, UnauthorizedComponent],
  templateUrl: './add-log.component.html',
  styleUrl: './add-log.component.css',
})
export class AddLogComponent {
  userService = inject(UserService);
  Model: LogViewModel = {
    Service: '',
    Message: '',
    Level: '',
    Timestamp: '',
  };
  logLevels: string[] = ['INFO', 'WARN', 'ERROR'];
  constructor(
    private _logService: LogService,
    private toastr: ToastrService,
    private router: Router
  ) {}
  addLog() {
    this._logService.AddLog(this.Model).subscribe({
      next: (response: ResponsAPI<object>) => {
        if (response.isSuccess) {
          console.log(response);
          this.toastr.success(response.message);
          this.router.navigate(['Log']);
        } else {
          this.toastr.error(response.message);
        }
      },
    });
    console.log(this.Model);
  }
}
