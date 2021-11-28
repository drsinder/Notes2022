using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Notes2022.Server.Data;
using Notes2022.Server.Models;
using Notes2022.Shared;
using Microsoft.EntityFrameworkCore;
using Hangfire;

namespace Notes2022.Server.Services
{
    public class Notes2022Service : INotes2022Service
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public Notes2022Service(NotesDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<AboutModel> GetAboutModel()
        {
            AboutModel model = new AboutModel
            {
                PrimeAdminName = Globals.PrimeAdminName,
                PrimeAdminEmail = Globals.PrimeAdminEmail,
                StartupDateTime = Globals.StartupDateTime
            };

            return model;
        }

        public async ValueTask<HomePageModel> GetAdminPageData(string userName)
        {
            HomePageModel model = new HomePageModel();

            ApplicationUser user = await _userManager.FindByIdAsync(userName);
            bool test = await _userManager.IsInRoleAsync(user, "Admin");
            if (!test)
                return model;

            NoteFile hpmf = _db.NoteFile.Where(p => p.NoteFileName == "homepagemessages").FirstOrDefault();
            if (hpmf is not null)
            {
                NoteHeader hpmh = _db.NoteHeader.Where(p => p.NoteFileId == hpmf.Id).OrderByDescending(p => p.CreateDate).FirstOrDefault();
                if (hpmh is not null)
                {
                    model.Message = _db.NoteContent.Where(p => p.NoteHeaderId == hpmh.Id).FirstOrDefault().NoteBody;
                }
            }

            model.NoteFiles = _db.NoteFile
                .OrderBy(p => p.NoteFileName).ToList();

            model.NoteAccesses = new List<NoteAccess>();

            List<ApplicationUser> udl = _db.Users.ToList();

            model.UserListData = new List<UserData>();
            foreach (ApplicationUser userx in udl)
            {
                UserData ud = NoteDataManager.GetUserData(userx);
                model.UserListData.Add(ud);
            }

            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    model.UserData = NoteDataManager.GetUserData(user);

                    foreach (NoteFile nf in model.NoteFiles)
                    {
                        NoteAccess na = await AccessManager.GetAccess(_db, user.Id, nf.Id, 0);
                        model.NoteAccesses.Add(na);
                    }
                }
                else
                {
                    model.UserData = new UserData { TimeZoneID = Globals.TimeZoneDefaultID };
                }
            }
            catch
            {
                model.UserData = new UserData { TimeZoneID = Globals.TimeZoneDefaultID };
            }

            return model;

        }

        public async ValueTask<HomePageModel> GetHomePageData(string userName)
        {
            HomePageModel model = new HomePageModel();

            model.Message = string.Empty;
            NoteFile hpmf = _db.NoteFile.Where(p => p.NoteFileName == "homepagemessages").FirstOrDefault();
            if (hpmf is not null)
            {
                NoteHeader hpmh = _db.NoteHeader.Where(p => p.NoteFileId == hpmf.Id && p.IsDeleted == false).OrderByDescending(p => p.CreateDate).FirstOrDefault();
                if (hpmh is not null)
                {
                    model.Message = _db.NoteContent.Where(p => p.NoteHeaderId == hpmh.Id).FirstOrDefault().NoteBody;
                }
            }

            model.NoteFiles = _db.NoteFile
                .OrderBy(p => p.NoteFileName).ToList();

            model.NoteAccesses = new List<NoteAccess>();

            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userName);
                    model.UserData = NoteDataManager.GetUserData(user);

                    foreach (NoteFile nf in model.NoteFiles)
                    {
                        NoteAccess na = await AccessManager.GetAccess(_db, user.Id, nf.Id, 0);
                        model.NoteAccesses.Add(na);
                    }

                    if (model.NoteAccesses.Count > 0)
                    {
                        NoteFile[] theList = new NoteFile[model.NoteFiles.Count];
                        model.NoteFiles.CopyTo(theList);
                        foreach (NoteFile nf2 in theList)
                        {
                            NoteAccess na = model.NoteAccesses.SingleOrDefault(p => p.NoteFileId == nf2.Id);
                            if (!na.ReadAccess && !na.Write && !na.EditAccess)
                            {
                                model.NoteFiles.Remove(nf2);
                            }
                        }
                    }
                }
                else
                {
                    model.UserData = new UserData { TimeZoneID = Globals.TimeZoneDefaultID };
                }
            }
            catch
            {
                model.UserData = new UserData { TimeZoneID = Globals.TimeZoneDefaultID };
            }

            return model;
        }

        public async ValueTask<List<NoteAccess>> GetAccessList(string fileid)
        {
            int Id = int.Parse(fileid);

            List<NoteAccess> list = await _db.NoteAccess.Where(p => p.NoteFileId == Id).OrderBy(p => p.ArchiveId).ToListAsync();
            return list;
        }

        public async Task UpdateAccessItem(UpdateAccessRequest req)
        {
            NoteAccess item = req.item;

            ApplicationUser me = await _userManager.FindByIdAsync(req.eMail);

            NoteAccess myAccess = await AccessManager.GetAccess(_db, me.Id, item.NoteFileId, item.ArchiveId);
            if (!myAccess.EditAccess)
                return;

            NoteAccess work = await _db.NoteAccess.Where(p => p.NoteFileId == item.NoteFileId
                && p.ArchiveId == item.ArchiveId && p.UserID == item.UserID)
                .FirstOrDefaultAsync();
            if (work is null)
                return;

            work.ReadAccess = item.ReadAccess;
            work.Respond = item.Respond;
            work.Write = item.Write;
            work.DeleteEdit = item.DeleteEdit;
            work.SetTag = item.SetTag;
            work.ViewAccess = item.ViewAccess;
            work.EditAccess = item.EditAccess;

            _db.Update(work);
            await _db.SaveChangesAsync();
        }

        public async Task CreateAccessItem(UpdateAccessRequest req)
        {
            NoteAccess item = req.item;
            
            ApplicationUser me = await _userManager.FindByIdAsync(req.eMail);

            NoteAccess myAccess = await AccessManager.GetAccess(_db, me.Id, item.NoteFileId, item.ArchiveId);
            if (!myAccess.EditAccess)
                return;

            NoteAccess work = await _db.NoteAccess.Where(p => p.NoteFileId == item.NoteFileId
                && p.ArchiveId == item.ArchiveId && p.UserID == item.UserID)
                .FirstOrDefaultAsync();
            if (work is not null)
                return;     // already exists

            if (item.UserID == Globals.AccessOtherId())
                return;     // can not create "Other"

            NoteFile nf = _db.NoteFile.Where(p => p.Id == item.NoteFileId).FirstOrDefault();

            if (item.ArchiveId < 0 || item.ArchiveId > nf.NumberArchives)
                return;

            _db.NoteAccess.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAccessItem(UpdateAccessRequest req)
        {
            NoteAccess item = req.item;

            if (item.UserID == Globals.AccessOtherId())
                return;     // can not delete "Other"

            // also can not delete self
            ApplicationUser user = await _userManager.FindByIdAsync(req.eMail);
            NoteAccess myAccess = await AccessManager.GetAccess(_db, user.Id, item.NoteFileId, item.ArchiveId);
            if (!myAccess.EditAccess)
                return; // no edit access

            if (item.UserID == user.Id)
                return;     // can not delete self"

            _db.NoteAccess.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task SendEmail(EmailModel req)
        {
            EmailSender sender = new EmailSender();
            BackgroundJob.Enqueue(() => sender.SendEmailAsync(req.email, req.subject, req.payload));
        }


    }
}
