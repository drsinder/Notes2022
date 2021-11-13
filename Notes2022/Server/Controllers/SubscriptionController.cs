﻿/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2022, Dale Sinder
    **
    ** Name: SubscriptionController.cs
    **
    ** Description:
    **      Manage subscriptions
    **
    ** This program is free software: you can redistribute it and/or modify
    ** it under the terms of the GNU General Public License version 3 as
    ** published by the Free Software Foundation.   
    **
    ** This program is distributed in the hope that it will be useful,
    ** but WITHOUT ANY WARRANTY; without even the implied warranty of
    ** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    ** GNU General Public License version 3 for more details.
    **
    **  You should have received a copy of the GNU General Public License
    **  version 3 along with this program in file "license-gpl-3.0.txt".
    **  If not, see<http: //www.gnu.org/licenses/gpl-3.0.txt>.
    **
    **--------------------------------------------------------------------------*/


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes2022.Server.Data;
using Notes2022.Server.Models;
using Notes2022.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Notes2021Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [Route("api/[controller]/{fileId}")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubscriptionController(NotesDbContext db,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor
            )
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<List<Subscription>> Get()
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser me = await _userManager.FindByIdAsync(userId);

            List<Subscription> mine = await _db.Subscription.Where(p => p.SubscriberId == me.Id).ToListAsync();

            if (mine == null)
                mine = new List<Subscription>();

            return mine;
        }

        [HttpPost]
        public async Task Post(SCheckModel model)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser me = await _userManager.FindByIdAsync(userId);

            int fileId = model.fileId;

            NoteFile file = _db.NoteFile.Find(fileId);

            Subscription sub = new Subscription
            {
                NoteFileId = fileId,
                NoteFile = file,
                SubscriberId = me.Id,
            };

            _db.Subscription.Add(sub);
            _db.Entry(sub).State = EntityState.Added;
            await _db.SaveChangesAsync();
        }

        [HttpDelete]
        public async Task Delete(int fileId)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser me = await _userManager.FindByIdAsync(userId);
            Subscription mine = await _db.Subscription.SingleOrDefaultAsync(p => p.SubscriberId == me.Id && p.NoteFileId == fileId);
            if (mine == null)
                return;

            _db.Subscription.Remove(mine);
            await _db.SaveChangesAsync();
        }

    }
}