import { Competency } from "./competency";

export interface Indicator
{
  id: number;
  competency: Competency;
  competencyId: number;
  value: number;
  deviation: number;
  weight: number;
}
