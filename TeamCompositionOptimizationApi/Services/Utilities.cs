using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TeamCompositionOptimizationApi.Models.Database;
using TeamCompositionOptimizationApi.Models.DTOs;
using TeamCompositionOptimizationApi.Models.Optimization;

namespace TeamCompositionOptimizationApi.Services
{
    public class Utilities
    {

        public static string HashPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = SHA256.HashData(passwordBytes);
            return Convert.ToBase64String(hashedBytes);
        }

        public static OptimizationResult Optimize(List<Indicator> indicators, List<Candidate> candidates, double threshold, double budget, double laboriousness, double time, List<Competency> competencies)
        {
            List<OptIndicator> optIndicators = new List<OptIndicator>();
            for (int i = 0; i < competencies.Count; i++)
            {
                Indicator indicator = indicators.Where(x => x.CompetencyId == competencies[i].Id).First();
                OptIndicator optIndicator = new OptIndicator()
                {
                    Value = indicator.Value,
                    Deviation = indicator.Deviation,
                    Weight = indicator.Weight
                };
                optIndicators.Add(optIndicator);
            }
            List<OptCandidate> optCandidates = new List<OptCandidate>();
            for (int i = 0; i < candidates.Count; i++)
            {
                Candidate candidate = candidates[i];
                OptCandidate optCandidate = new OptCandidate()
                {
                    Id = candidate.Id,
                    Competencies = [],
                    WorkingTime = candidate.WorkingTime,
                    Salary = candidate.Salary
                };
                for (int j = 0; j < competencies.Count; j++)
                {
                    CandidateCompetency? candidateCompetency = candidate.Competencies.Where(x => x.CompetencyId == competencies[j].Id).FirstOrDefault();
                    FuzzyValue fuzzyValue;
                    if (candidateCompetency == null)
                    {
                        fuzzyValue = new()
                        {
                            Value = 0,
                            DeviationLeft = 0,
                            DeviationRight = 0
                        };
                    }
                    else
                    {
                        fuzzyValue = new()
                        {
                            Value = candidateCompetency.Value,
                            DeviationLeft = candidateCompetency.DeviationLeft,
                            DeviationRight = candidateCompetency.DeviationRight
                        };
                    }
                    optCandidate.Competencies.Add(fuzzyValue);
                }
                optCandidates.Add(optCandidate);
            }

            List<OptTeamOption> optTeamOptions = Optimize(optIndicators, optCandidates, threshold, budget, laboriousness, time);
            List<TeamOption> teamOptions = [];
            for (int i = 0; i < optTeamOptions.Count; i++)
            {
                OptTeamOption optTeamOption = optTeamOptions[i];
                TeamOption teamOption = new()
                {
                    Criteria1 = optTeamOption.Criteria1,
                    Criteria2 = optTeamOption.Criteria2,
                    Cost = optTeamOption.Cost,
                    TeamworkTime = optTeamOption.TeamworkTime,
                    Candidates = [],
                    MaxCompetencies = []
                };
                for (int j = 0; j < optTeamOption.Candidates.Count; j++)
                {
                    OptCandidate optCandidate = optTeamOption.Candidates[j];
                    Candidate candidate = candidates.Where(x => x.Id == optCandidate.Id).First();
                    teamOption.Candidates.Add(candidate);
                }
                for (int j = 0; j < competencies.Count; j++)
                {
                    OptCompliance optCompliance = optTeamOption.MaxCompetencies[j];
                    Compliance compliance = new()
                    {
                        Competency = competencies[j],
                        Value = optCompliance.Value,
                        Membership = optCompliance.Membership
                    };
                    teamOption.MaxCompetencies.Add(compliance);
                }
                teamOptions.Add(teamOption);
            }


            OptimizationResult optimizationResult = new()
            {
                TeamOptions = teamOptions,
                Indicators = indicators,
                Candidates = candidates,
                Threshold = threshold,
                Budget = budget,
                Laboriousness = laboriousness,
                Time = time
            };


            return optimizationResult;
        }

