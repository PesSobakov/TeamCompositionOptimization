import { Injectable } from '@angular/core';
import { environment } from './../environments/environment';

import { BrowserModule } from "@angular/platform-browser";
import { platformBrowserDynamic } from "@angular/platform-browser-dynamic";
import { ReactiveFormsModule, FormControl, FormsModule } from "@angular/forms";

import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import
{
  map,
  debounceTime,
  distinctUntilChanged,
  switchMap,
  tap
} from "rxjs/operators";
import { HelpPage } from './help/helpPage';
import { Candidate } from './optimization/candidate';
import { Competency } from './optimization/competency';
import { OptimizationDto } from './optimization/optimizationDto';
import { OptimizationResult } from './optimization/optimizationResult';
import { AccountInfo } from './login/accountInfo';
import { Credentials } from './login/credentials';
import { Subscription } from './login/subscription';
import { SelectDto } from './optimization/selectDto';
import { GeneralizedCompetence } from './optimization/generalizedCompetence';
import { selectResult } from './optimization/selectResult';



@Injectable({
  providedIn: 'root'
})

@Injectable()
export class ApiService
{
  readonly endpoint: string = `https://${environment.server}/`;

  constructor(private http: HttpClient) { }

  login(credentials: Credentials)
  {
    let url = `${this.endpoint}api/auth/login`;
    return this.http
      .post(url, credentials, { withCredentials: true });
  }
  logout()
  {
    let url = `${this.endpoint}api/auth/logout`;
    return this.http
      .get(url, { withCredentials: true });
  }
  register(credentials: Credentials)
  {
    let url = `${this.endpoint}api/auth/register`;
    return this.http
      .post(url, credentials, { withCredentials: true });
  }
  deleteAccount()
  {
    let url = `${this.endpoint}api/auth/deleteaccount`;
    return this.http
      .get(url, { withCredentials: true });
  }
  accountInfo()
  {
    let url = `${this.endpoint}api/auth/accountinfo`;
    return this.http
      .get<AccountInfo>(url, { withCredentials: true })
      .pipe(map((item) =>
      {
        return <AccountInfo> {
          isSuperuser: item.isSuperuser,
          login: item.login,
          subscription: item.subscription,
          subscriptionDueDate: new Date(item.subscriptionDueDate)
        }
      }));
  }
  subscriptions()
  {
    let url = `${this.endpoint}api/auth/subscriptions`;
    return this.http
      .get<Subscription[]>(url, { withCredentials: true });
  }
  changeSubscription(id:number)
  {
    let url = `${this.endpoint}api/auth/changesubscription/${id}`;
    return this.http
      .get(url, { withCredentials: true });
  }

  getHelpNames(): Observable<HelpPage[]>
  {
    let url = `${this.endpoint}api/help/get`;
    return this.http
      .get<HelpPage[]>(url, { withCredentials: true });
  }

  getLocalizedHelpNames(locale:string): Observable<HelpPage[]>
  {
    let url = `${this.endpoint}api/help/getlocalized/${locale}`;
    return this.http
      .get<HelpPage[]>(url, { withCredentials: true });
  }

  getHelp(id: number): Observable<HelpPage> 
  {
    let url = `${this.endpoint}api/help/get/${id}`;
    return this.http
      .get<HelpPage>(url, { withCredentials: true });
  }

  postHelp(helpPage: HelpPage)
  {
    let url = `${this.endpoint}api/help/post`;
    return this.http
      .post(url, helpPage, { withCredentials: true });
  }

  patchHelp(id: number, helpPage: HelpPage)
  {
    let url = `${this.endpoint}api/help/patch/${id}`;
    return this.http
      .patch(url, helpPage, { withCredentials: true });
  }

  deleteHelp(id: number)
  {
    let url = `${this.endpoint}api/help/delete/${id}`;
    return this.http
      .delete(url, { withCredentials: true });
  }


  getCandidates()
  {
    let url = `${this.endpoint}api/optimization/candidates`;
    return this.http
      .get<Candidate[]>(url, { withCredentials: true });
  }

  getCandidate(id: number)
  {
    let url = `${this.endpoint}api/optimization/candidates/${id}`;
    return this.http
      .get<Candidate>(url, { withCredentials: true });
  }

  postCandidate(candidate: Candidate)
  {
    let url = `${this.endpoint}api/optimization/candidates`;
    return this.http
      .post(url, candidate, { withCredentials: true });
  }

  patchCandidate(id: number, candidate: Candidate)
  {
    let url = `${this.endpoint}api/optimization/candidates/${id}`;
    return this.http
      .patch(url, candidate, { withCredentials: true });
  }

  deleteCandidate(id: number)
  {
    let url = `${this.endpoint}api/optimization/candidates/${id}`;
    return this.http
      .delete(url, { withCredentials: true });
  }

  getCompetencies()
  {
    let url = `${this.endpoint}api/optimization/competencies`;
    return this.http
      .get<Competency[]>(url, { withCredentials: true });
  }

  getCompetency(id: number)
  {
    let url = `${this.endpoint}api/optimization/competencies/${id}`;
    return this.http
      .get<Competency>(url, { withCredentials: true });
  }

  postCompetency(competency: Competency)
  {
    let url = `${this.endpoint}api/optimization/competencies`;
    return this.http
      .post(url, competency, { withCredentials: true });
  }

  patchCompetency(id: number, competency: Competency)
  {
    let url = `${this.endpoint}api/optimization/competencies/${id}`;
    return this.http
      .patch(url, competency, { withCredentials: true });
  }

  deleteCompetency(id: number)
  {
    let url = `${this.endpoint}api/optimization/competencies/${id}`;
    return this.http
      .delete(url, { withCredentials: true });
  }

  optimize(optimizationDto: OptimizationDto)
  {
    let url = `${this.endpoint}api/optimization/optimize`;
    return this.http
      .post(url, optimizationDto, { withCredentials: true });
  }

  select(selectDto: SelectDto)
  {
    let url = `${this.endpoint}api/optimization/select`;
    return this.http
      .post<selectResult>(url, selectDto, { withCredentials: true });
  }

  getResult()
  {
    let url = `${this.endpoint}api/optimization/result`;
    return this.http
      .get<OptimizationResult[]>(url, { withCredentials: true });
  }

  deleteResult(id: number)
  {
    let url = `${this.endpoint}api/optimization/result/${id}`;
    return this.http
      .delete(url, { withCredentials: true });
  }
}
