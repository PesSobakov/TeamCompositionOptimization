using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeamCompositionOptimizationApi.Models.Database;
using TeamCompositionOptimizationApi.Models.DTOs;
using TeamCompositionOptimizationApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;
using Task_Board_API.Models.ErrorResponses;
using TeamCompositionOptimizationApi.Models.Optimization;

namespace TeamCompositionOptimizationApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [EnableCors("OpenCORSPolicy")]
    public class OptimizationController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;

        public OptimizationController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        // GET: api/optimization/candidates
        [HttpGet]
        [ActionName("candidates")]
        public async Task<IActionResult> GetCandidate()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Include(x => x.Candidates).ThenInclude(x => x.Competencies).ThenInclude(x => x.Competency)
                .Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }


            List<Candidate> candidates = user.Candidates;

            if (candidates.Count == 0)
            {
                return NoContent();
            }

            List<CandidateDto> candidateDtos = candidates.Select(x => new CandidateDto()
            {
                Id = x.Id,
                Name = x.Name,
                WorkingTime = x.WorkingTime,
                Salary = x.Salary,
                Competencies = x.Competencies.Select(x => new CandidateCompetencyDto()
                {
                    Id = x.Id,
                    Value = x.Value,
                    DeviationLeft = x.DeviationLeft,
                    DeviationRight = x.DeviationRight,
                    CandidateId = x.CandidateId,
                    CompetencyId = x.CompetencyId
                }).ToList()
            }).ToList();

            return Ok(candidateDtos);
        }

        // GET api/optimization/candidates/5
        [HttpGet("{id}")]
        [ActionName("candidates")]
        public async Task<IActionResult> GetCandidate(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users.Include(x => x.Candidates).ThenInclude(x => x.Competencies).Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }


            Candidate? candidate = user.Candidates.Where(x => x.Id == id).FirstOrDefault();

            if (candidate == null)
            {
                return NotFound();
            }

            CandidateDto candidateDto = new CandidateDto()
            {
                Id = candidate.Id,
                Name = candidate.Name,
                WorkingTime = candidate.WorkingTime,
                Salary = candidate.Salary,
                Competencies = candidate.Competencies.Select(x => new CandidateCompetencyDto()
                {
                    Id = x.Id,
                    Value = x.Value,
                    DeviationLeft = x.DeviationLeft,
                    DeviationRight = x.DeviationRight,
                    CandidateId = x.CandidateId,
                    CompetencyId = x.CompetencyId
                }).ToList()
            };

            return Ok(candidateDto);
        }

        // POST api/optimization/candidates
        [HttpPost]
        [ActionName("candidates")]
        public async Task<IActionResult> PostCandidate([FromBody] CandidateDto candidateDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users.Include(x => x.Candidates).ThenInclude(x => x.Competencies).Include(x => x.Competencies).Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            List<Competency> competencies = user.Competencies;

            List<CandidateCompetency> candidateCompetencies = [];

            for (int i = 0; i < candidateDto.Competencies.Count; i++)
            {
                Competency? competency = competencies.Where(x => x.Id == candidateDto.Competencies[i].CompetencyId).FirstOrDefault();
                if (competency == null)
                {
                    return NotFound();
                }
                CandidateCompetency candidateCompetency = new CandidateCompetency
                {
                    Value = candidateDto.Competencies[i].Value,
                    DeviationLeft = candidateDto.Competencies[i].DeviationLeft,
                    DeviationRight = candidateDto.Competencies[i].DeviationRight,
                    Competency = competency
                };
                candidateCompetencies.Add(candidateCompetency);
            }

            Candidate candidate = new()
            {
                Name = candidateDto.Name,
                WorkingTime = candidateDto.Salary,
                Salary = candidateDto.Salary,
                Competencies = candidateCompetencies
            };
            user.Candidates.Add(candidate);
            await _databaseContext.SaveChangesAsync();
            return Ok();
        }

        // PATCH api/optimization/candidates/5
        [HttpPatch("{id}")]
        [ActionName("candidates")]
        public async Task<IActionResult> PatchCandidate(int id, [FromBody] CandidateDto candidateDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Include(x => x.Candidates).ThenInclude(x => x.Competencies).ThenInclude(x => x.Competency)
                .Include(x => x.Competencies).Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            Candidate? candidate = user.Candidates.Where(x => x.Id == id).FirstOrDefault();

            if (candidate == null)
            {
                return NotFound();
            }

            List<int> candidateCompetencyIds = candidateDto.Competencies.Select(x => x.Id).ToList();

            candidate.Competencies.RemoveAll((x) =>
            {
                return !candidateCompetencyIds.Contains(x.Id);
            });

            List<Competency> competencies = user.Competencies;

            foreach (var item in candidateDto.Competencies)
            {
                CandidateCompetency? candidateCompetency = candidate.Competencies.Where(x => x.Id == item.Id).FirstOrDefault();
                if (candidateCompetency == null)
                {
                    Competency? competency = competencies.Where(x => x.Id == item.CompetencyId).FirstOrDefault();
                    if (competency == null)
                    {
                        return NotFound();
                    }
                    candidateCompetency = new CandidateCompetency
                    {
                        Value = item.Value,
                        DeviationLeft = item.DeviationLeft,
                        DeviationRight = item.DeviationRight,
                        Competency = competency
                    };
                    candidate.Competencies.Add(candidateCompetency);
                }
                else
                {
                    Competency? competency = competencies.Where(x => x.Id == item.CompetencyId).FirstOrDefault();
                    if (competency == null)
                    {
                        return NotFound();
                    }
                    candidateCompetency.Value = item.Value;
                    candidateCompetency.DeviationLeft = item.DeviationLeft;
                    candidateCompetency.DeviationRight = item.DeviationRight;
                    candidateCompetency.Competency = competency;
                }
            }

            candidate.Name = candidateDto.Name;
            candidate.WorkingTime = candidateDto.WorkingTime;
            candidate.Salary = candidateDto.Salary;

            await _databaseContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/optimization/candidates/5
        [HttpDelete("{id}")]
        [ActionName("candidates")]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Include(x => x.Candidates).ThenInclude(x => x.Competencies)
                .Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            Candidate? candidate = user.Candidates.Where(x => x.Id == id).FirstOrDefault();
            if (candidate == null)
            {
                return NotFound();
            }

            //user.Candidates.Remove(candidate);
            _databaseContext.Candidates.Remove(candidate);
            await _databaseContext.SaveChangesAsync();
            return Ok();
        }



        // GET: api/optimization/competencies
        [HttpGet]
        [ActionName("competencies")]
        public async Task<IActionResult> GetCompetency()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users.Include(x => x.Competencies).Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            List<Competency> competencies = user.Competencies;

            if (competencies.Count == 0)
            {
                return NoContent();
            }

            List<CompetencyDto> competencyDtos = competencies.Select(x => new CompetencyDto()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return Ok(competencyDtos);
        }

        // GET api/optimization/competencies/5
        [HttpGet("{id}")]
        [ActionName("competencies")]
        public async Task<IActionResult> GetCompetency(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users.Include(x => x.Competencies).Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            Competency? competency = user.Competencies.Where(x => x.Id == id).FirstOrDefault();

            if (competency == null)
            {
                return NotFound();
            }

            CompetencyDto competencyDto = new()
            {
                Id = competency.Id,
                Name = competency.Name
            };

            return Ok(competencyDto);
        }

        // POST api/optimization/competencies
        [HttpPost]
        [ActionName("competencies")]
        public async Task<IActionResult> PostCompetency([FromBody] CompetencyDto competencyDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users.Include(x => x.Competencies).Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            Competency competency = new()
            {
                Name = competencyDto.Name
            };
            user.Competencies.Add(competency);
            await _databaseContext.SaveChangesAsync();
            return Ok();
        }

        // PATCH api/optimization/competencies/5
        [HttpPatch("{id}")]
        [ActionName("competencies")]
        public async Task<IActionResult> PatchCompetency(int id, [FromBody] CompetencyDto competencyDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users.Include(x => x.Competencies).Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            Competency? competency = user.Competencies.Where(x => x.Id == id).FirstOrDefault();

            if (competency == null)
            {
                return NotFound();
            }

            competency.Name = competencyDto.Name;

            await _databaseContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/optimization/competencies/5
        [HttpDelete("{id}")]
        [ActionName("competencies")]
        public async Task<IActionResult> DeleteCompetency(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Include(x => x.Competencies).ThenInclude(x => x.CandidateCompetencies).ThenInclude(x => x.Candidate)
                .Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            Competency? competency = user.Competencies.Where(x => x.Id == id).FirstOrDefault();
            if (competency == null)
            {
                return NotFound();
            }

            _databaseContext.Competencies.Remove(competency);
            //user.Competencies.Remove(competency);
            await _databaseContext.SaveChangesAsync();
            return Ok();
        }



        // POST api/optimization/optimize
        [HttpPost]
        [ActionName("optimize")]
        public async Task<IActionResult> Optimize([FromBody] OptimizationDto optimizationDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Include(x => x.Subscription)
                .Include(x => x.Candidates).ThenInclude(x => x.Competencies).ThenInclude(x => x.Competency)
                .Include(x => x.Competencies)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.Indicators).ThenInclude(x => x.Competency)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.Candidates).ThenInclude(x => x.Competencies).ThenInclude(x => x.Competency)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.TeamOptions).ThenInclude(x => x.Candidates).ThenInclude(x => x.Competencies).ThenInclude(x => x.Competency)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.TeamOptions).ThenInclude(x => x.MaxCompetencies)
                .Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            if (optimizationDto.Candidates.Count > user.Subscription.MaxCandidatesCount)
            {
                Dictionary<string, string> errors = new()
                {
                    { "message", "candidates limit exceeded" }
                };
                return BadRequest(new ValidationError(errors));
            }

            if (optimizationDto.Indicators.Count > user.Subscription.MaxCompetenciesCount)
            {
                Dictionary<string, string> errors = new()
                {
                    { "message", "competencies limit exceeded" }
                };
                return BadRequest(new ValidationError(errors));
            }

            List<Competency> allCompetencies = user.Competencies;

            List<Indicator> indicators = [];
            List<Competency> competencies = [];
            double weightSum = optimizationDto.Indicators.Sum(x => x.Weight);
            if (weightSum == 0)
            {
                weightSum = 1;
            }
            foreach (var item in optimizationDto.Indicators)
            {
                Competency? competency = allCompetencies.Where(x => x.Id == item.CompetencyId).FirstOrDefault();
                if (competency == null)
                {
                    return NotFound();
                }
                competencies.Add(competency);
                Indicator indicator = new()
                {
                    CompetencyId = competency.Id,
                    Competency = competency,
                    Value = item.Value,
                    Deviation = item.Deviation,
                    Weight = item.Weight / weightSum
                };
                indicators.Add(indicator);
            }

            List<Candidate> candidates = [];
            foreach (var item in optimizationDto.Candidates)
            {
                Candidate? candidate = user.Candidates.Where(x => x.Id == item.Id).FirstOrDefault();
                if (candidate == null)
                {
                    return NotFound();
                }
                candidates.Add(candidate);
            }

            (var optimizationIndicators, var optimizationCandidates, var optimizationCompetencies) = Utilities.CopyIndicatorsCandidatesCompetencies(indicators, candidates, competencies);

            await _databaseContext.Indicators.AddRangeAsync(optimizationIndicators);
            await _databaseContext.Candidates.AddRangeAsync(optimizationCandidates);
            await _databaseContext.Competencies.AddRangeAsync(optimizationCompetencies);
            await _databaseContext.SaveChangesAsync();

            OptimizationResult optimizationResult = Utilities.Optimize(optimizationIndicators, optimizationCandidates, optimizationDto.Threshold, optimizationDto.Budget, optimizationDto.Laboriousness, optimizationDto.Time, optimizationCompetencies);

            user.OptimizationResults.Add(optimizationResult);
            await _databaseContext.SaveChangesAsync();

            OptimizationResultDto optimizationResuzltDto = new()
            {
                TeamOptions = optimizationResult.TeamOptions.Select(x => new TeamOptionDto
                {
                    Criteria1 = x.Criteria1,
                    Criteria2 = x.Criteria2,
                    Cost = x.Cost,
                    TeamworkTime = x.TeamworkTime,
                    MaxCompetencies = x.MaxCompetencies.Select(y => new ComplianceDto()
                    {
                        CompetencyId = y.CompetencyId,
                        Value = y.Value,
                        Membership = y.Membership
                    }).ToList(),
                    Candidates = x.Candidates.Select(y => new CandidateDto()
                    {
                        Id = y.Id,
                        Name = y.Name,
                        WorkingTime = y.WorkingTime,
                        Salary = y.Salary,
                        Competencies = y.Competencies.Where(competency => competencies.Select(comp => comp.Id).Contains(competency.Competency.Id)).Select(z => new CandidateCompetencyDto()
                        {
                            Id = z.Id,
                            Value = z.Value,
                            DeviationLeft = z.DeviationLeft,
                            DeviationRight = z.DeviationRight,
                            CandidateId = z.CandidateId,
                            CompetencyId = z.CompetencyId
                        }).ToList()
                    }).ToList()
                }).ToList(),
                Candidates = optimizationResult.Candidates.Select(y => new CandidateDto()
                {
                    Id = y.Id,
                    Name = y.Name,
                    WorkingTime = y.WorkingTime,
                    Salary = y.Salary,
                    Competencies = y.Competencies.Where(competency => competencies.Select(comp => comp.Id).Contains(competency.Competency.Id)).Select(z => new CandidateCompetencyDto()
                    {
                        Id = z.Id,
                        Value = z.Value,
                        DeviationLeft = z.DeviationLeft,
                        DeviationRight = z.DeviationRight,
                        CandidateId = z.CandidateId,
                        CompetencyId = z.CompetencyId
                    }).ToList()
                }).ToList(),
                Indicators = optimizationResult.Indicators.Select(x => new IndicatorDto()
                {
                    Competency = new CompetencyDto()
                    {
                        Id = x.Competency.Id,
                        Name = x.Competency.Name
                    },
                    CompetencyId = x.CompetencyId,
                    Value = x.Value,
                    Deviation = x.Deviation,
                    Weight = x.Weight
                }).ToList(),
                Threshold = optimizationResult.Threshold,
                Budget = optimizationResult.Budget,
                Laboriousness = optimizationResult.Laboriousness,
                Time = optimizationResult.Time
            };

            return Ok(optimizationResuzltDto);
        }


        // POST api/optimization/select
        [HttpPost]
        [ActionName("select")]
        public async Task<IActionResult> Select([FromBody] SelectDto selectDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Include(x => x.Subscription)
                .Include(x => x.Candidates).ThenInclude(x => x.Competencies).ThenInclude(x => x.Competency)
                .Include(x => x.Competencies)
                .Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            List<Competency> allCompetencies = user.Competencies;

            List<Indicator> indicators = [];
            List<Competency> competencies = [];
            double weightSum = selectDto.Indicators.Sum(x => x.Weight);
            if (weightSum == 0)
            {
                weightSum = 1;
            }
            foreach (var item in selectDto.Indicators)
            {
                Competency? competency = allCompetencies.Where(x => x.Id == item.CompetencyId).FirstOrDefault();
                if (competency == null)
                {
                    return NotFound();
                }
                competencies.Add(competency);
                Indicator indicator = new()
                {
                    CompetencyId = competency.Id,
                    Competency = competency,
                    Value = item.Value,
                    Deviation = item.Deviation,
                    Weight = item.Weight / weightSum
                };
                indicators.Add(indicator);
            }

            List<Candidate> candidates = [];
            foreach (var item in selectDto.Candidates)
            {
                Candidate? candidate = user.Candidates.Where(x => x.Id == item.Id).FirstOrDefault();
                if (candidate == null)
                {
                    return NotFound();
                }
                candidates.Add(candidate);
            }
            SelectResultDto selectResult = new()
            {
                Indicators = indicators.Select(x => new IndicatorDto()
                {
                    Competency = new CompetencyDto()
                    {
                        Id = x.Competency.Id,
                        Name = x.Competency.Name
                    },
                    CompetencyId = x.CompetencyId,
                    Value = x.Value,
                    Deviation = x.Deviation,
                    Weight = x.Weight
                }).ToList(),
                Candidates = candidates.Select(y => new CandidateDto()
                {
                    Id = y.Id,
                    Name = y.Name,
                    WorkingTime = y.WorkingTime,
                    Salary = y.Salary,
                    Competencies = y.Competencies.Where(competency => competencies.Select(comp => comp.Id).Contains(competency.Competency.Id)).Select(z => new CandidateCompetencyDto()
                    {
                        Id = z.Id,
                        Value = z.Value,
                        DeviationLeft = z.DeviationLeft,
                        DeviationRight = z.DeviationRight,
                        CandidateId = z.CandidateId,
                        CompetencyId = z.CompetencyId
                    }).ToList()
                }).ToList(),
                GeneralizedCompetences = Utilities.Select(indicators, candidates, competencies)
            };

            return Ok(selectResult);
        }


        // GET: api/optimization/result
        [HttpGet]
        [ActionName("result")]
        public async Task<IActionResult> GetResult()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Include(x => x.Candidates).ThenInclude(x => x.Competencies)
                .Include(x => x.Competencies)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.Indicators)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.Indicators).ThenInclude(x => x.Competency)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.Candidates).ThenInclude(x => x.Competencies)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.Candidates).ThenInclude(x => x.Competencies).ThenInclude(x => x.Competency)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.TeamOptions).ThenInclude(x => x.Candidates).ThenInclude(x => x.Competencies)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.TeamOptions).ThenInclude(x => x.Candidates).ThenInclude(x => x.Competencies).ThenInclude(x => x.Competency)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.TeamOptions).ThenInclude(x => x.MaxCompetencies)
                .Where(x => x.Login == login)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            List<OptimizationResult> results = user.OptimizationResults;

            if (results.Count == 0)
            {
                return NoContent();
            }

            List<OptimizationResultDto> resuzltDtos = results.Select(optimizationResult => new OptimizationResultDto()
            {
                Id = optimizationResult.Id,
                TeamOptions = optimizationResult.TeamOptions.Select(x => new TeamOptionDto
                {
                    Criteria1 = x.Criteria1,
                    Criteria2 = x.Criteria2,
                    Cost = x.Cost,
                    TeamworkTime = x.TeamworkTime,
                    MaxCompetencies = x.MaxCompetencies.Select(y => new ComplianceDto()
                    {
                        CompetencyId = y.CompetencyId,
                        Value = y.Value,
                        Membership = y.Membership
                    }).ToList(),
                    Candidates = x.Candidates.Select(y => new CandidateDto()
                    {
                        Id = y.Id,
                        Name = y.Name,
                        WorkingTime = y.WorkingTime,
                        Salary = y.Salary,
                        Competencies = y.Competencies.Where(competency => optimizationResult.Indicators.Select(indicator => indicator.Competency.Id).Contains(competency.Competency.Id)).Select(z => new CandidateCompetencyDto()
                        {
                            Id = z.Id,
                            Value = z.Value,
                            DeviationLeft = z.DeviationLeft,
                            DeviationRight = z.DeviationRight,
                            CandidateId = z.CandidateId,
                            CompetencyId = z.CompetencyId
                        }).ToList()
                    }).ToList()
                }).ToList(),
                Candidates = optimizationResult.Candidates.Select(y => new CandidateDto()
                {
                    Id = y.Id,
                    Name = y.Name,
                    WorkingTime = y.WorkingTime,
                    Salary = y.Salary,
                    Competencies = y.Competencies.Where(competency => optimizationResult.Indicators.Select(indicator => indicator.Competency.Id).Contains(competency.Competency.Id)).Select(z => new CandidateCompetencyDto()
                    {
                        Id = z.Id,
                        Value = z.Value,
                        DeviationLeft = z.DeviationLeft,
                        DeviationRight = z.DeviationRight,
                        CandidateId = z.CandidateId,
                        CompetencyId = z.CompetencyId
                    }).ToList()
                }).ToList(),
                Indicators = optimizationResult.Indicators.Select(x => new IndicatorDto()
                {
                    Competency = new CompetencyDto()
                    {
                        Id = x.Competency.Id,
                        Name = x.Competency.Name
                    },
                    CompetencyId = x.CompetencyId,
                    Value = x.Value,
                    Deviation = x.Deviation,
                    Weight = x.Weight
                }).ToList(),
                Threshold = optimizationResult.Threshold,
                Budget = optimizationResult.Budget,
                Laboriousness = optimizationResult.Laboriousness,
                Time = optimizationResult.Time
            })
            .ToList();

            return Ok(resuzltDtos);
        }

        // DELETE api/optimization/result/5
        [HttpDelete("{id}")]
        [ActionName("result")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Include(x => x.Candidates).ThenInclude(x => x.Competencies)
                .Include(x => x.Competencies)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.Indicators)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.Candidates).ThenInclude(x => x.Competencies)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.TeamOptions).ThenInclude(x => x.Candidates).ThenInclude(x => x.Competencies)
                .Include(x => x.OptimizationResults).ThenInclude(x => x.TeamOptions).ThenInclude(x => x.MaxCompetencies)
                .Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            OptimizationResult? result = user.OptimizationResults.Where(x => x.Id == id).FirstOrDefault();
            if (result == null)
            {
                return NotFound();
            }

            user.OptimizationResults.Remove(result);
            await _databaseContext.SaveChangesAsync();
            return Ok();
        }



        // GET: api/optimization/result
        [HttpGet]
        [ActionName("seed")]
        [AllowAnonymous]
        public async Task<IActionResult> SeedDatabase()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Database.EnsureCreated();

            _databaseContext.Users.RemoveRange(_databaseContext.Users);
            _databaseContext.Subscriptions.RemoveRange(_databaseContext.Subscriptions);
            _databaseContext.Competencies.RemoveRange(_databaseContext.Competencies);
            _databaseContext.Indicators.RemoveRange(_databaseContext.Indicators);
            _databaseContext.Candidates.RemoveRange(_databaseContext.Candidates);
            _databaseContext.CandidateCompetencies.RemoveRange(_databaseContext.CandidateCompetencies);
            _databaseContext.OptimizationResults.RemoveRange(_databaseContext.OptimizationResults);
            _databaseContext.Compliances.RemoveRange(_databaseContext.Compliances);
            _databaseContext.TeamOptions.RemoveRange(_databaseContext.TeamOptions);
            _databaseContext.HelpPages.RemoveRange(_databaseContext.HelpPages);

            List<Subscription> subscriptions =
            [
                new Subscription() { Name = "normal", MaxCandidatesCount=10,MaxCompetenciesCount=10},
                new Subscription() { Name = "premium", MaxCandidatesCount=20,MaxCompetenciesCount=20}
            ];

            List<User> users =
            [
                new User() { Login = "user@gmail.com", PasswordHash= Utilities.HashPassword("string"), Subscription =subscriptions[1], SubscriptionDueDate = DateTime.UtcNow+TimeSpan.FromDays(90)},
               new User() { Login = "admin", PasswordHash= Utilities.HashPassword("admin"), Subscription =subscriptions[0], SubscriptionDueDate = DateTime.UtcNow+TimeSpan.FromDays(90),IsSuperuser = true},
             ];


            List<Competency> competencies =
            [
                new Competency() { Name = ".NET"},
                new Competency() { Name = "JavaScript"},
                new Competency() { Name = "Angular"},
                new Competency() { Name = "SQL"},
                new Competency() { Name = "Azure"}
            ];

            List<Indicator> indicators =
            [
                new Indicator(){ Competency = competencies[0], Value=3,Deviation=1,Weight=0.2},
                new Indicator(){ Competency = competencies[1], Value=1,Deviation=0.3,Weight=0.2},
                new Indicator(){ Competency = competencies[2], Value=2,Deviation=0.5,Weight=0.2},
                new Indicator(){ Competency = competencies[3], Value=2,Deviation=0.5,Weight=0.2},
                new Indicator(){ Competency = competencies[4], Value=3,Deviation=1,Weight=0.2}
            ];

            List<Candidate> candidates =
            [
                new Candidate { Name ="John", WorkingTime = 30,Salary = 10,
                    Competencies =
                    [
                        new CandidateCompetency(){ Competency = competencies[0], Value = 1, DeviationLeft = 0, DeviationRight = 0},
                        new CandidateCompetency(){ Competency = competencies[1], Value = 1, DeviationLeft = 0, DeviationRight = 0},
                        new CandidateCompetency(){ Competency = competencies[2], Value = 2, DeviationLeft = 0, DeviationRight = 0},
                        new CandidateCompetency(){ Competency = competencies[3], Value = 2, DeviationLeft = 0, DeviationRight = 0},
                        new CandidateCompetency(){ Competency = competencies[4], Value = 4, DeviationLeft = 0, DeviationRight = 0}
                    ]},
                new Candidate { Name ="Bill", WorkingTime = 20,Salary = 15,
                    Competencies =
                    [
                        new CandidateCompetency(){ Competency = competencies[0], Value = 3, /*DeviationLeft = 1,*/ DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[1], Value = 1, /*DeviationLeft = 0.5,*/ DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[2], Value = 2, /*DeviationLeft = 0.5,*/ DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[3], Value = 2, /*DeviationLeft = 1,*/ DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[4], Value = 4, /*DeviationLeft = 1,*/ DeviationRight = 0}
                    ]},
                new Candidate { Name ="Paul", WorkingTime = 30,Salary = 8,
                    Competencies =
                    [
                        new CandidateCompetency(){ Competency = competencies[0], Value = 3, /*DeviationLeft = 0.5,*/ DeviationRight = 0.5},
                        new CandidateCompetency(){ Competency = competencies[1], Value = 1, /*DeviationLeft = 0.5,*/ DeviationRight = 0.5},
                        new CandidateCompetency(){ Competency = competencies[2], Value = 2, /*DeviationLeft = 1,*/ DeviationRight = 0.5},
                        new CandidateCompetency(){ Competency = competencies[3], Value = 2, /*DeviationLeft = 1,*/ DeviationRight = 0.5},
                        new CandidateCompetency(){ Competency = competencies[4], Value = 3, /*DeviationLeft = 0.4,*/ DeviationRight = 0.4}
                    ]},
            ];

            await _databaseContext.AddRangeAsync(indicators);
            users[0].Competencies.AddRange(competencies);
            users[0].Candidates.AddRange(candidates);
            await _databaseContext.Users.AddRangeAsync(users);
            await _databaseContext.SaveChangesAsync();

            OptimizationResult optimizationResult = Utilities.Optimize(indicators, candidates, 0.9, 5000, 250, 5, competencies);

            List<HelpPage> helpPages =
            [
                new HelpPage(){Name = "General information", Text = "The information system consists of the following pages:\r\n- Home page;\r\n- Login to the system;\r\n- Optimization;\r\n- Help;\r\n- Personal account;\r\nThe home page contains basic information about the information system.\r\nThe login page allows you to register, log in, log out, and delete your account.\r\nThe Optimization page allows you to rank candidates and optimize the composition of the project team.\r\nThe Help page contains help pages that explain how to use the information system.\r\nThe personal account allows you to view your login and current subscription, and change your subscription."},
                new HelpPage(){Name = "Login page", Text = "To log in or register, you need to enter your login and password and click the appropriate button. If you are logged in, the logout and delete buttons will be available."},
                new HelpPage(){Name = "Optimization page", Text = "To optimize the composition of the project team, you first need to select the competencies that the team should have. A competency has three parameters: the minimum compliance of candidates with this competency, the possible deviation from this value, and the weight.\r\nThen you need to add candidates. Candidates have a match for each competency, a possible upward deviation, the number of working hours per week, and a salary per hour.\r\nThe optimization has parameters such as the threshold value of the probability of a candidate's compliance with a competency, budget, labor costs for the project in man-hours, and project completion time in weeks.\r\nAfter selecting competencies, candidates, and optimization parameters, you can start optimization by clicking on the appropriate button.\r\nThe result of the optimization will be options for possible teams. The team options contain a list of candidates, project cost, man-hours spent on the project, maximum matching values for each competency, the sum of matching values for all competencies, and the sum of maximum values for each competency."},
                new HelpPage(){Name = "Help Pages", Text = "Help pages describe the information system and allow you to familiarize yourself with the main functions."},
                new HelpPage(){Name = "Account page", Text = "Account page allows you to see your login, current subscription, and its expiration date. Account page also displays available subscriptions and allows you to change your subscription."},
            ];


            users[0].OptimizationResults.Add(optimizationResult);
            await _databaseContext.HelpPages.AddRangeAsync(helpPages);
            await _databaseContext.SaveChangesAsync();

            return Ok();
        }

        // GET: api/optimization/createdatabase
        [HttpGet]
        [ActionName("createdatabase")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateDatabase()
        {
            _databaseContext.Database.EnsureCreated();
            return Ok();
        }

        // GET: api/optimization/createdatabase
        [HttpGet]
        [ActionName("deletedatabase")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteDatabase()
        {
            _databaseContext.Database.EnsureDeleted();
            return Ok();
        }

        // GET: api/optimization/recreate
        [HttpGet]
        [ActionName("recreate")]
        [AllowAnonymous]
        public async Task<IActionResult> RecreateDatabase()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Database.EnsureCreated();

            _databaseContext.Users.RemoveRange(_databaseContext.Users);
            _databaseContext.Subscriptions.RemoveRange(_databaseContext.Subscriptions);
            _databaseContext.Competencies.RemoveRange(_databaseContext.Competencies);
            _databaseContext.Indicators.RemoveRange(_databaseContext.Indicators);
            _databaseContext.Candidates.RemoveRange(_databaseContext.Candidates);
            _databaseContext.CandidateCompetencies.RemoveRange(_databaseContext.CandidateCompetencies);
            _databaseContext.OptimizationResults.RemoveRange(_databaseContext.OptimizationResults);
            _databaseContext.Compliances.RemoveRange(_databaseContext.Compliances);
            _databaseContext.TeamOptions.RemoveRange(_databaseContext.TeamOptions);
            _databaseContext.HelpPages.RemoveRange(_databaseContext.HelpPages);

            List<Subscription> subscriptions =
            [
                new Subscription() { Name = "normal", MaxCandidatesCount=10,MaxCompetenciesCount=10},
                new Subscription() { Name = "premium", MaxCandidatesCount=20,MaxCompetenciesCount=20}
            ];

            List<User> users =
            [
                new User() { Login = "user@gmail.com", PasswordHash= Utilities.HashPassword("string"), Subscription =subscriptions[1], SubscriptionDueDate = DateTime.UtcNow+TimeSpan.FromDays(90)},
               new User() { Login = "admin", PasswordHash= Utilities.HashPassword("admin"), Subscription =subscriptions[0], SubscriptionDueDate = DateTime.UtcNow+TimeSpan.FromDays(90),IsSuperuser = true},
             ];


            List<Competency> competencies =
            [
                new Competency() { Name = "Design patterns"},
                new Competency() { Name = "Nest.js"},
                new Competency() { Name = "OOP paradigm"},
                new Competency() { Name = "S.O.L.I.D Principles"},
                new Competency() { Name = "Functional testing"}
            ];

            List<Indicator> indicators =
            [
                new Indicator(){ Competency = competencies[0], Value=3,Deviation=1,Weight=0.2},
                new Indicator(){ Competency = competencies[1], Value=1,Deviation=0.3,Weight=0.2},
                new Indicator(){ Competency = competencies[2], Value=2,Deviation=0.5,Weight=0.2},
                new Indicator(){ Competency = competencies[3], Value=2,Deviation=0.5,Weight=0.2},
                new Indicator(){ Competency = competencies[4], Value=3,Deviation=1,Weight=0.2}
            ];

            List<Candidate> candidates =
            [
                new Candidate { Name ="Robert", WorkingTime = 30,Salary = 10,
                    Competencies =
                    [
                        new CandidateCompetency(){ Competency = competencies[0], Value = 4, DeviationLeft = 1, DeviationRight = 0},
                        new CandidateCompetency(){ Competency = competencies[1], Value = 1, DeviationLeft = 0.5, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[2], Value = 1, DeviationLeft = 0.5, DeviationRight = 0.5},
                        new CandidateCompetency(){ Competency = competencies[3], Value = 2, DeviationLeft = 1, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[4], Value = 0, DeviationLeft = 0, DeviationRight = 1}
                    ]},
                new Candidate { Name ="John", WorkingTime = 30,Salary = 10,
                    Competencies =
                    [
                        new CandidateCompetency(){ Competency = competencies[0], Value = 1, DeviationLeft = 0, DeviationRight = 0},
                        new CandidateCompetency(){ Competency = competencies[1], Value = 1, DeviationLeft = 0, DeviationRight = 0},
                        new CandidateCompetency(){ Competency = competencies[2], Value = 2, DeviationLeft = 0, DeviationRight = 0},
                        new CandidateCompetency(){ Competency = competencies[3], Value = 2, DeviationLeft = 0, DeviationRight = 0},
                        new CandidateCompetency(){ Competency = competencies[4], Value = 4, DeviationLeft = 0, DeviationRight = 0}
                    ]},
                new Candidate { Name ="Bill", WorkingTime = 20,Salary = 15,
                    Competencies =
                    [
                        new CandidateCompetency(){ Competency = competencies[0], Value = 3, DeviationLeft = 1, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[1], Value = 1, DeviationLeft = 0.5, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[2], Value = 2, DeviationLeft = 0.5, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[3], Value = 2, DeviationLeft = 1, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[4], Value = 4, DeviationLeft = 1, DeviationRight = 0}
                    ]},
                new Candidate { Name ="Scott", WorkingTime = 30,Salary = 8,
                    Competencies =
                    [
                        new CandidateCompetency(){ Competency = competencies[0], Value = 1, DeviationLeft = 0.5, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[1], Value = 1, DeviationLeft = 0.5, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[2], Value = 2, DeviationLeft = 1, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[3], Value = 2, DeviationLeft = 1, DeviationRight = 1},
                        new CandidateCompetency(){ Competency = competencies[4], Value = 3, DeviationLeft = 1, DeviationRight = 1}
                    ]},
                new Candidate { Name ="Paul", WorkingTime = 30,Salary = 8,
                    Competencies =
                    [
                        new CandidateCompetency(){ Competency = competencies[0], Value = 3, DeviationLeft = 0.5, DeviationRight = 0.5},
                        new CandidateCompetency(){ Competency = competencies[1], Value = 1, DeviationLeft = 0.5, DeviationRight = 0.5},
                        new CandidateCompetency(){ Competency = competencies[2], Value = 2, DeviationLeft = 1, DeviationRight = 0.5},
                        new CandidateCompetency(){ Competency = competencies[3], Value = 2, DeviationLeft = 1, DeviationRight = 0.5},
                        new CandidateCompetency(){ Competency = competencies[4], Value = 3, DeviationLeft = 0.4, DeviationRight = 0.4}
                    ]},
            ];
            foreach (var candidate in candidates)
            {
                foreach (var competency in candidate.Competencies)
                {
                    competency.DeviationLeft = competency.DeviationRight;
                }
            }


            await _databaseContext.AddRangeAsync(indicators);
            users[0].Competencies.AddRange(competencies);
            users[0].Candidates.AddRange(candidates);
            await _databaseContext.Users.AddRangeAsync(users);
            await _databaseContext.SaveChangesAsync();

            List<Candidate> selectedCandidates = [
                candidates[1],
                candidates[2],
                candidates[4]
            ];

            (var optimizationIndicators, var optimizationCandidates, var optimizationCompetencies) = Utilities.CopyIndicatorsCandidatesCompetencies(indicators, selectedCandidates, competencies);
            await _databaseContext.Indicators.AddRangeAsync(optimizationIndicators);
            await _databaseContext.Candidates.AddRangeAsync(optimizationCandidates);
            await _databaseContext.Competencies.AddRangeAsync(optimizationCompetencies);
            await _databaseContext.SaveChangesAsync();

            OptimizationResult optimizationResult = Utilities.Optimize(optimizationIndicators, optimizationCandidates, 0.9, 5000, 250, 5, optimizationCompetencies);

            List<HelpPage> helpPages =
            [
                new HelpPage(){Locale="en", Name = "General information", Text = "The information system consists of the following pages:\r\n- Home page;\r\n- Login to the system;\r\n- Optimization;\r\n- Help;\r\n- Personal account.\r\n\r\nThe home page contains basic information about the information system.\r\n\r\nThe login page allows you to register, log in, log out, and delete your account.\r\n\r\nThe Optimization page allows you to rank candidates and optimize the composition of the project team.\r\n\r\nThe Help page contains help pages that explain how to use the information system.\r\n\r\nThe personal account allows you to view your login and current subscription, and change your subscription."},
                new HelpPage(){Locale="en", Name = "Login page", Text = "To log in or register, you need to enter your login and password and click the appropriate button. If you are logged in, the logout and delete buttons will be available."},
                new HelpPage(){Locale="en", Name = "Optimization page", Text = "To optimize the composition of the project team, you first need to select the competencies that the team should have. A competency has three parameters: the minimum compliance of candidates with this competency, the possible deviation from this value, and the weight.\r\n\r\nCandidates' compliance with the competency is determined on an arbitrary scale selected by the user. Weight is the importance of a competency relative to others; during optimization, the weight of each competency is proportionally changed so that the sum of the weights of all competencies is equal to 1.\r\n\r\nThen you need to add candidates. Candidates have a match for each competency, a possible upward deviation, the number of working hours per week, and a salary per hour.\r\n\r\nThe optimization includes such parameters as the threshold value of the candidate's competency compliance, budget, labor costs for the project in man-hours, and project completion time in weeks. The threshold value of the candidate's competence is a value from 0 to 1, which corresponds to the minimum acceptable membership of the candidate's competence in the fuzzy set. Budget is the maximum acceptable cost of the project, for the team option, this value is calculated as the sum of the team members' salaries per week (number of working hours per week multiplied by the salary per hour) multiplied by the project duration. Labor costs for the project are the minimum number of man-hours that the team must work for a set period of time, for the team option, this value is calculated as the sum of the working hours per week of team members multiplied by the project duration.\r\n\r\nBefore optimization, you can rank the candidates to select only the best candidates and thus reduce the number of necessary calculations and team options as a result. To do this, select candidates and competencies and click the appropriate button. The result of the ranking will be a list of candidates sorted by the generalized competency.\r\n\r\nAfter selecting the competencies, candidates, and optimization parameters, you can start the optimization by clicking on the appropriate button. The result of the optimization will be options for possible teams. Team options include a list of candidates, project cost, man-hours spent on the project, maximum matching values for each competency, the sum of matching values for all competencies, and the sum of maximum values for each competency."},
                new HelpPage(){Locale="en", Name = "Help Pages", Text = "Help pages describe the information system and allow you to familiarize yourself with the main functions."},
                new HelpPage(){Locale="en", Name = "Account page", Text = "Account page allows you to see your login, current subscription, and its expiration date. Account page also displays available subscriptions and allows you to change your subscription."},
                new HelpPage(){Locale="ua", Name = "Загальна інформація", Text = "Інформаційна система складається з таких сторінок:\r\n– Домашня сторінка;\r\n– Вхід в систему;\r\n– Оптимізація;\r\n– Довідка;\r\n– Особистий кабінет.\r\n\r\nДомашня сторінка містить основну інформацію про інформаційну систему.\r\n\r\nСторінка входу дозволяє зареєструватися, ввійти в систему, вийти з системи та видалити аккаунт.\r\n\r\nСторінка оптимізації дозволяє провести ранжування кандидатів та оптимізацію складу команди проєкту.\r\n\r\nСторінка довідки містить довідкові сторінки в яких поясняється як користуватися інформаційною системою.\r\n\r\nОсобистий кабінет дозволяє переглянути логін та поточну підписку, змінити підписку."},
                new HelpPage(){Locale="ua", Name = "Вхід в систему", Text = "Для входу в систему або реєстрації необхідно ввести логін і пароль та натиснути відповідну кнопку. Якщо виконано вхід в систему, то будуть доступні кнопки виходу та видалення аккаунту."},
                new HelpPage(){Locale="ua", Name = "Оптимізація", Text = "Для оптимізації складу команди проєкту спочатку необхідно обрати компетенції, які повинна мати створена команда. Компетенція має три параметри: мінімальна відповідність кандидатів цій компетенції, можливе відхилення від цього значення та вага. Відповідність кандидатів компетенції визначається по довільній шкалі, обраній користувачем. Вага це важливість компетенції відносно інших, під час оптимізації вага кожної компетенції пропорційно змінюється щоб сума ваги всіх компетенцій дорівнювала 1.\r\n\r\nПотім необхідно додати кандидатів. Кандидати мають відповідність кожній компетенції, можливе відхилення в більшу сторону, кількість робочих годин в тиждень та зарплату за годину.\r\n\r\nВ оптимізації є такі параметри, як порогове значення вірогідності відповідності кандидата компетенції, бюджет, трудовитрати на виконання проєкта в людиногодинах та час виконання проєкту в тижнях. Порогове значення вірогідності відповідності кандидата компетенції це значення від 0 до 1 що відповідає мінімальній прийнятній приналежності відповідності кандидата компетенції до нечіткої множини. Бюджет це максимальна прийнятна вартість проєкту, для варіанта команди це значення розраховується як сума заробітної плати членів команди за тиждень (кількість робочих годин в тиждень перемножена на заробітну плату за годину) перемножена на час виконання проєкту. Трудовитрати на виконання проєкта це мінімальна кількість людиногодин, яку має пропрацювати команда за встановлений термін, для варіанта команди це значення розраховується як сума робочих годин в тиждень членів команди перемножена на час виконання проєкту.\r\n\r\nПеред оптимізацією можна провести ранжування кандидатів, щоб обрати лише найкращих кандидатів і таким чином зменшити кількість необхідних розрахунків та варіантів команд у результаті. Для цього потрібно вибрати кандидатів та компетенції і натиснути відповідну кнопку. Результатом ранжування буде список кандидатів, відсортований за усередненою зваженою компетенцією.\r\n\r\nПісля вибору компетенцій, кандидатів та параметрів оптимізації можна почати оптимізацію, натиснувши на відповідну кнопку. Результатом оптимізації будуть варіанти можливих команд. Варіанти команд містять список кандидатів, вартість виконання проєкту, витрачені на нього людиногодини, максимальні значення відповідності кожній компетенції, сума значень відповідності всіх компетенцій та сума максимальних значень кожної компетенції."},
                new HelpPage(){Locale="ua", Name = "Довідка", Text = "Сторінки довідки описують інформаційну систему та дозволяють ознайомитися з основними функціями."},
                new HelpPage(){Locale="ua", Name = "Особистий кабінет", Text = "Особистий кабінет дозволяє побачити логін, поточну підписку та термін її закінчення. Також в особистому кабінеті відображаються доступні підписки та є можливість змінити підписку."},
            ];

            users[0].OptimizationResults.Add(optimizationResult);
            await _databaseContext.HelpPages.AddRangeAsync(helpPages);
            await _databaseContext.SaveChangesAsync();

            return Ok();
        }
    }
}
