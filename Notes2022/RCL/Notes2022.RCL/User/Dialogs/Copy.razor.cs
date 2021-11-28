using Blazored.Modal;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Notes2022.Shared;
using System.Net.Http.Json;

namespace Notes2022.RCL.User.Dialogs
{
    public partial class Copy
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public NoteHeader Note { get; set; }

        //[Parameter] public UserData UserData { get; set; }
        private List<NoteFile> Files { get; set; }

        private bool WholeString { get; set; }

        private int SelectedId { get; set; } = 0;

        [Inject] GrpcChannel Channel { get; set; }
        public Copy()
        { }
        
        protected async override Task OnInitializedAsync()
        {
            Files = await DAL.GetNoteFilesOrderedByName(Http);
            Files.Insert(0, new NoteFile { Id = 0, NoteFileName = "Select a file" });
        }

        protected async Task OnSubmit()
        {
            if (SelectedId == 0)
                return;
            CopyModel cm = new CopyModel();
            cm.FileId = SelectedId;
            cm.Note = Note;
            cm.WholeString = WholeString;
            cm.UserId = Globals.UserData.UserId;
            await DAL.CopyNote(Channel, cm);
            await ModalInstance.CloseAsync();
        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }
    }
}