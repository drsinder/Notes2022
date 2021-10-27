using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Notes2022.Server.Data;
using Notes2022.Server.Models;
using Notes2022.Shared;

namespace Notes2022.Server.Controllers
{
    [Route("api/[controller]")]
    [Route("api/[controller]/{headerId:long}")]
    [ApiController]
    public class PreviousNoteController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PreviousNoteController(NotesDbContext db,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor
          )
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<LongWrapper> Get(long headerId)
        {
            long newId = 0;

            NoteHeader oh = _db.NoteHeader.SingleOrDefault(x => x.Id == headerId);
            NoteHeader nh = null;
            nh = _db.NoteHeader.SingleOrDefault(p => p.NoteOrdinal == oh.NoteOrdinal && p.ResponseOrdinal == oh.ResponseOrdinal - 1);

            if (nh == null)
                nh = _db.NoteHeader.SingleOrDefault(p => p.NoteOrdinal == oh.NoteOrdinal - 1 && p.ResponseOrdinal == 0);

            if (nh != null)
                newId = nh.Id;

            LongWrapper longWrapper = new LongWrapper();
            longWrapper.mylong = newId;
            return longWrapper;
        }
    }
}
