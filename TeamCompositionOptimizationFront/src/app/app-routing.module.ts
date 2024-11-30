import { LOCALE_ID, NgModule } from '@angular/core';
import { ExtraOptions, RouteReuseStrategy, RouterModule, Routes } from '@angular/router';
import { OptimizationComponent } from './optimization/optimization.component';
import { HelpComponent } from './help/help.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { AccountComponent } from './account/account.component';

const routes: Routes = [
  {
    path: 'optimization',
    component: OptimizationComponent
  },
  {
    path: 'help',
    component: HelpComponent
  },
  {
    path: 'help/:id',
    component: HelpComponent
  },
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'account',
    component: AccountComponent
  },
  { path: '', redirectTo: '/home', pathMatch: 'full' }/*,
  { path: '**', component: PageNotFoundComponent }*/
];

@NgModule({
  imports: [RouterModule.forRoot(routes, <ExtraOptions>{ bindToComponentInputs: true, onSameUrlNavigation: 'reload' } )],
  exports: [RouterModule]
})
export class AppRoutingModule { }
