import { Candidate } from "./candidate";
import { Indicator } from "./indicator";

export interface OptimizationDto
{
  indicators: Indicator[];
  candidates: Candidate[];
  threshold: number;
  budget: number;
  laboriousness: number;
  time: number;
}
