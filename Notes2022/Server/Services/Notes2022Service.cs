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
using System.Text;

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

        private async Task<NoteAccess> GetMyAccess(ApplicationUser me, int fileid, int ArcId)
        {
            return await AccessManager.GetAccess(_db, me.Id, fileid, ArcId);
        }
        public async ValueTask<NoteDisplayIndexModel> GetNoteIndex(IntWrapper req)
        {
            NoteDisplayIndexModel idxModel = new NoteDisplayIndexModel();

            int arcId = 0;

            ApplicationUser user = await _userManager.FindByIdAsync(req.userId);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            bool isUser = await _userManager.IsInRoleAsync(user, "User");
            if (!isUser)
                return idxModel;

            idxModel.myAccess = await GetMyAccess(user, req.myInt, arcId);
            if (isAdmin)
            {
                idxModel.myAccess.ViewAccess = true;
            }
            idxModel.noteFile = await NoteDataManager.GetFileById(_db, req.myInt);

            if (!idxModel.myAccess.ReadAccess && !idxModel.myAccess.Write)
            {
                idxModel.message = "You do not have access to file " + idxModel.noteFile.NoteFileName;
                return idxModel;
            }

            List<LinkedFile> linklist = await _db.LinkedFile.Where(p => p.HomeFileId == req.myInt).ToListAsync();
            if (linklist is not null && linklist.Count > 0)
                idxModel.linkedText = " (Linked)";

            idxModel.AllNotes = await NoteDataManager.GetAllHeaders(_db, req.myInt, arcId);

            idxModel.Notes = idxModel.AllNotes.FindAll(p => p.ResponseOrdinal == 0).OrderBy(p => p.NoteOrdinal).ToList();

            idxModel.UserData = LocalManager.GetUserData(user);
            idxModel.tZone = await LocalManager.GetUserTimeZone(user, _db);

            idxModel.Tags = await _db.Tags.Where(p => p.NoteFileId == req.myInt && p.ArchiveId == arcId).ToListAsync();

            idxModel.ArcId = arcId;

            return idxModel;
        }

        public async Task CopyNote(CopyModel req)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(req.UserId);
            UserData UserData = NoteDataManager.GetUserData(user);

            int fileId = req.FileId;

            string uid = UserData.UserId;
            NoteAccess myAccess = await AccessManager.GetAccess(_db, uid, fileId, 0);
            if (!myAccess.Write)
                return;

            NoteHeader Header = req.Note;
            bool whole = req.WholeString;
            NoteFile noteFile = await _db.NoteFile.SingleAsync(p => p.Id == fileId);

            if (!whole)
            {
                NoteContent cont = await _db.NoteContent.SingleAsync(p => p.NoteHeaderId == Header.Id);
                //cont.NoteHeader = null;
                List<Tags> tags = await _db.Tags.Where(p => p.NoteHeaderId == Header.Id).ToListAsync();

                string Body = string.Empty;
                Body = MakeHeader(Header, noteFile);
                Body += cont.NoteBody;

                Header = Header.CloneForLink();

                Header.Id = 0;
                Header.ArchiveId = 0;
                Header.LinkGuid = string.Empty;
                Header.NoteOrdinal = 0;
                Header.ResponseCount = 0;
                Header.NoteFileId = fileId;
                Header.BaseNoteId = 0;
                //Header.NoteFile = null;
                Header.AuthorID = UserData.UserId;
                Header.AuthorName = UserData.DisplayName;

                Header.CreateDate = Header.ThreadLastEdited = Header.LastEdited = DateTime.Now.ToUniversalTime();

                await NoteDataManager.CreateNote(_db, Header, Body, Tags.ListToString(tags), Header.DirectorMessage, true, false);

                return;
            }
            else
            {
                // get base note

                NoteHeader BaseHeader;
                BaseHeader = await _db.NoteHeader.SingleAsync(p => p.NoteFileId == Header.NoteFileId
                    && p.ArchiveId == Header.ArchiveId
                    && p.NoteOrdinal == Header.NoteOrdinal
                    && p.ResponseOrdinal == 0);

                Header = BaseHeader.CloneForLink();

                NoteContent cont = await _db.NoteContent.SingleAsync(p => p.NoteHeaderId == Header.Id);
                //cont.NoteHeader = null;
                List<Tags> tags = await _db.Tags.Where(p => p.NoteHeaderId == Header.Id).ToListAsync();

                string Body = string.Empty;
                Body = MakeHeader(Header, noteFile);
                Body += cont.NoteBody;

                Header.Id = 0;
                Header.ArchiveId = 0;
                Header.LinkGuid = string.Empty;
                Header.NoteOrdinal = 0;
                Header.ResponseCount = 0;
                Header.NoteFileId = fileId;
                Header.BaseNoteId = 0;
                //Header.NoteFile = null;
                Header.AuthorID = UserData.UserId;
                Header.AuthorName = UserData.DisplayName;

                Header.CreateDate = Header.ThreadLastEdited = Header.LastEdited = DateTime.Now.ToUniversalTime();

                Header.NoteContent = null;

                NoteHeader NewHeader = await NoteDataManager.CreateNote(_db, Header, Body, Tags.ListToString(tags), Header.DirectorMessage, true, false);

                for (int i = 1; i <= BaseHeader.ResponseCount; i++)
                {
                    NoteHeader RHeader = await _db.NoteHeader.SingleAsync(p => p.NoteFileId == BaseHeader.NoteFileId
                        && p.ArchiveId == BaseHeader.ArchiveId
                        && p.NoteOrdinal == BaseHeader.NoteOrdinal
                        && p.ResponseOrdinal == i);

                    Header = RHeader.CloneForLinkR();

                    cont = await _db.NoteContent.SingleAsync(p => p.NoteHeaderId == Header.Id);
                    tags = await _db.Tags.Where(p => p.NoteHeaderId == Header.Id).ToListAsync();

                    Body = string.Empty;
                    Body = MakeHeader(Header, noteFile);
                    Body += cont.NoteBody;

                    Header.Id = 0;
                    Header.ArchiveId = 0;
                    Header.LinkGuid = string.Empty;
                    Header.NoteOrdinal = NewHeader.NoteOrdinal;
                    Header.ResponseCount = 0;
                    Header.NoteFileId = fileId;
                    Header.BaseNoteId = NewHeader.Id;
                    //Header.NoteFile = null;
                    Header.ResponseOrdinal = 0;
                    Header.AuthorID = UserData.UserId;
                    Header.AuthorName = UserData.DisplayName;

                    Header.CreateDate = Header.ThreadLastEdited = Header.LastEdited = DateTime.Now.ToUniversalTime();

                    await NoteDataManager.CreateResponse(_db, Header, Body, Tags.ListToString(tags), Header.DirectorMessage, true, false);
                }

            }
        }

        private string MakeHeader(NoteHeader header, NoteFile noteFile)
        {
            StringBuilder sb = new();

            sb.Append("<div class=\"copiednote\">From: ");
            sb.Append(noteFile.NoteFileName);
            sb.Append(" - ");
            sb.Append(header.NoteSubject);
            sb.Append(" - ");
            sb.Append(header.AuthorName);
            sb.Append(" - ");
            sb.Append(header.CreateDate.ToShortDateString());
            sb.AppendLine("</div>");
            return sb.ToString();
        }


        public async Task CreateNewNote(TextViewModel tvm)
        {
            if (tvm.MyNote is null)
                return;

            ApplicationUser user = await _userManager.FindByIdAsync(tvm.UserId);
            bool test = await _userManager.IsInRoleAsync(user, "User");
            if (!test)
                return;

            ApplicationUser me = user;

            DateTime now = DateTime.Now.ToUniversalTime();
            NoteHeader nheader = new()
            {
                LastEdited = now,
                ThreadLastEdited = now,
                CreateDate = now,
                NoteFileId = tvm.NoteFileID,
                AuthorName = me.DisplayName,
                AuthorID = me.Id,
                NoteSubject = tvm.MySubject,
                DirectorMessage = tvm.DirectorMessage,
                ResponseOrdinal = 0,
                ResponseCount = 0
            };

            NoteHeader created;

            if (tvm.BaseNoteHeaderID == 0)
            {
                created = await NoteDataManager.CreateNote(_db, nheader, tvm.MyNote, tvm.TagLine, tvm.DirectorMessage, true, false);
            }
            else
            {
                nheader.BaseNoteId = tvm.BaseNoteHeaderID;
                nheader.RefId = tvm.RefId;
                created = await NoteDataManager.CreateResponse(_db, nheader, tvm.MyNote, tvm.TagLine, tvm.DirectorMessage, true, false);
            }


            await ProcessLinkedNotes();

            await SendNewNoteToSubscribers(created);

        }

        private async Task SendNewNoteToSubscribers(NoteHeader myNote)
        {
            List<Subscription> subs = await _db.Subscription
                .Where(p => p.NoteFileId == myNote.NoteFileId)
                .ToListAsync();

            if (subs is null || subs.Count == 0)
                return;

            ForwardViewModel fv = new ForwardViewModel();
            fv.NoteID = myNote.Id;

            fv.NoteFile = await _db.NoteFile.SingleAsync(p => p.Id == myNote.NoteFileId);

            string myEmail = await LocalService.MakeNoteForEmail(fv, fv.NoteFile, _db, Globals.PrimeAdminEmail, Globals.PrimeAdminName);

            EmailSender emailSender = new EmailSender();

            foreach (Subscription s in subs)
            {
                ApplicationUser usr = await _userManager.FindByIdAsync(s.SubscriberId);

                NoteAccess na = await AccessManager.GetAccess(_db, usr.Id, s.NoteFileId, 0);
                if (na.ReadAccess)
                    BackgroundJob.Enqueue(() => emailSender.SendEmailAsync(usr.UserName, myNote.NoteSubject, myEmail));
            }
        }

        private async Task ProcessLinkedNotes()
        {
            List<LinkQueue> items = await _db.LinkQueue.Where(p => p.Enqueued == false).ToListAsync();
            foreach (LinkQueue item in items)
            {
                LinkProcessor lp = new LinkProcessor(_db);
                BackgroundJob.Enqueue(() => lp.ProcessLinkAction(item.Id));
                item.Enqueued = true;
                _db.Update(item);
            }
            if (items.Count > 0)
                await _db.SaveChangesAsync();


        }

        public async Task<NoteFile> GetNewNote(IntWrapper item)
        {
            return _db.NoteFile.Single(p => p.Id == item.myInt);
        }

        public async Task<NoteHeader> GetNewNote2()
        {
            return _db.NoteHeader.OrderByDescending(p => p.Id).FirstOrDefault();
        }

        public async Task UpdateNote(TextViewModel tvm)
        {
            if (tvm.MyNote is null)
                return;

            // get old Noteheader
            NoteHeader nheader = await NoteDataManager.GetBaseNoteHeaderById(_db, tvm.NoteID);

            // upate header
            DateTime now = DateTime.Now.ToUniversalTime();
            nheader.NoteSubject = tvm.MySubject;
            nheader.DirectorMessage = tvm.DirectorMessage;
            //nheader.LastEdited = now;
            nheader.ThreadLastEdited = now;

            NoteContent nc = new()
            {
                NoteHeaderId = tvm.NoteID,
                NoteBody = tvm.MyNote
            };

            await NoteDataManager.EditNote(_db, _userManager, nheader, nc, tvm.TagLine);

            await ProcessLinkedNotes();
        }

        public async Task DeleteNote(IntWrapper req)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(req.userId);
            bool test = await _userManager.IsInRoleAsync(user, "User");
            if (!test)
                return;

            NoteHeader nh = _db.NoteHeader.Single(p => p.Id == req.myLong);
            await NoteDataManager.DeleteNote(_db, nh);
        }

        public async Task<IntWrapper> GetFileIdForNoteId(IntWrapper req)
        {
            NoteHeader header = _db.NoteHeader.SingleOrDefault(p => p.Id == req.myLong);
            IntWrapper w2 = new IntWrapper() { myInt = header.NoteFileId };
            return w2;
        }

        public async Task<DisplayModel> GetNoteContent(IntWrapper item)
        {
            string userId = item.userId;
            long id = item.myLong;
            int vers = item.myInt;

            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            NoteHeader nh = await _db.NoteHeader.SingleAsync(p => p.Id == id && p.Version == vers);
            NoteContent c = await _db.NoteContent.SingleAsync(p => p.NoteHeaderId == nh.Id);
            List<Tags> tags = await _db.Tags.Where(p => p.NoteHeaderId == nh.Id).ToListAsync();
            NoteFile nf = await _db.NoteFile.SingleAsync(p => p.Id == nh.NoteFileId);

            NoteAccess access = await AccessManager.GetAccess(_db, userId, nh.NoteFileId, nh.ArchiveId);

            bool canEdit = await _userManager.IsInRoleAsync(user, "Admin");
            if (userId == nh.AuthorID)
                canEdit = true;

            return new DisplayModel { header = nh, content = c, tags = tags, noteFile = nf, access = access, CanEdit = canEdit, IsAdmin = isAdmin };
        }

        public async Task<List<NoteHeader>> GetVersions(IntWrapper item)
        {
            int fileid = item.myInt;
            int ordinal = item.myInt2;
            int respordinal = item.myInt3;
            int arcid = item.myInt4;

            List<NoteHeader> hl;

            hl = _db.NoteHeader.Where(p => p.NoteFileId == fileid && p.Version != 0
                    && p.NoteOrdinal == ordinal && p.ResponseOrdinal == respordinal && p.ArchiveId == arcid)
                .OrderBy(p => p.Version)
                .ToList();

            return hl;
        }

        public async Task<List<UserData>> GetUserData()
        {
            List<ApplicationUser> users = _db.Users.ToList();

            List<UserData> list = new List<UserData>();
            foreach (ApplicationUser user in users)
            {
                UserData userData = NoteDataManager.GetUserData(user);
                list.Add(userData);
            }
            return list;
        }

        //public async Task<EditUserViewModel> GetUserEdit(IntWrapper req)
        //{
        //    ApplicationUser user = await _userManager.FindByIdAsync(req.userId);

        //    UserData me = NoteDataManager.GetUserData(user);

        //    var myRoles = await _userManager.GetRolesAsync(user);

        //    List<IdentityRole> allRoles = _db.Roles.OrderBy(p => p.Name).ToList();

        //    List<CheckedUser> myList = new List<CheckedUser>();

        //    foreach (IdentityRole item in allRoles)
        //    {
        //        CheckedUser it = new CheckedUser();
        //        it.theRole = item;
        //        it.isMember = myRoles.Where(p => p == item.Name).FirstOrDefault() is not null;
        //        myList.Add(it);
        //    }

        //    EditUserViewModel stuff = new EditUserViewModel()
        //    {
        //        UserData = me,
        //        RolesList = myList,
        //        HangfireLoc = Globals.HangfireLoc
        //    };
        //    return stuff;
        //}


        public async Task<List<Subscription>> GetSubscriptions(IntWrapper item)
        {
            string userId = item.userId;

            ApplicationUser me = await _userManager.FindByIdAsync(userId);

            List<Subscription> mine = await _db.Subscription.Where(p => p.SubscriberId == me.Id).ToListAsync();

            if (mine is null)
                mine = new List<Subscription>();

            List<Subscription> avail = new List<Subscription>();

            foreach (Subscription m in mine)
            {
                NoteAccess na = await AccessManager.GetAccess(_db, userId, m.NoteFileId, 0);
                if (na.ReadAccess)
                    avail.Add(m);
            }

            return avail;
        }

        public async Task CreateSubscription(SCheckModel model)
        {
            int fileId = model.fileId;

            NoteFile file = _db.NoteFile.Find(fileId);

            Subscription sub = new Subscription
            {
                NoteFileId = fileId,
                NoteFile = file,
                SubscriberId = model.userId
            };

            _db.Subscription.Add(sub);
            _db.Entry(sub).State = EntityState.Added;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteSubscription(SCheckModel model)
        {
            ApplicationUser me = await _userManager.FindByIdAsync(model.userId);
            Subscription mine = await _db.Subscription.SingleOrDefaultAsync(p => p.SubscriberId == me.Id && p.NoteFileId == model.fileId);
            if (mine is null)
                return;

            _db.Subscription.Remove(mine);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Sequencer>> GetSequencer(SCheckModel checkModel)
        {
            ApplicationUser me = await _userManager.FindByIdAsync(checkModel.userId);

            List<Sequencer> mine = await _db.Sequencer.Where(p => p.UserId == me.Id).OrderBy(p => p.Ordinal).ThenBy(p => p.LastTime).ToListAsync();

            if (mine is null)
                mine = new List<Sequencer>();

            List<Sequencer> avail = new List<Sequencer>();

            foreach (Sequencer m in mine)
            {
                NoteAccess na = await AccessManager.GetAccess(_db, checkModel.userId, m.NoteFileId, 0);
                if (na.ReadAccess)
                    avail.Add(m);
            }
            return avail.OrderBy(p => p.Ordinal).ToList();
        }

        public async Task CreateSequencer(SCheckModel checkModel)
        {
            ApplicationUser me = await _userManager.FindByIdAsync(checkModel.userId);

            List<Sequencer> mine = await _db.Sequencer.Where(p => p.UserId == me.Id).OrderByDescending(p => p.Ordinal).ToListAsync();

            int ord;
            if (mine is null || mine.Count == 0)
            {
                ord = 1;
            }
            else
            {
                ord = mine[0].Ordinal + 1;
            }

            Sequencer tracker = new Sequencer
            {
                Active = true,
                NoteFileId = checkModel.fileId,
                LastTime = DateTime.Now.ToUniversalTime(),
                UserId = me.Id,
                Ordinal = ord,
                StartTime = DateTime.Now.ToUniversalTime()
            };

            _db.Sequencer.Add(tracker);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteSequencer(SCheckModel checkModel)
        {
            ApplicationUser me = await _userManager.FindByIdAsync(checkModel.userId);
            Sequencer mine = await _db.Sequencer.SingleOrDefaultAsync(p => p.UserId == me.Id && p.NoteFileId == checkModel.fileId);
            if (mine is null)
                return;

            _db.Sequencer.Remove(mine);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateSequencer(Sequencer seq)
        {
            Sequencer modified = await _db.Sequencer.SingleAsync(p => p.UserId == seq.UserId && p.NoteFileId == seq.NoteFileId);

            modified.Active = seq.Active;
            if (seq.Active)  // starting to seq
            {
                modified.StartTime = DateTime.Now.ToUniversalTime();
            }
            else
            {
                modified.LastTime = seq.StartTime;
            }

            _db.Entry(modified).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task UpateSequencerPosition(Sequencer seq)
        {
            Sequencer modified = await _db.Sequencer.SingleAsync(p => p.UserId == seq.UserId && p.NoteFileId == seq.NoteFileId);

            modified.LastTime = seq.LastTime;
            modified.Ordinal = seq.Ordinal;

            _db.Entry(modified).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }


    }
}

