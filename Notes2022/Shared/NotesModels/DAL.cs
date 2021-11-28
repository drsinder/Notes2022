using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;
using System.Net.Http.Json;
using System.Web;

namespace Notes2022.Shared
{
    public static class DAL
    {
        #region About
        public static async Task<AboutModel> GetAboutModel(GrpcChannel Channel)
        {
            try
            {
                var client = Channel.CreateGrpcService<INotes2022Service>();
                return await client.GetAboutModel();
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            return new AboutModel();
        }
        #endregion
        #region AccessList
        public static async Task DeleteAccessItem(GrpcChannel Channel, UpdateAccessRequest item)
        {
            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.DeleteAccessItem(item);
        }


        public static async Task CreateAccessItem(GrpcChannel Channel, UpdateAccessRequest item)
        {
            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.CreateAccessItem(item);
        }

        public static async Task UpdateAccessItem(GrpcChannel Channel, UpdateAccessRequest item)
        {
            try
            {
                var client = Channel.CreateGrpcService<INotes2022Service>();
                await client.UpdateAccessItem(item);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        public static async Task<List<NoteAccess>> GetAccessList(GrpcChannel Channel, string fileId)
        {
            try
            {
                var client = Channel.CreateGrpcService<INotes2022Service>();
                return await client.GetAccessList(fileId);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }

            return null;
        }


        public static async Task<NoteAccess> GetMyAccess(HttpClient Http, int FileId)
        {
            return await Http.GetFromJsonAsync<NoteAccess>("api/myaccess/" + FileId);
        }

        #endregion
        #region HomePageModel
        public static async Task<HomePageModel> GetHomePageData(HttpClient Http)
        {
            HomePageModel model = await Http.GetFromJsonAsync<HomePageModel>("api/HomePageData");
            return model;
        }

        public static async Task<HomePageModel> GetAdminPageData(GrpcChannel Channel, string eMail)
        {
            try
            {
                var client = Channel.CreateGrpcService<INotes2022Service>();
                return await client.GetAdminPageData(eMail);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            return null;
        }

        public static async Task<HomePageModel> GetHomePageData(GrpcChannel Channel, string eMail)
        {
            try
            {
                var client = Channel.CreateGrpcService<INotes2022Service>();
                return await client.GetHomePageData(eMail);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            return null;
        }

        #endregion
        #region Email
        public static async Task SendEmail(GrpcChannel Channel, EmailModel stuff)
        {
            //await Http.PostAsJsonAsync("api/Email", stuff);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.SendEmail(stuff);
        }
        #endregion
        #region Export
        public static async Task<NoteContent> GetExport2(HttpClient Http, long id)
        {
            return await Http.GetFromJsonAsync<NoteContent>("api/Export2/" + id);
        }

        public static async Task<List<NoteHeader>> GetExport(HttpClient Http, int fileid, int arcid, int noteOrd, int respOrd)
        {
            string req = "" + fileid + "." + arcid + "." + noteOrd + "." + respOrd;
            return await Http.GetFromJsonAsync<List<NoteHeader>>("api/Export/" + req);
        }

        public static async Task<JsonExport> GetExportJson(HttpClient Http, int fileid, int arcid)
        {
            string req = fileid.ToString() + "." + arcid.ToString();
            return await Http.GetFromJsonAsync<JsonExport>("api/ExportJson/" + req);
        }

        public static async Task Forward(HttpClient Http, ForwardViewModel stuff)
        {
            await Http.PostAsJsonAsync("api/Forward", stuff);
        }

        public static async Task<bool> Import(HttpClient Http, string NoteFile, string UploadFile)
        {
            return await Http.GetFromJsonAsync<bool>("api/Import/" + NoteFile + "/" + UploadFile);
        }

        #endregion
        #region Linked
        public static async Task<bool> LinkTest(HttpClient Http, string uri)
        {
            string appUriEncoded = HttpUtility.UrlEncode(uri);
            return await Http.GetFromJsonAsync<bool>("api/LinkTest/" + appUriEncoded);
        }

        public static async Task<bool> LinkTest2(HttpClient Http, string uri, string remoteFile)
        {
            string appUriEncoded = HttpUtility.UrlEncode(uri);
            return await Http.GetFromJsonAsync<bool>("api/LinkTest2/" + appUriEncoded + "/" + remoteFile);
        }

        public static async Task<List<LinkedFile>> GetLinked(HttpClient Http)
        {
            return await Http.GetFromJsonAsync<List<LinkedFile>>("api/Linked");
        }

        public static async Task CreateLinked(HttpClient Http, LinkedFile linked)
        {
            await Http.PostAsJsonAsync<LinkedFile>("api/Linked", linked);
        }

        public static async Task UpdateLinked(HttpClient Http, LinkedFile linked)
        {
            await Http.PutAsJsonAsync<LinkedFile>("api/Linked", linked);
        }

        public static async Task DeleteLinked(HttpClient Http, int Id)
        {
            await Http.DeleteAsync("api/Linked/" + Id);
        }

        #endregion
        #region Note
        public static async Task<NoteHeader> GetNewNote2(GrpcChannel Channel)
        {
            //return await Http.GetFromJsonAsync<NoteHeader>("api/NewNote2");

            var client = Channel.CreateGrpcService<INotes2022Service>();
            return await client.GetNewNote2();

        }

        public static async Task<NoteFile> GetNewNote(GrpcChannel Channel, int fileId)
        {
            //return await Http.GetFromJsonAsync<NoteFile>("api/NewNote/" + fileId);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            IntWrapper item = new IntWrapper() { myInt = fileId };
            return await client.GetNewNote(item);

        }

        public static async Task CreateNewNote(GrpcChannel Channel, TextViewModel tvm)
        {
            //await Http.PostAsJsonAsync("api/NewNote/", Model);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.CreateNewNote(tvm);

        }

        public static async Task UpdateNote(GrpcChannel Channel, TextViewModel tvm)
        {
            //await Http.PutAsJsonAsync("api/NewNote/", Model);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.UpdateNote(tvm);

        }

        public static async Task<int> GetFileIdForNoteId(GrpcChannel Channel, long NoteId)
        {
            //return await Http.GetFromJsonAsync<int>("api/GetFIleIdForNoteId/" + NoteId);

            IntWrapper item = new IntWrapper() { myLong = NoteId };
            var client = Channel.CreateGrpcService<INotes2022Service>();
            IntWrapper item2 = await client.GetFileIdForNoteId(item);
            return item2.myInt;

        }

        public static async Task<DisplayModel> GetNoteContent(GrpcChannel Channel, IntWrapper req)
        {
            //return await Http.GetFromJsonAsync<DisplayModel>("api/notecontent/" + NoteId + "/" + Vers);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            return await client.GetNoteContent(req);

        }

        //public static async Task<List<Tags>> GetTags(HttpClient Http, int nfid)
        //{
        //    return await Http.GetFromJsonAsync<List<Tags>>("api/Tags/" + nfid);
        //}

        public static async Task<List<NoteHeader>> GetVersions(GrpcChannel Channel, int fileid, int ordinal, int respordinal, int arcid)
        {
            //return await Http.GetFromJsonAsync<List<NoteHeader>>("api/Versions/" + fileid + "/"
            //    + ordinal + "/" + respordinal + "/" + arcid);

            IntWrapper intWrapper = new IntWrapper() 
            { 
                myInt = fileid,
                myInt2 = ordinal,
                myInt3 = respordinal,
                myInt4 = arcid
            };
            var client = Channel.CreateGrpcService<INotes2022Service>();
            return await client.GetVersions(intWrapper);
        }

        public static async Task<NoteDisplayIndexModel> GetNoteIndex(GrpcChannel Channel, IntWrapper req)
        {
            //return await Http.GetFromJsonAsync<NoteDisplayIndexModel>("api/NoteIndex/" + NotesfileId);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            return await client.GetNoteIndex(req);
        }

        public static async Task CopyNote(GrpcChannel Channel, CopyModel cm)
        {
            //await Http.PostAsJsonAsync("api/CopyNote", cm);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.CopyNote(cm);
        }

        public static async Task DeleteNote(GrpcChannel Channel, IntWrapper req)
        {
            //await Http.DeleteAsync("api/DeleteNote/" + Id);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.DeleteNote(req);

        }
        #endregion
        #region NoteFileAdmin

        public static async Task<List<NoteFile>> GetNoteFilesOrderedByName(HttpClient Http)
        {
            return await Http.GetFromJsonAsync<List<NoteFile>>("api/NoteFileAdmin");
        }

        public static async Task CreateNoteFile(HttpClient Http, CreateFileModel Model)
        {
            await Http.PostAsJsonAsync("api/NoteFileAdmin", Model);
        }

        public static async Task UpdateNoteFile(HttpClient Http, NoteFile Model)
        {
            await Http.PutAsJsonAsync("api/NoteFileAdmin", Model);
        }

        public static async Task DeleteNoteFile(HttpClient Http, int FileId)
        {
            await Http.DeleteAsync("api/NoteFileAdmin/" + FileId);
        }

        public static async Task CreateStdNoteFile(HttpClient Http, string filename)
        {
            await Http.PostAsJsonAsync("api/NoteFileAdminStd", new Stringy { value = filename });
        }
        #endregion
        #region Sequencer

        public static async Task<List<Sequencer>> GetSequencer(GrpcChannel Channel, string userid)
        {
            //return await Http.GetFromJsonAsync<List<Sequencer>>("api/sequencer");

            var client = Channel.CreateGrpcService<INotes2022Service>();
            SCheckModel model = new SCheckModel() { userId = userid};
            return await client.GetSequencer(model);


        }

        public static async Task CreateSequencer(GrpcChannel Channel, SCheckModel Model)
        {
            //await Http.PostAsJsonAsync("api/sequencer", Model);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.CreateSequencer(Model);
        }

        public static async Task DeleteSequencer(GrpcChannel Channel, SCheckModel Model)
        {
            //await Http.DeleteAsync("api/sequencer/" + SequencerFileId);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.DeleteSequencer(Model);
        }

        public static async Task UpateSequencer(GrpcChannel Channel, Sequencer seq)
        {
            //await Http.PutAsJsonAsync("api/sequencer", seq);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.UpdateSequencer(seq);
        }

        public static async Task UpateSequencerPosition(GrpcChannel Channel, Sequencer seq)
        {
            //await Http.PutAsJsonAsync("api/sequenceredit", seq);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.UpateSequencerPosition(seq);
        }
        #endregion
        #region Subscription
        public static async Task<List<Subscription>> GetSubscriptions(GrpcChannel Channel, string userid)
        {
            //return await Http.GetFromJsonAsync<List<Subscription>>("api/subscription");

            var client = Channel.CreateGrpcService<INotes2022Service>();
            return await client.GetSubscriptions(new IntWrapper() { userId = userid });

        }

        public static async Task DeleteSubscription(GrpcChannel Channel, SCheckModel Model)
        {
            //await Http.DeleteAsync("api/Subscription/" + fileId);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.DeleteSubscription(Model);

        }

        public static async Task CreateSubscription(GrpcChannel Channel, SCheckModel Model)
        {
            //await Http.PostAsJsonAsync("api/Subscription", Model);

            var client = Channel.CreateGrpcService<INotes2022Service>();
            await client.CreateSubscription(Model);
        }

        #endregion
        #region UserData
        public static async Task<UserData> GetUserData(HttpClient Http)
        {
            return await Http.GetFromJsonAsync<UserData>("api/User");
        }

        public static async Task UpdateUserData(HttpClient Http, UserData userData)
        {
            await Http.PutAsJsonAsync("api/User", userData);
        }


        public static async Task<EditUserViewModel> GetUserEdit(HttpClient Http, string UserId)
        {
            return await Http.GetFromJsonAsync<EditUserViewModel>("api/useredit/" + UserId);
        }

        //public static async Task<EditUserViewModel> GetUserEdit(GrpcChannel Channel, string UserId)
        //{
        //    //return await Http.GetFromJsonAsync<EditUserViewModel>("api/useredit/" + UserId);

        //    var client = Channel.CreateGrpcService<INotes2022Service>();
        //    return await client.GetUserEdit(new IntWrapper() { userId = UserId} );
        //}

        public static async Task UpdateUser(HttpClient Http, EditUserViewModel model)
        {
            await Http.PutAsJsonAsync("api/useredit", model);
        }

        public static async Task<List<UserData>> GetUserList(GrpcChannel Channel)
        {
            //return await Http.GetFromJsonAsync<List<UserData>>("api/userlists");

            var client = Channel.CreateGrpcService<INotes2022Service>();
            return await client.GetUserData();
        }
        #endregion
    }
}
