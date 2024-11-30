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

namespace TeamCompositionOptimizationApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class HelpController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;

        public HelpController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        // GET: api/help/get
        [HttpGet]
        [ActionName("get")]
        public async Task<IActionResult> Get()
        {
            List<HelpPage> helpPages = await _databaseContext.HelpPages.ToListAsync();

            if (helpPages.Count == 0)
            {
                return NoContent();
            }

            List<HelpPageNameDto> HelpPageNameDtos = helpPages.Select(x => new HelpPageNameDto() { Id = x.Id, Name = x.Name, Locale = x.Locale }).ToList();
            return Ok(HelpPageNameDtos);
        }

        // GET: api/help/getlocalized
        [HttpGet("{locale}")]
        [ActionName("getlocalized")]
        public async Task<IActionResult> Get(string locale)
        {
            List<HelpPage> helpPages = await _databaseContext.HelpPages.Where(x=>x.Locale == locale).ToListAsync();

            if (helpPages.Count == 0)
            {
                return NoContent();
            }

            List<HelpPageNameDto> HelpPageNameDtos = helpPages.Select(x => new HelpPageNameDto() { Id = x.Id, Name = x.Name,Locale = x.Locale }).ToList();
            return Ok(HelpPageNameDtos);
        }

        // GET api/help/get/5
        [HttpGet("{id}")]
        [ActionName("get")]
        public async Task<IActionResult> Get(int id)
        {
            HelpPage? helpPage = await _databaseContext.HelpPages.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (helpPage == null)
            {
                return NotFound();
            }

            HelpPageDto helpPageDto = new HelpPageDto() { Id=helpPage.Id, Name = helpPage.Name, Text = helpPage.Text,Locale = helpPage.Locale };
            return Ok(helpPageDto);
        }

        // POST api/help/post
        [HttpPost]
        [ActionName("post")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Post([FromBody] HelpPageDto helpPageDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.IsSuperuser == false)
            {
                return Forbid();
            }

            HelpPage helpPage = new() { Name = helpPageDto.Name, Text = helpPageDto.Text, Locale = helpPageDto.Locale };
            _databaseContext.HelpPages.Add(helpPage);
            await _databaseContext.SaveChangesAsync();
            return Ok();
        }

        // PATCH api/help/patch/5
        [HttpPatch("{id}")]
        [ActionName("patch")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Patch(int id, [FromBody] HelpPageDto helpPageDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.IsSuperuser == false)
            {
                return Forbid();
            }

            HelpPage? helpPage = await _databaseContext.HelpPages.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (helpPage == null)
            {
                return NotFound();
            }

            if (helpPageDto.Name != null)
            {
                helpPage.Name = helpPageDto.Name;
            }
            if (helpPageDto.Text != null)
            {
                helpPage.Text = helpPageDto.Text;
            }
            if (helpPageDto.Locale != null)
            {
                helpPage.Locale = helpPageDto.Locale;
            }

            await _databaseContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/help/delete/5
        [HttpDelete("{id}")]
        [ActionName("delete")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            User? user = await _databaseContext.Users
                .Where(x => x.Login == login).AsSplitQuery().FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.IsSuperuser == false)
            {
                return Forbid();
            }

            HelpPage? helpPage = await _databaseContext.HelpPages.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (helpPage == null)
            {
                return NotFound();
            }
           
            _databaseContext.HelpPages.Remove(helpPage);
            await _databaseContext.SaveChangesAsync();
            return Ok();
        }
    }
}
