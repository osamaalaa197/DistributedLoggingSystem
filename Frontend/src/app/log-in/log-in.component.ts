import { Component } from '@angular/core';
import { LogInViewModel } from '../SharedClass/log-in-view-model';
import { UserService } from '../Services/user.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-log-in',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './log-in.component.html',
  styleUrl: './log-in.component.css',
})
export class LogInComponent {
  Model: LogInViewModel = {
    email: '',
    password: '',
  };
  constructor(private _userService: UserService, private router: Router) {}
  login() {
    this._userService.LogIn(this.Model).subscribe({
      next: (response) => {
        console.log(response);
        this.router.navigate(['Log']);
      },
      error: (err) => console.log(err),
    });
  }
}