        public static List<GeneralizedCompetence> Select(List<Indicator> indicators, List<Candidate> candidates, List<Competency> competencies)
        {
            List<OptIndicator> optIndicators = [];
            for (int i = 0; i < competencies.Count; i++)
            {
                Indicator indicator = indicators.Where(x => x.CompetencyId == competencies[i].Id).First();
                OptIndicator optIndicator = new OptIndicator()
                {
                    Value = indicator.Value,
                    Deviation = indicator.Deviation,
                    Weight = indicator.Weight
                };
                optIndicators.Add(optIndicator);
            }
            List<OptCandidate> optCandidates = new List<OptCandidate>();
            for (int i = 0; i < candidates.Count; i++)
            {
                Candidate candidate = candidates[i];
                OptCandidate optCandidate = new OptCandidate()
                {
                    Id = candidate.Id,
                    Competencies = [],
                    WorkingTime = candidate.WorkingTime,
                    Salary = candidate.Salary
                };
                for (int j = 0; j < competencies.Count; j++)
                {
                    CandidateCompetency? candidateCompetency = candidate.Competencies.Where(x => x.CompetencyId == competencies[j].Id).FirstOrDefault();
                    FuzzyValue fuzzyValue;
                    if (candidateCompetency == null)
                    {
                        fuzzyValue = new()
                        {
                            Value = 0,
                            DeviationLeft = 0,
                            DeviationRight = 0
                        };
                    }
                    else
                    {
                        fuzzyValue = new()
                        {
                            Value = candidateCompetency.Value,
                            DeviationLeft = candidateCompetency.DeviationLeft,
                            DeviationRight = candidateCompetency.DeviationRight
                        };
                    }
                    optCandidate.Competencies.Add(fuzzyValue);
                }
                optCandidates.Add(optCandidate);
            }

            List<GeneralizedCompetence> GeneralizedCompetencies = Select(optIndicators, optCandidates);
            GeneralizedCompetencies = GeneralizedCompetencies.OrderByDescending(x => x.Competence).ToList();

            return GeneralizedCompetencies;
        }


        public static (List<Indicator> indicators, List<Candidate> candidates, List<Competency> competencies) CopyIndicatorsCandidatesCompetencies(List<Indicator> indicators, List<Candidate> candidates, List<Competency> competencies)
        {
            List<(Competency original, Competency copy)> competencyCopy = competencies
                .Select(competency => (competency, new Competency() { Name = competency.Name }))
                .ToList();

            List<Candidate> candidateCopies = candidates.Select(candidate => new Candidate()
            {
                Name = candidate.Name,
                WorkingTime = candidate.WorkingTime,
                Salary = candidate.Salary,
                Competencies = candidate.Competencies.Where(candidateCompetency => competencyCopy.Select(x => x.original.Id).Contains(candidateCompetency.Competency.Id)).Select(candidateCompetency => new CandidateCompetency()
                {
                    Value = candidateCompetency.Value,
                    DeviationLeft = candidateCompetency.DeviationLeft,
                    DeviationRight = candidateCompetency.DeviationRight,
                    Competency = competencyCopy.Where(x => x.original.Id == candidateCompetency.Competency.Id).First().copy,
                }).ToList()
            }).ToList();

            List<Indicator> indicatorCopies = indicators.Select(indicator => new Indicator()
            {
                Competency = competencyCopy.Where(x => x.original.Id == indicator.Competency.Id).First().copy,
                Value = indicator.Value,
                Deviation = indicator.Deviation,
                Weight = indicator.Weight
            }).ToList();

            return (indicatorCopies, candidateCopies, competencyCopy.Select(x=>x.copy).ToList());
        }



        static IEnumerable<List<int>> Combination(int n, int k)
        {
            if (k > n || n < 1 || k < 1)
            {
                yield break;
            }
            else if (k == 1)
            {
                for (int i = 0; i < n; i++)
                {
                    yield return new List<int>() { i };
                }
                yield break;
            }

            int current = k - 1;
            List<int> currentCombination = Enumerable.Range(0, k).ToList();

            while (true)
            {
                yield return currentCombination.ToList();
                if (currentCombination[current] < n - 1)
                {
                    currentCombination[current]++;
                }
                else
                {
                    do
                    {
                        current--;
                    }
                    while (current > 0 && currentCombination[current] == currentCombination[current + 1] - 1);
                    if (current == 0 && currentCombination[current] == currentCombination[current + 1] - 1)
                    {
                        yield break;
                    }
                    else
                    {
                        currentCombination[current]++;
                        do
                        {
                            current++;
                            currentCombination[current] = currentCombination[current - 1] + 1;
                        }
                        while (current < k - 1);
                    }
                }
            }


        }

        static IEnumerable<List<int>> CombinationsBreadth(int count, int maxCount)
        {
            for (int i = 0; i < maxCount; i++)
            {
                foreach (var item in Combination(count, i + 1))
                {
                    yield return item;
                }
            }
            yield break;
        }


