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

namespace TeamCompositionOptimizationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class AccountController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;

        public AccountController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        //todo
        /*
        // POST api/auth/login
        [HttpPost]
        [ActionName("login")]
        public async Task<IActionResult> Post([FromBody] CredentialsDto credentialsDto)
        {
            string login = credentialsDto.Login;
            string passwordHash = Utilities.HashPassword(credentialsDto.Password);

           User? user = await _databaseContext.Users.Where(x => x.Login == login).FirstOrDefaultAsync();
            if (user == null) { 
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
        [ActionName("logout")]
        public async Task<IActionResult> Get()
        {
           await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
        */
    }
}
