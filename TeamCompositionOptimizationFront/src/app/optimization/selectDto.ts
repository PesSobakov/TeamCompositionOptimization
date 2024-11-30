import { Candidate } from "./candidate";
import { Indicator } from "./indicator";

export interface SelectDto
{
  indicators: Indicator[];
  candidates: Candidate[];
}
