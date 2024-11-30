using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeamCompositionOptimizationApi.Models.Database;
using TeamCompositionOptimizationApi.Models.DTOs;
using Task_Board_API.Models.ErrorResponses;
using Microsoft.AspNetCore.Authorization;
using TeamCompositionOptimizationApi.Services;
using Microsoft.AspNetCore.Routing.Matching;

namespace TeamCompositionOptimizationApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;

        public AuthController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        // POST api/auth/login
        [HttpPost]
        [ActionName("login")]
        public async Task<IActionResult> Login([FromBody] CredentialsDto credentialsDto)
        {
            string login = credentialsDto.Login;
            string passwordHash = Utilities.HashPassword(credentialsDto.Password);

            User? user = await _databaseContext.Users.Where(x => x.Login == login).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.PasswordHash != passwordHash)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>{
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);

            return Ok();
        }

        // GET: api/auth/logout
        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [ActionName("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        // POST api/auth/register
        [HttpPost]
        [ActionName("register")]
        public async Task<IActionResult> Register([FromBody] CredentialsDto credentialsDto)
        {
            if (await _databaseContext.Users.Where(x => x.Login == credentialsDto.Login).AnyAsync())
            {
                Dictionary<string, string> errors = new()
                {
                    { "login", "This email already used" }
                };
                return BadRequest(new ValidationError(errors));
            }

            string passwordHash = Utilities.HashPassword(credentialsDto.Password);
            Subscription? subscription = _databaseContext.Subscriptions.Where(x => x.Name == "normal").FirstOrDefault();
            if (subscription == null) {
                subscription = new Subscription() { Name = "normal", MaxCandidatesCount = 10, MaxCompetenciesCount = 10 };
                _databaseContext.Subscriptions.Add(subscription);
                await _databaseContext.SaveChangesAsync();
            }

            User user = new User() { Login = credentialsDto.Login, PasswordHash = passwordHash ,Subscription = subscription };
            _databaseContext.Users.Add(user);
            await _databaseContext.SaveChangesAsync();

            var claims = new List<Claim>{
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);

            return Ok();
        }

        // POST api/auth/deleteaccount
        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [ActionName("deleteaccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users.Where(x => x.Login == login).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _databaseContext.Remove(user);
            await _databaseContext.SaveChangesAsync();

            return Ok();
        }

        // GET api/auth/accountinfo
        [HttpGet]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [ActionName("accountinfo")]
        public async Task<IActionResult> AccountInfo()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return NoContent();
            }

            User? user = await _databaseContext.Users.Include(x => x.Subscription).Where(x => x.Login == login).FirstOrDefaultAsync();
            if (user == null)
            {
                return NoContent();
            }

            UserDto userDto = new()
            {
                Login = user.Login,
                Subscription = new()
                {
                    Id = user.Subscription.Id,
                    Name = user.Subscription.Name,
                    MaxCandidatesCount = user.Subscription.MaxCandidatesCount,
                    MaxCompetenciesCount = user.Subscription.MaxCompetenciesCount,
                },
                SubscriptionDueDate = user.SubscriptionDueDate,
                IsSuperuser = user.IsSuperuser
            };

            return Ok(userDto);
        }

        // GET api/auth/subscriptions
        [HttpGet]
        [ActionName("subscriptions")]
        public async Task<IActionResult> Subscriptions()
        {
            List<Subscription> subscriptions = await _databaseContext.Subscriptions.ToListAsync();
            if (subscriptions.Count == 0)
            {
                return NoContent();
            }

            List<SubscriptionDto> subscriptionDtos = subscriptions.Select(x => new SubscriptionDto()
            {
                Id = x.Id,
                Name = x.Name,
                MaxCandidatesCount = x.MaxCandidatesCount,
                MaxCompetenciesCount = x.MaxCompetenciesCount
            }).ToList();

            return Ok(subscriptionDtos);
        }

        // GET api/auth/changesubscription/1
        [HttpGet("{id}")]
        [ActionName("changesubscription")]
        public async Task<IActionResult> ChangeSubscription(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users.Include(x => x.Subscription).Where(x => x.Login == login).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            Subscription? subscription = await _databaseContext.Subscriptions.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (subscription == null)
            {
                return NotFound();
            }

            user.Subscription = subscription;
            await _databaseContext.SaveChangesAsync();

            return Ok();
        }

    }
}
