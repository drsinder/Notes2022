using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Notes2022.Shared
{
    [ServiceContract]
    public interface INotes2022Service
    {
        [OperationContract]
        public Task<AboutModel> GetAboutModel();

        //[OperationContract]
        //public ValueTask<HomePageModel> GetAdminPageData(Stringy userName);

        [OperationContract]
        public ValueTask<HomePageModel> GetHomePageData(string userName);

        [OperationContract]
        public ValueTask<List<NoteAccess>> GetAccessList(string fileid);

        public Task UpdateAccessItem(UpdateAccessRequest req);

        public Task CreateAccessItem(UpdateAccessRequest req);

        public Task DeleteAccessItem(UpdateAccessRequest req);

        public Task SendEmail(EmailModel req);

        [OperationContract]
        public ValueTask<NoteDisplayIndexModel> GetNoteIndex(IntWrapper req);

        [OperationContract]
        public Task CopyNote(CopyModel req);

        [OperationContract]
        public Task CreateNewNote(TextViewModel req);

        [OperationContract]
        public Task<NoteFile> GetNewNote(IntWrapper item);

        [OperationContract]
        public Task<NoteHeader> GetNewNote2();

        [OperationContract]
        public Task UpdateNote(TextViewModel req);

        [OperationContract]
        public Task DeleteNote(IntWrapper req);

        [OperationContract]
        public Task<IntWrapper> GetFileIdForNoteId(IntWrapper req);

        [OperationContract]
        public Task<DisplayModel> GetNoteContent(IntWrapper item);

        [OperationContract]
        public Task<List<NoteHeader>> GetVersions(IntWrapper item);

        [OperationContract]
        public Task<List<UserData>> GetUserData();

        //[OperationContract]
        //public Task<EditUserViewModel> GetUserEdit(IntWrapper item);

        [OperationContract]
        public Task<List<Subscription>> GetSubscriptions(IntWrapper item);

        [OperationContract]
        public Task CreateSubscription(SCheckModel checkModel);

        [OperationContract]
        public Task DeleteSubscription(SCheckModel checkModel);

        [OperationContract]
        public Task<List<Sequencer>> GetSequencer(SCheckModel checkModel);

        [OperationContract]
        public Task CreateSequencer (SCheckModel checkModel);

        [OperationContract]
        public Task DeleteSequencer(SCheckModel checkModel);

        [OperationContract]
        public Task UpdateSequencer(Sequencer seq);

        [OperationContract]
        public Task UpateSequencerPosition(Sequencer seq);

        [OperationContract]
        public Task<JsonExport> GetJsonExport(IntWrapper req);

        [OperationContract]
        public Task<List<NoteHeader>> GetExport(IntWrapper req);

        [OperationContract]
        public Task<NoteContent> GetExport2(IntWrapper req);

        [OperationContract]
        public Task<List<NoteHeader>> GetExport3(IntWrapper req);

        [OperationContract]
        public Task Forward(ForwardViewModel req);

        [OperationContract]
        public Task<ForwardViewModel> Import(ForwardViewModel req);

        [OperationContract]
        public Task<ForwardViewModel> LinkTest(string req);

        [OperationContract]
        public Task<ForwardViewModel> LinkTest2(ForwardViewModel req);

        [OperationContract]
        public Task<List<LinkedFile>> GetLinkedFiles();


        [OperationContract]
        public Task CreateLinked(LinkedFile linked);

        [OperationContract]
        public Task UpdateLinked(LinkedFile linked);

        [OperationContract]
        public Task DeleteLinked(string Id);

        [OperationContract]
        public Task<NoteAccess> GetMyAccess(IntWrapper req);

        [OperationContract]
        public Task CreateStdNoteFile(ForwardViewModel req);

        [OperationContract]
        public Task<List<NoteFile>> GetNoteFiles();

        [OperationContract]
        public Task CreateNoteFile(CreateFileModel cfm);

        [OperationContract]
        public Task UpdateNoteFile(NoteFile noteFile);

        [OperationContract]
        public Task DeleteNoteFile(IntWrapper req);


    }
}
