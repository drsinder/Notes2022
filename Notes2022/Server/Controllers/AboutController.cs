using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notes2022.Server.Data;
using Notes2022.Server.Models;
using Notes2022.Shared;

namespace Notes2022.Server.Controllers
{
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NotesDbContext _db;

        public AboutController(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, NotesDbContext db)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        [HttpGet]
        public async Task<AboutModel> Get()
        {
            AboutModel model = new AboutModel
            {
                PrimeAdminName = Globals.PrimeAdminName,
                PrimeAdminEmail = Globals.PrimeAdminEmail,
                StartupDateTime = Globals.StartupDateTime
            };

            return model;
        }

    }
}
