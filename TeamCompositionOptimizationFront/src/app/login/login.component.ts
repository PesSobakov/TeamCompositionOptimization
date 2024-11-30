import { Component } from '@angular/core';
import { Credentials } from './credentials';
import { ApiService } from '../api.service';
import { AccountInfo } from './accountInfo';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent
{
  constructor(
    private api: ApiService,
    private route: ActivatedRoute,
    private router: Router,
  )
  {
    this.router.routeReuseStrategy.shouldReuseRoute = ()=>false;
}
  credentials: Credentials = <Credentials>{};
  accountInfo?: AccountInfo;
  error?: string;
  login()
  {
    this.api.login(this.credentials).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.error = $localize`Wrong credentials`;
        }
        else {
          this.error = $localize`Login error`;
        }
      },
      next: () =>
      {
        this.router.navigate(["/login"]);
        this.updateAccountInfo();
        this.error = undefined;
      }
    });
  }
  logout()
  {
    this.api.logout().subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.error = $localize`Unauthorized`;
        }
        else {
          this.error = $localize`Logout error`;
        }
      },
      next: () =>
      {
        this.router.navigate(["/login"]);
        this.updateAccountInfo();
        this.error = undefined;
      }
    });
  }
  register()
  {
    this.api.register(this.credentials).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.error.errors.login) {
          if (error.error.errors.login == "This email already used") {
            this.error = $localize`This email already used`;
          }
          else {
            this.error = $localize`Register error`;
          }
        }
      },
      complete: () =>
      {
        this.router.navigate(["/login"]);
        this.updateAccountInfo();
        this.error = undefined;
      }
    });
  }
  deleteAccount()
  {
    this.api.deleteAccount().subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.error = $localize`Unauthorized`;
        }
        else {
          this.error = $localize`Delete account error`;
        }
      },
      complete: () =>
      {
        this.router.navigate(["/login"]);
        this.updateAccountInfo();
        this.error = undefined;
      }
    });
  }
  updateAccountInfo()
  {
    this.api.accountInfo().subscribe({
      next: (res) =>
      {
        this.accountInfo = res;
        this.error = undefined;
      },
      error: () => { this.accountInfo = undefined; }
    });
  }

  ngOnInit()
  {
    this.updateAccountInfo();
  }
}
