using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Blazored.Modal;
using Notes2022.Shared;

namespace Notes2022.Client.Pages.User.Dialogs
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
        protected async override Task OnInitializedAsync()
        {
            Files = await Http.GetFromJsonAsync<List<NoteFile>>("api/NoteFileAdmin");
            Files.Insert(0, new NoteFile{Id = 0, NoteFileName = "Select a file"});
        }

        protected async Task OnSubmit()
        {
            if (SelectedId == 0)
                return;
            CopyModel cm = new CopyModel();
            cm.FileId = SelectedId;
            cm.Note = Note;
            cm.WholeString = WholeString;
            //cm.UserData = UserData;
            await Http.PostAsJsonAsync("api/CopyNote", cm);
            await ModalInstance.CloseAsync();
        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }
    }
}