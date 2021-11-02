using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Notes2022.Server.Data;
using Notes2022.Shared;
using Microsoft.AspNetCore.Identity;
using Notes2022.Server.Models;

namespace Notes2022.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserListsController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public UserListsController(NotesDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<List<UserData>> Get()
        {
            List<ApplicationUser> users = _db.Users.ToList();

            List<UserData> list = new List<UserData>();
            foreach (ApplicationUser user in users)
            {
                UserData aux = new UserData();

                aux.UserId = user.Id;
                aux.DisplayName = user.DisplayName;
                aux.Email = user.Email;
                aux.TimeZoneID = user.TimeZoneID;

                aux.Ipref0 = user.Ipref0;
                aux.Ipref1 = user.Ipref1;
                aux.Ipref2 = user.Ipref2;
                aux.Ipref3 = user.Ipref3;
                aux.Ipref4 = user.Ipref4;
                aux.Ipref5 = user.Ipref5;
                aux.Ipref6 = user.Ipref6;
                aux.Ipref7 = user.Ipref7;
                aux.Ipref8 = user.Ipref8;
                aux.Ipref9 = user.Ipref9;

                aux.Pref0 = user.Pref0;
                aux.Pref1 = user.Pref1;
                aux.Pref2 = user.Pref2;
                aux.Pref3 = user.Pref3;
                aux.Pref4 = user.Pref4;
                aux.Pref5 = user.Pref5;
                aux.Pref6 = user.Pref6;
                aux.Pref7 = user.Pref7;
                aux.Pref8 = user.Pref8;
                aux.Pref9 = user.Pref9;

                aux.MyGuid = user.MyGuid;

                //aux.MyStyle = user.MyStyle;

                list.Add(aux);
            }

            return list;
        }

    }
}
