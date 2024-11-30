import { Component } from '@angular/core';
import { AccountInfo } from '../login/accountInfo';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../api.service';
import { Subscription } from '../login/subscription';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrl: './account.component.css'
})
export class AccountComponent
{
  constructor(
    private api: ApiService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  accountInfo?: AccountInfo;
  subscriptions?: Subscription[];
;

  updateAccountInfo()
  {
    this.api.accountInfo().subscribe((res) =>
    {
      this.accountInfo = res;
    });
  }
  updateSubscriptions()
  {
    this.api.subscriptions().subscribe((res) =>
    {
      this.subscriptions = res;
    });
  }
  changeSubscription(id:number)
  {
    this.api.changeSubscription(id).subscribe(() =>
    {
      this.updateAccountInfo();
      this.updateSubscriptions();
    });
  }
  
  ngOnInit()
  {
    this.updateAccountInfo();
    this.updateSubscriptions();
  }

}
