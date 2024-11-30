import { Candidate } from "./candidate";
import { Indicator } from "./indicator";
import { TeamOption } from "./teamOption";

export interface OptimizationResult
{
  id: number,
  teamOptions: TeamOption[];
  indicators: Indicator[];
  candidates: Candidate[];
  threshold: number;
  budget: number;
  laboriousness: number;
  time: number;
}
