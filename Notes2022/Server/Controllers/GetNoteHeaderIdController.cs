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
    [Route("api/[controller]/{notefileId:int}/{noteOrd:int}/{noteRespOrd:int}")]
    [ApiController]
    public class GetNoteHeaderIdController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetNoteHeaderIdController(NotesDbContext db,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor
            )
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<LongWrapper> Get(int notefileId, int noteOrd, int noteRespOrd)
        {
            long newId = 0;

            NoteHeader nh = _db.NoteHeader.SingleOrDefault(p => p.NoteFileId == notefileId && p.NoteOrdinal == noteOrd && p.ResponseOrdinal == noteRespOrd);
            if (nh != null)
                newId = nh.Id;

            LongWrapper longWrapper = new LongWrapper();
            longWrapper.mylong = newId;
            return longWrapper;
        }
    }
}
