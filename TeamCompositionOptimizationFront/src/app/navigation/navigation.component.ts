import { environment } from './../../environments/environment';

import { Component, inject, Inject, LOCALE_ID } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { filter, map, shareReplay } from 'rxjs/operators';
import { AccountInfo } from '../login/accountInfo';
import { ActivatedRoute, NavigationEnd, NavigationStart, Router } from '@angular/router';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css'
})
export class NavigationComponent
{
  constructor(
    private api: ApiService,
    private route: ActivatedRoute,
    private router: Router,
    @Inject(LOCALE_ID) public locale: string
  )
  {
    router.events.pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe({
        next: (e: NavigationEnd) =>
        {
          this.url = e.url;
          this.updateAccountInfo();
        }
      });
    router.events.pipe(filter((e): e is NavigationStart => e instanceof NavigationStart))
      .subscribe({
        next: (e: NavigationStart) =>
        {
          this.url = e.url;
          this.updateAccountInfo();
        }
      })
  }

  private breakpointObserver = inject(BreakpointObserver);

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  accountInfo?: AccountInfo;
  url: string = "";

  checkNav(nav: string)
  {
    return this.url.includes(nav);
  }

  setSelectedNav(nav: string)
  {
    return this.url.includes(nav) ? "selected-nav" : "";
  }

  updateAccountInfo()
  {
    this.api.accountInfo().subscribe({
      next: (res) =>
      {
        this.accountInfo = res;
      },
      error: () =>
      {
        this.accountInfo = undefined;
      }
    });
  }

  isEn()
  {
    return this.locale == "en";
  }

  getChangeLanguageLink()
  {
    return "../../" + (this.isEn() ? "ua" : "en") + this.url
  }

  ngOnInit()
  {
    this.updateAccountInfo();
  }
}
