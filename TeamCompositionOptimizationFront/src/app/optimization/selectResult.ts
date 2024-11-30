import { Candidate } from "./candidate";
import { GeneralizedCompetence } from "./generalizedCompetence";
import { Indicator } from "./indicator";
import { TeamOption } from "./teamOption";

export interface selectResult
{
  generalizedCompetences: GeneralizedCompetence[];
  candidates: Candidate[];
  indicators: Indicator[];
}
