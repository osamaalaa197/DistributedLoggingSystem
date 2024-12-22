import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { FormsModule } from '@angular/forms';
import { UserService } from './Services/user.service';
import { LogComponent } from './log/log.component';
import { HomeComponent } from './home/home.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  constructor(private _userService: UserService) {}
  ngOnInit(): void {
    this.getCurrentUser();
  }
  getCurrentUser() {
    const token = localStorage.getItem('token');
    if (!token) return;
    this._userService.currenUser.set(token);
  }
}
