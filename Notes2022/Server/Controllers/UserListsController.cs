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
                UserData userData = new UserData();

                userData.UserId = user.Id;
                userData.DisplayName = user.DisplayName;
                userData.Email = user.Email;
                userData.TimeZoneID = user.TimeZoneID;
                userData.Ipref2 = user.Ipref2;
                userData.Ipref3 = user.Ipref3;
                userData.Ipref4 = user.Ipref4;
                userData.Ipref5 = user.Ipref5;
                userData.Pref6 = user.Pref6;
                userData.Pref7 = user.Pref7;
                userData.Pref8 = user.Pref8;
                userData.Pref1 = user.Pref1;
                userData.Pref2 = user.Pref2;
                userData.Pref3 = user.Pref3;
                userData.Pref4 = user.Pref4;
                userData.Pref5 = user.Pref5;
                userData.Pref6 = user.Pref6;
                userData.Pref7 = user.Pref7;
                userData.Pref8 = user.Pref8;
                userData.MyStyle = user.MyStyle;
                userData.MyGuid = user.MyGuid;

                list.Add(userData);
            }

            return list;
        }

    }
}