        static List<OptTeamOption> Optimize(List<OptIndicator> indicators, List<OptCandidate> candidates, double threshold, double budget, double laboriousness, double time)
        {
            int indicatorsCount = indicators.Count;
            int candidatesCount = candidates.Count;

            List<List<OptCompliance>> compliances = new(candidatesCount);
            for (int i = 0; i < candidatesCount; i++)
            {
                List<OptCompliance> tempCompliance = new(indicatorsCount);
                for (int j = 0; j < indicatorsCount; j++)
                {
                    if (candidates[i].Competencies[j].Value < indicators[j].Value - indicators[j].Deviation)
                    {
                        tempCompliance.Add(new() { Value = 0d, Membership = 0d });
                    }
                    else if (candidates[i].Competencies[j].Value >= indicators[j].Value)
                    {
                        tempCompliance.Add(new() { Value = candidates[i].Competencies[j].Value, Membership = 1d });
                    }
                    else
                    {
                        double q = indicators[j].Value;
                        double dq = indicators[j].Deviation;
                        double c = candidates[i].Competencies[j].Value;
                        double dc = candidates[i].Competencies[j].DeviationRight;

                        double compliance = ((q * dc) + (dq * c)) / (dc + dq);
                        double membership = (c + dc - compliance) / dc;

                        tempCompliance.Add(new() { Value = compliance, Membership = membership });
                    }
                }
                compliances.Add(tempCompliance);
            }

            List<OptCandidate> filteredCandidates = new();
            List<List<OptCompliance>> filteredCompliances = new();
            for (int i = 0; i < candidatesCount; i++)
            {
                if (compliances[i].Max(x => x.Membership) >= threshold)
                {
                    filteredCandidates.Add(candidates[i]);
                    filteredCompliances.Add(compliances[i]);
                }
            }

            List<OptTeamOption> TeamOptions = new();

            foreach (var chosen in CombinationsBreadth(filteredCandidates.Count, filteredCandidates.Count))
            {
                OptTeamOption tempTeamOption = new();
                tempTeamOption.MaxCompetencies = new(indicatorsCount);
                for (int i = 0; i < indicatorsCount; i++)
                {
                    tempTeamOption.MaxCompetencies.Add(new() { Value = 0, Membership = 0 });
                }

                for (int i = 0; i < chosen.Count; i++)
                {
                    for (int j = 0; j < indicatorsCount; j++)
                    {
                        if (filteredCompliances[chosen[i]][j].Value > tempTeamOption.MaxCompetencies[j].Value)
                        {
                            tempTeamOption.MaxCompetencies[j] = filteredCompliances[chosen[i]][j];
                        }
                        tempTeamOption.Criteria2 += filteredCompliances[chosen[i]][j].Value * indicators[j].Weight;
                    }
                    tempTeamOption.Cost += filteredCandidates[chosen[i]].Salary * time * filteredCandidates[chosen[i]].WorkingTime;
                    tempTeamOption.TeamworkTime += time * filteredCandidates[chosen[i]].WorkingTime;

                    tempTeamOption.Candidates.Add(filteredCandidates[chosen[i]]);
                }

                tempTeamOption.Criteria1 = tempTeamOption.MaxCompetencies.Sum(x => x.Value);

                if (tempTeamOption.MaxCompetencies.Min(x => x.Membership) >= threshold)
                {
                    if (tempTeamOption.Cost <= budget)
                    {
                        if (tempTeamOption.TeamworkTime >= laboriousness)
                        {
                            TeamOptions.Add(tempTeamOption);
                        }
                    }
                }
            }

            TeamOptions = TeamOptions.OrderByDescending(x => x.Criteria1).ToList();

            return TeamOptions;
        }

