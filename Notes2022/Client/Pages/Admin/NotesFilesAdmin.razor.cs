using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using Blazored.Modal;
using Blazored.Modal.Services;
using Notes2022.Shared;
using Notes2022.Client.Pages.Admin.Dialogs;

namespace Notes2022.Client.Pages.Admin
{
    public partial class NotesFilesAdmin
    {
        private List<string> todo { get; set; }
        private List<NoteFile> files { get; set; }
        private string? message;
        private HomePageModel model { get; set; }

        [Inject] HttpClient Http { get; set; }
        [Inject] AuthenticationStateProvider AuthProv { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] IModalService Modal { get; set; }
        public NotesFilesAdmin()
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            await GetStuff();
        }

        protected async Task GetStuff()
        {
            model = await Http.GetFromJsonAsync<HomePageModel>("api/AdminPageData");

            todo = new List<string> { "announce", "pbnotes", "noteshelp", "pad", "homepagemessages" };

            foreach (NoteFile file in model.NoteFiles)
            {
                if (file.NoteFileName == "announce")
                    todo.Remove("announce");
                if (file.NoteFileName == "pbnotes")
                    todo.Remove("pbnotes");
                if (file.NoteFileName == "noteshelp")
                    todo.Remove("noteshelp");
                if (file.NoteFileName == "pad")
                    todo.Remove("pad");
                if (file.NoteFileName == "homepagemessages")
                    todo.Remove("homepagemessages");
            }
            files = model.NoteFiles;
        }

        private async Task CreateAnnounce()
        {
            await Http.PostAsJsonAsync("api/NoteFileAdminStd", new Stringy { value = "announce" });
            Navigation.NavigateTo("/admin/notefilelist", true);
        }

        private async Task CreatePbnotes()
        {
            await Http.PostAsJsonAsync("api/NoteFileAdminStd", new Stringy { value = "pbnotes" });
            Navigation.NavigateTo("/admin/notefilelist", true);
        }

        private async Task CreateNotesHelp()
        {
            await Http.PostAsJsonAsync("api/NoteFileAdminStd", new Stringy { value = "noteshelp" });
            Navigation.NavigateTo("/admin/notefilelist", true);
        }

        private async Task CreatePad()
        {
            await Http.PostAsJsonAsync("api/NoteFileAdminStd", new Stringy { value = "pad" });
            Navigation.NavigateTo("/admin/notefilelist", true);
        }

        private async Task CreateHomePageMessages()
        {
            await Http.PostAsJsonAsync("api/NoteFileAdminStd", new Stringy { value = "homepagemessages" });
            Navigation.NavigateTo("/admin/notefilelist", true);
        }

        async void CreateNoteFile(int Id)
        {
            this.StateHasChanged();
            var parameters = new ModalParameters();
            parameters.Add("FileId", Id);
            var xModal = Modal.Show<CreateNoteFile>("Create Notefile", parameters);
            var result = await xModal.Result;
            if (!result.Cancelled)
                Navigation.NavigateTo("/admin/notefilelist", true);
        }

        async void DeleteNoteFile(int Id)
        {
            NoteFile file = files.Find(p => p.Id == Id);

            this.StateHasChanged();
            var parameters = new ModalParameters();
            parameters.Add("FileId", Id);
            parameters.Add("FileName", file.NoteFileName);
            parameters.Add("FileTitle", file.NoteFileTitle);
            var xModal = Modal.Show<DeleteNoteFile>("Delete", parameters);
            var result = await xModal.Result;
            if (!result.Cancelled)
                Navigation.NavigateTo("/admin/notefilelist", true);
        }

        async void NoteFileDetails(int Id)
        {
            NoteFile file = files.Find(p => p.Id == Id);

            var parameters = new ModalParameters();
            parameters.Add("FileId", Id);
            parameters.Add("FileName", file.NoteFileName);
            parameters.Add("FileTitle", file.NoteFileTitle);
            parameters.Add("LastEdited", file.LastEdited);
            parameters.Add("NumberArchives", file.NumberArchives);
            parameters.Add("Owner", model.UserListData.Find(p => p.UserId == file.OwnerId).DisplayName);
            var xModal = Modal.Show<NoteFileDetails>("Details", parameters);
            await xModal.Result;
        }

        async void EditNoteFile(int Id)
        {

            NoteFile file = files.Find(p => p.Id == Id);

            var parameters = new ModalParameters();
            parameters.Add("FileId", Id);
            parameters.Add("FileName", file.NoteFileName);
            parameters.Add("FileTitle", file.NoteFileTitle);
            parameters.Add("LastEdited", file.LastEdited);
            parameters.Add("NumberArchives", file.NumberArchives);
            parameters.Add("Owner", file.OwnerId);
            var xModal = Modal.Show<EditNoteFile>("Edit Notefile", parameters);
            var result = await xModal.Result;
            if (!result.Cancelled)
                Navigation.NavigateTo("/admin/notefilelist", true);
        }

    }
}
