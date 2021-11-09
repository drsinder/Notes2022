using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes2022.Server.Data;
using Notes2022.Server.Models;
using Notes2022.Shared;
using System.Security.Claims;

namespace Notes2022.Server.Controllers
{
    [Route("api/[controller]/{noteid:long}")]
    [ApiController]
    public class DeleteNoteController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteNoteController(NotesDbContext db,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor
            )
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpDelete]
        public async Task Delete(long noteid)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            bool test = await _userManager.IsInRoleAsync(user, "User");
            if (!test)
                return;

            NoteHeader nh = _db.NoteHeader.Single(p => p.Id == noteid);
            nh.IsDeleted = true;
            _db.Entry(nh).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            if (nh.ResponseOrdinal == 0 && nh.ResponseCount > 0)
            {
                // delete all responses
                for (int i = 1; i <= nh.ResponseCount; i++)
                {
                    NoteHeader rh = _db.NoteHeader.Single(p => p.ResponseOrdinal == i && p.Version == 0);
                    rh.IsDeleted = true;
                    _db.Entry(rh).State = EntityState.Modified;
                }
                await _db.SaveChangesAsync();
            }
        }

    }
}
