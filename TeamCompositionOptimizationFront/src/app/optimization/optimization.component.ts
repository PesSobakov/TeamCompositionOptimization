import { Component, ElementRef, ViewChild } from '@angular/core';
import { Observable, firstValueFrom } from 'rxjs';
import { Candidate } from './candidate';
import { Competency } from './competency';
import { Indicator } from './indicator';
import { ApiService } from '../api.service';
import { ActivatedRoute, Router } from '@angular/router';
import { OptimizationResult } from './optimizationResult';
import { OptimizationDto } from './optimizationDto';
import { CandidateCompetency } from './candidateCompetency';
import { HttpErrorResponse } from '@angular/common/http';
import { SelectDto } from './selectDto';
import { GeneralizedCompetence } from './generalizedCompetence';
import domToImage from 'dom-to-image';
import { jsPDF, jsPDFOptions } from 'jspdf';
import { TeamOption } from './teamOption';
import { selectResult } from './selectResult';

@Component({
  selector: 'app-optimization',
  templateUrl: './optimization.component.html',
  styleUrl: './optimization.component.css'
})
export class OptimizationComponent
{
  constructor(
    private api: ApiService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  candidates$?: Observable<Candidate[]> = this.api.getCandidates();
  competencies$?: Observable<Competency[]> = this.api.getCompetencies();
  results$?: Observable<OptimizationResult[]> = this.api.getResult();
  candidates: Candidate[] = [];
  competencies: Competency[] = [];
  results: OptimizationResult[] = [];
  currentResult?: OptimizationResult;
  indicators: Indicator[] = [];
  indicatorsValid: IndicatorValid[] = [];

  checkedCandidates: CandidateChecked[] = [];
  checkedIndicators: IndicatorChecked[] = [];

  candidateEdit: Candidate = <Candidate>{};
  candidateEditValid: CandidateCompetencyValid[] = [];
  candidateEditId?: number;
  candidateCreate: Candidate = <Candidate>{};
  candidateCreateValid: CandidateCompetencyValid[] = [];
  isCreatingCandidate: boolean = false;

  competencyEdit: Competency = <Competency>{};
  competencyEditId?: number;
  competencyCreate: Competency = <Competency>{};
  isCreatingCompetency: boolean = false;

  threshold: number = 1;
  budget: number = 10000;
  laboriousness: number = 10;
  time: number = 1;

  optimizationError?: string;

  selectionResult?: selectResult;

  getSelectionCandidateById(id: number)
  {
    return this.selectionResult?.candidates.find((x) => { return x.id == id; });
  }

  selectedCandidates?: Candidate[] = [];

  roundNumber(number: number | undefined)
  {
    return Number(number?.toFixed(5));
  }

  competencyName(id: number) 
  {
    return this.competencies.find(x => x.id == id)?.name ?? ($localize`not found`);
  }
  getCompetency(candidate: Candidate | undefined, competency: Competency | undefined)
  {
    return candidate?.competencies.find((x) => { return x.competencyId == competency?.id; });
  }
  getCompetencyById(id: number)
  {
    return this.competencies.find((x) => { return x.id == id; });
  }
  getCandidateById(id: number)
  {
    return this.candidates.find((x) => { return x.id == id; });
  }
  getCheckedCandidate(candidate: Candidate)
  {
    return this.checkedCandidates.find((x) => { return x.id == candidate.id; }) ?? <CandidateChecked>{};
  }
  getCheckedIndicator(indicator: Indicator)
  {
    return this.checkedIndicators.find((x) => { return x.id == indicator.competencyId; }) ?? <IndicatorChecked>{};
  }
  getCompliance(teamOption: TeamOption, indicator: Indicator)
  {
    return teamOption.maxCompetencies.find((x) => { return x.competencyId == indicator.competency.id })
  }

  getIndicatorValid(indicator: Indicator)
  {
    return this.indicatorsValid.find((x) => { return x.id == indicator.competencyId; }) ?? <IndicatorValid>{};
  }
  changeIndicatorValidValue(indicator: Indicator, valid: boolean)
  {
    var indicatorValid = (this.indicatorsValid.find((x) => { return x.id == indicator.competencyId; }) ?? <IndicatorValid>{});
    indicatorValid.isValidValue = valid;
  }
  changeIndicatorValidDeviation(indicator: Indicator, valid: boolean)
  {
    var indicatorValid = (this.indicatorsValid.find((x) => { return x.id == indicator.competencyId; }) ?? <IndicatorValid>{});
    indicatorValid.isValidDeviation = valid;
  }
  changeIndicatorValidWeight(indicator: Indicator, valid: boolean)
  {
    var indicatorValid = (this.indicatorsValid.find((x) => { return x.id == indicator.competencyId; }) ?? <IndicatorValid>{});
    indicatorValid.isValidWeight = valid;
  }
  isIndicatorsValid()
  {
    for (let indicatorValid of this.indicatorsValid) {
      if (indicatorValid.isValidDeviation == false || indicatorValid.isValidValue == false || indicatorValid.isValidWeight == false) {
        return false;
      }
    }
    return true;
  }
  isIndicatorsValueValid()
  {
    for (let indicatorValid of this.indicatorsValid) {
      if (indicatorValid.isValidValue == false) {
        return false;
      }
    }
    return true;
  }
  isIndicatorsDeviationValid()
  {
    for (let indicatorValid of this.indicatorsValid) {
      if (indicatorValid.isValidDeviation == false) {
        return false;
      }
    }
    return true;
  }
  isIndicatorsWeightValid()
  {
    for (let indicatorValid of this.indicatorsValid) {
      if (indicatorValid.isValidWeight == false) {
        return false;
      }
    }
    return true;
  }

  changeCandidateEditValidValue(competency: Competency, valid: boolean)
  {
    var candidateEditValid = (this.candidateEditValid.find((x) => { return x.id == competency.id; }) ?? <CandidateCompetencyValid>{});
    candidateEditValid.isValidValue = valid;
  }
  changeCandidateEditValidDeviation(competency: Competency, valid: boolean)
  {
    var candidateEditValid = (this.candidateEditValid.find((x) => { return x.id == competency.id; }) ?? <CandidateCompetencyValid>{});
    candidateEditValid.isValidDeviationRight = valid;
  }
  isCandidateEditValid()
  {
    for (let candidateEditValid of this.candidateEditValid) {
      if (candidateEditValid.isValidValue == false || candidateEditValid.isValidDeviationRight == false) {
        return false;
      }
    }
    return true;
  }

  changeCandidateCreateValidValue(competency: Competency, valid: boolean)
  {
    var candidateCreateValid = (this.candidateCreateValid.find((x) => { return x.id == competency.id; }) ?? <CandidateCompetencyValid>{});
    candidateCreateValid.isValidValue = valid;
  }
  changeCandidateCreateValidDeviation(competency: Competency, valid: boolean)
  {
    var candidateCreateValid = (this.candidateCreateValid.find((x) => { return x.id == competency.id; }) ?? <CandidateCompetencyValid>{});
    candidateCreateValid.isValidDeviationRight = valid;
  }
  isCandidateCreateValid()
  {
    for (let candidateCreateValid of this.candidateCreateValid) {
      if (candidateCreateValid.isValidValue == false || candidateCreateValid.isValidDeviationRight == false) {
        return false;
      }
    }
    return true;
  }

  isSelectedCandidates()
  {
    if (this.checkedCandidates.find((x) => { return x.isChecked == true; }) != undefined) {
      return true;
    }
    else return false;
  }

  isSelectedCompetencies()
  {
    if (this.checkedIndicators.find((x) => { return x.isChecked == true; }) != undefined) {
      return true;
    }
    else return false;
  }


  fixMissingCompetencies()
  {
    for (var candidate of this.candidates) {
      let changed = false;
      for (var competency of this.competencies) {
        let candidateCompetency = this.getCompetency(candidate, competency);
        if (candidateCompetency === undefined) {
          candidateCompetency = <CandidateCompetency>{ competencyId: competency.id };
          candidate.competencies.push(candidateCompetency);
          changed = true;
        }
      }
      let filteredCandidateCompetencies = candidate.competencies.filter((x) => { return (this.getCompetencyById(x.competencyId) !== undefined); });
      if (filteredCandidateCompetencies.length != candidate.competencies.length) {
        candidate.competencies = filteredCandidateCompetencies;
        changed = true;
      }
      if (changed) {
        this.api.patchCandidate(candidate.id, candidate).subscribe({
          error: (error: HttpErrorResponse) =>
          {
            if (error.status == 401) {
              this.router.navigate(['/login']);
            }
          },
          complete: () =>
          {
            this.updateCandidates();
          }
        });
      }
    }
  }

  optimize()
  {
    this.optimizationError = undefined;
    let optimizationDto: OptimizationDto = <OptimizationDto>{};
    optimizationDto.threshold = this.threshold;
    optimizationDto.budget = this.budget;
    optimizationDto.laboriousness = this.laboriousness;
    optimizationDto.time = this.time;
    optimizationDto.candidates = this.candidates.filter((item) => { return this.getCheckedCandidate(item).isChecked });
    optimizationDto.indicators = this.indicators.filter((item) => { return this.getCheckedIndicator(item).isChecked });
    this.api.optimize(optimizationDto).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
        else if (error.error.errors.message) {
          this.optimizationError = error.error.errors.message;
        }
      },
      complete: () =>
      {
        this.updateResults();
      }
    });
  }

  select()
  {
    this.optimizationError = undefined;
    let selectDto: SelectDto = <SelectDto>{};
    selectDto.candidates = this.candidates.filter((item) => { return this.getCheckedCandidate(item).isChecked });
    selectDto.indicators = this.indicators.filter((item) => { return this.getCheckedIndicator(item).isChecked });
    this.api.select(selectDto).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
        else if (error.error.errors.message) {
          this.optimizationError = error.error.errors.message;
        }
      },
      next: (res) =>
      {
        this.selectionResult = res;
      }
    });
  }

  export()
  {
    const data = document.getElementById('pdf');
    if (data) {
      const scale = 4;
      const width = data.clientWidth;
      const height = data.clientHeight;
      domToImage.toPng(data, {
        width: width * scale,
        height: height * scale,
        style: {
          transform: "scale(" + scale + ")",
          transformOrigin: "top left"
        }
      }).then(result =>
      {
        let orientation;
        if (width > height) {
          orientation = 'l';
        } else {
          orientation = 'p';
        }
        let jsPdfOptions = <jsPDFOptions>{
          orientation: orientation,
          unit: 'px',
          format: [width, height],
          compress: true
        };
        const pdf = new jsPDF(jsPdfOptions);
        pdf.addImage(result, 'PNG', 0, 0, width, height);
        pdf.save($localize`Result` + '.pdf');
      }).catch(error =>
      {
      });
    }
  }

  editCompetency(competency: Competency)
  {
    this.competencyEditId = competency.id;
    this.competencyEdit = structuredClone(competency);
  }
  confirmEditCompetency(competency: Competency)
  {
    this.api.patchCompetency(competency.id, competency).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      },
      complete: () =>
      {
        this.updateCompetencies();
        this.competencyEdit = <Candidate>{};
        this.competencyEditId = undefined;
      }
    });
  }
  cancelEditCompetency()
  {
    this.competencyEdit = <Competency>{};
    this.competencyEditId = undefined;
  }
  deleteCompetency(id: number)
  {
    this.api.deleteCompetency(id).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      },
      complete: () =>
      {
        this.updateCompetencies();
        this.updateCandidates();
      }
    });
  }
  createCompetency()
  {
    this.competencyCreate = <Competency>{};
    this.isCreatingCompetency = true;
  }
  createCompetencyConfirm(competency: Competency)
  {
    this.api.postCompetency(competency).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      },
      complete: () =>
      {
        this.updateCompetencies();
        this.competencyCreate = <Competency>{};
        this.isCreatingCompetency = false;
        this.fixMissingCompetencies();
      }
    });
  }
  createCompetencyCancel()
  {
    this.competencyCreate = <Competency>{};
    this.isCreatingCompetency = false;
  }

  editCandidate(candidate: Candidate)
  {
    this.candidateEditId = candidate.id;
    this.candidateEdit = structuredClone(candidate);
    this.candidateEditValid = this.candidateEdit.competencies.map((x) => { return <CandidateCompetencyValid>{ id: x.competencyId } });
  }
  confirmEditCandidate(candidate: Candidate)
  {
    this.api.patchCandidate(candidate.id, candidate).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      },
      complete: () =>
      {
        this.updateCandidates();
        this.candidateEdit = <Candidate>{};
        this.candidateEditId = undefined;
        this.candidateEditValid = [];
      }
    });
  }
  cancelEditCandidate()
  {
    this.candidateEdit = <Candidate>{};
    this.candidateEditId = undefined;
    this.candidateEditValid = [];
  }
  deleteCandidate(id: number)
  {
    this.api.deleteCandidate(id).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      },
      complete: () =>
      {
        this.updateCandidates();
      }
    });
  }
  createCandidate()
  {
    this.candidateCreate = <Candidate>{ salary: 10, workingTime: 40 };
    this.candidateCreate.competencies = this.competencies.map((item) => { return <CandidateCompetency>{ competencyId: item.id, value: 0, deviationLeft: 0, deviationRight: 0 } })
    this.isCreatingCandidate = true;
    this.candidateCreateValid = this.candidateCreate.competencies.map((x) => { return <CandidateCompetencyValid>{ id: x.competencyId } });
  }
  createCandidateConfirm(candidate: Candidate)
  {
    this.api.postCandidate(candidate).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      },
      complete: () =>
      {
        this.updateCandidates();
        this.candidateCreate = <Candidate>{};
        this.isCreatingCandidate = false;
        this.candidateCreateValid = [];
      }
    });
  }
  createCandidateCancel()
  {
    this.candidateCreate = <Candidate>{};
    this.isCreatingCandidate = false;
    this.candidateCreateValid = [];
  }

  selectResult(id: number)
  {
    this.currentResult = this.results.find((item) => { return item.id == id });
  }
  deleteResult(id: number)
  {
    this.api.deleteResult(id).subscribe({
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      },
      complete: () =>
      {
        this.updateResults();
      }
    });
  }

  updateCandidates()
  {
    this.candidates$?.subscribe({
      next: (res) =>
      {
        this.candidates = res ?? [];

        let newCheckedCandidates = this.candidates.map((item) => { return <CandidateChecked>{ id: item.id, isChecked: false } });
        for (var i = 0; i < newCheckedCandidates.length; i++) {
          let checkedCandidate = this.checkedCandidates.find((x) => { return newCheckedCandidates[i].id == x.id });
          if (checkedCandidate) {
            newCheckedCandidates[i] = checkedCandidate;
          }
        }
        this.checkedCandidates = newCheckedCandidates;

        this.fixMissingCompetencies();
      },
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      }
    });
  }
  updateCompetencies()
  {
    this.competencies$?.subscribe({
      next: (res) =>
      {
        this.competencies = res ?? [];
        this.indicators = this.competencies.map((item) => { return <Indicator>{ competencyId: item.id, value: 1, deviation: 0, weight: 1 } });

        let newCheckedIndicators = this.competencies.map((item) => { return <IndicatorChecked>{ id: item.id, isChecked: false } });
        for (var i = 0; i < newCheckedIndicators.length; i++) {
          let checkedIndicator = this.checkedIndicators.find((x) => { return newCheckedIndicators[i].id == x.id });
          if (checkedIndicator) {
            newCheckedIndicators[i] = checkedIndicator;
          }
        }
        this.checkedIndicators = newCheckedIndicators;

        this.indicatorsValid = this.competencies.map((item) => { return <IndicatorValid>{ id: item.id } });


        this.fixMissingCompetencies();
      },
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      }
    });
  }
  updateResults()
  {
    this.results$?.subscribe({
      next: (res) =>
      {
        this.results = res ?? [];
        if (this.results.length > 0) {
          this.currentResult = this.results[0];
        }
        else {
          this.currentResult = undefined;
        }
      },
      error: (error: HttpErrorResponse) =>
      {
        if (error.status == 401) {
          this.router.navigate(['/login']);
        }
      }
    });
  }

  ngOnInit()
  {
    this.updateCandidates();
    this.updateCompetencies();
    this.updateResults();
  }
}

interface CandidateChecked
{
  id: number;
  isChecked: boolean;
}
interface IndicatorChecked
{
  id: number;
  isChecked: boolean;
}

interface IndicatorValid
{
  id: number;
  isValidValue: boolean;
  isValidDeviation: boolean;
  isValidWeight: boolean;
}

interface CandidateCompetencyValid
{
  id: number;
  isValidValue: boolean;
  isValidDeviationRight: boolean;
}
