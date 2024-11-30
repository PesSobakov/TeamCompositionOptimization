import { Candidate } from "./candidate";
import { Compliance } from "./compliance";
import { Indicator } from "./indicator";

export interface TeamOption
{
  criteria1: number;
  criteria2: number;
  cost: number;
  teamworkTime: number;
  candidates: Candidate[];
  maxCompetencies: Compliance[];
}
