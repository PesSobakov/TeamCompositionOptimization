import { CandidateCompetency } from "./candidateCompetency";

export interface Candidate
{
  id: number;
  name: string;
  workingTime: number;
  salary: number;
  competencies: CandidateCompetency[];
}
