import { RegisterViewModel } from './../SharedClass/register-view-model';
import { Component, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ResponsAPI } from '../SharedClass/respons-api';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { UserService } from '../Services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  Model: RegisterViewModel = {
    email: '',
    password: '',
    phoneNumber: '',
    name: '',
  };
  cancelRegister = output<boolean>();
  constructor(
    private userService: UserService,
    private toastr: ToastrService,
    private router: Router
  ) {}
  register() {
    this.userService.Register(this.Model).subscribe({
      next: (response: ResponsAPI<object>) => {
        if (response.isSuccess) {
          this.toastr.success(response.message);
          this.router.navigate(['login']);
          this.cancel();
        } else {
          this.toastr.error(response.message);
        }
      },
    });
    console.log(this.Model);
  }
  cancel() {
    this.cancelRegister.emit(true);
  }
}