        public static List<GeneralizedCompetence> Select(List<OptIndicator> indicators, List<OptCandidate> candidates, int number = 0)
        {
            List<OptCandidate> priorities = new(candidates.Count);
            List<(double value, double deviationLeft, double deviationRight)> sums = new List<(double value, double deviationLeft, double deviationRight)>(indicators.Count);

            for (int i = 0; i < indicators.Count; i++)
            {
                double value = 0;
                double deviationLeft = 0;
                double deviationRight = 0;

                for (int j = 0; j < candidates.Count; j++)
                {
                    value += candidates[j].Competencies[i].Value;
                    deviationLeft += /*candidates[j].competencies[i].value - */candidates[j].Competencies[i].DeviationLeft;
                    deviationRight += /*candidates[j].competencies[i].value + */candidates[j].Competencies[i].DeviationRight;
                }

                sums.Add((value, deviationLeft, deviationRight));
            }

            for (int j = 0; j < candidates.Count; j++)
            {
                OptCandidate tempPriority = new();

                for (int i = 0; i < indicators.Count; i++)
                {
                    double value = (candidates[j].Competencies[i].Value / sums[i].value);
                    double deviationLeft = (candidates[j].Competencies[i].Value * sums[i].deviationRight + (/*candidates[j].competencies[i].value + */candidates[j].Competencies[i].DeviationLeft) * sums[i].value) / (Math.Pow(sums[i].value, 2));
                    double deviationRight = (candidates[j].Competencies[i].Value * sums[i].deviationLeft + (/*candidates[j].competencies[i].value + */candidates[j].Competencies[i].DeviationRight) * sums[i].value) / (Math.Pow(sums[i].value, 2));

                    tempPriority.Competencies.Add(new FuzzyValue() { Value = double.IsNaN( value)?0: value, DeviationLeft = double.IsNaN(deviationLeft) ? 0 : deviationLeft, DeviationRight = double.IsNaN(deviationRight) ? 0 : deviationRight });
                }

                priorities.Add(tempPriority);
            }

            List<(double value, double deviationLeft, double deviationRight)> generalizedCompetence = new();
            List<GeneralizedCompetence> defazified = new(candidates.Count);

            for (int i = 0; i < candidates.Count; i++)
            {
                double value = 0;
                double deviationLeft = 0;
                double deviationRight = 0;

                for (int j = 0; j < indicators.Count; j++)
                {
                    value += indicators[j].Weight * priorities[i].Competencies[j].Value;
                    deviationLeft += indicators[j].Weight * priorities[i].Competencies[j].DeviationLeft;
                    deviationRight += indicators[j].Weight * priorities[i].Competencies[j].DeviationRight;
                }

                generalizedCompetence.Add((value, deviationLeft, deviationRight));
                defazified.Add(new GeneralizedCompetence()
                {
                    Id = candidates[i].Id,
                    Competence = (value * 3 - deviationLeft + deviationRight) / 3
                });
            }

            return defazified;
        }

        public static List<TeamOption> Sort(List<TeamOption> teams)
        {
            List<(double criteria1, double criteria2, double cost)> normalized = new List<(double criteria1, double criteria2, double cost)>(teams.Count);
            List<(int index, double value)> singleCriteria = new List<(int, double)>();
            double max1 = double.NegativeInfinity;
            double max2 = double.NegativeInfinity;
            double max3 = double.NegativeInfinity;
            double min1 = double.PositiveInfinity;
            double min2 = double.PositiveInfinity;
            double min3 = double.PositiveInfinity;

            for (int i = 0; i < teams.Count; i++)
            {
                if (max1 < teams[i].Criteria1)
                {
                    max1 = teams[i].Criteria1;
                }
                if (max2 < teams[i].Criteria2)
                {
                    max2 = teams[i].Criteria2;
                }
                if (max3 < teams[i].Cost)
                {
                    max3 = teams[i].Cost;
                }

                if (min1 > teams[i].Criteria1)
                {
                    min1 = teams[i].Criteria1;
                }
                if (min2 > teams[i].Criteria2)
                {
                    min2 = teams[i].Criteria2;
                }
                if (min3 > teams[i].Cost)
                {
                    min3 = teams[i].Cost;
                }
            }

            for (int i = 0; i < teams.Count; i++)
            {
                double criteria1 = 0;
                double criteria2 = 0;
                double cost = 0;
                if (max1 != min1)
                {
                    criteria1 = (teams[i].Criteria1 - min1) / (max1 - min1);
                }
                if (max2 != min2)
                {
                    criteria2 = (teams[i].Criteria2 - min2) / (max2 - min2);
                }
                if (max3 != min3)
                {
                    cost = 1 - ((teams[i].Cost - min3) / (max3 - min3));
                }

                normalized.Add((criteria1, criteria2, cost));
                singleCriteria.Add((i, criteria1 + criteria2 + cost));
            }

            singleCriteria = singleCriteria.OrderByDescending(x => x.value).ToList();

            List<TeamOption> result = new List<TeamOption>();
            for (int i = 0; i < teams.Count; i++)
            {
                result.Add(teams[singleCriteria[i].index]);
            }

            return result;
        }

    }
}
