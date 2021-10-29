using Notes2022.Shared;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using Syncfusion.Blazor.SplitButtons;
using Blazored.Modal.Services;
using Blazored.Modal;
using Notes2022.Client.Pages.User.Dialogs;

namespace Notes2022.Client.Pages.User.Menus
{
    public partial class NoteMenu
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Parameter] public DisplayModel Model { get; set; }

        private static List<MenuItem> menuItems { get; set; }
        protected SfMenu<MenuItem> topMenu { get; set; }

        private bool HamburgerMode { get; set; } = false;

        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }

        public NoteMenu()
        {
        }

        protected override Task OnInitializedAsync()
        {
            menuItems = new List<MenuItem>();

            MenuItem item = new MenuItem() { Id = "ListNotes", Text = "Listing" };
            menuItems.Add(item);

            item = new MenuItem() { Id = "NextBase", Text = "Next Base" };
            menuItems.Add(item);

            item = new MenuItem() { Id = "PreviousBase", Text = "Previous Base" };
            menuItems.Add(item);

            item = new MenuItem() { Id = "NextNote", Text = "Next" };
            menuItems.Add(item);

            item = new MenuItem() { Id = "PreviousNote", Text = "Previous" };
            menuItems.Add(item);

            if (Model.access.ReadAccess)
            {
                MenuItem item2 = new MenuItem() { Id = "OutPutMenu", Text = "Output" };
                item2.Items = new List<MenuItem>();
                item2.Items.Add(new MenuItem() { Id = "Forward", Text = "Forward" });
                item2.Items.Add(new MenuItem() { Id = "Copy", Text = "Copy" });
                item2.Items.Add(new MenuItem() { Id = "mail", Text = "mail" });
                //item2.Items.Add(new MenuItem() { Id = "Mark", Text = "Mark for output" });
                item2.Items.Add(new MenuItem() { Id = "Html", Text = "Html (expandable)" });
                item2.Items.Add(new MenuItem() { Id = "html", Text = "html (flat)" });
                menuItems.Add(item2);

                if (Model.access.Respond)
                {
                    item = new MenuItem() { Id = "NewResponse", Text = "New Response" };
                    menuItems.Add(item);
                }

                if (Model.CanEdit)
                {
                    item = new MenuItem() { Id = "Edit", Text = "Edit" };
                    menuItems.Add(item);
                }

                item = new MenuItem() { Id = "Delete", Text = "Delete" };
                menuItems.Add(item);


                menuItems.Add(new MenuItem() { Id = "SearchFromNote", Text = "Search" });
                menuItems.Add(new MenuItem() { Id = "NoteHelp", Text = "Z for HELP" });
            }

            return Task.CompletedTask;
        }

        public async Task OnSelect(MenuEventArgs<MenuItem> e)
        {
            await ExecMenu(e.Item.Id);
        }

        public async Task ExecMenu(string id)
        {
            long myId;
            switch (id)
            {
                case "ListNotes":
                    Navigation.NavigateTo("noteindex/" + Model.noteFile.Id);
                    break;

                case "NewResponse":
                    long bnId = Model.header.Id;           // if base note
                    if (Model.header.ResponseOrdinal > 0)   // if response
                    {
                        bnId = Model.header.BaseNoteId;
                    }
                    Navigation.NavigateTo("newnote/" + Model.noteFile.Id + "/" + bnId + "/" + Model.header.Id);
                    break;

                case "Edit":
                    if (Model.CanEdit)
                        Navigation.NavigateTo("editnote/" + Model.header.Id, true);
                    break;

                case "NextBase":
                    myId = await Http.GetFromJsonAsync<long>("api/NextBaseNote/" + Model.header.Id);
                    if (myId > 0)
                    {
                        Navigation.NavigateTo("notedisplay/" + myId);
                    }
                    break;

                case "PreviousBase":
                    myId = await Http.GetFromJsonAsync<long>("api/PreviousBaseNote/" + Model.header.Id);
                    if (myId > 0)
                    {
                        Navigation.NavigateTo("notedisplay/" + myId);
                    }
                    break;

                case "NextNote":
                    myId = await Http.GetFromJsonAsync<long>("api/NextNote/" + Model.header.Id);
                    if (myId > 0)
                    {
                        Navigation.NavigateTo("notedisplay/" + myId);
                    }
                    break;

                case "PreviousNote":
                    myId = await Http.GetFromJsonAsync<long>("api/PreviousNote/" + Model.header.Id);
                    if (myId > 0)
                    {
                        Navigation.NavigateTo("notedisplay/" + myId);
                    }
                    break;

                case "NoteHelp":
                    Modal.Show<HelpDialog2>();
                    break;

                case "Delete":
                    ShowMessage("Delete not implemented yet.");
                    break;

                case "Forward":
                    ShowMessage("Forward not implemented yet.");
                    break;

                case "Copy":
                    ShowMessage("Copy not implemented yet.");
                    break;

                case "mail":
                    await DoEmail();
                    break;

                case "Html":
                    DoExport(true, true);
                    break;

                case "html":
                    DoExport(true, false);
                    break;


            }
        }

        private void DoExport(bool isHtml, bool isCollapsible, bool isEmail = false, string emailaddr = null)
        {
            var parameters = new ModalParameters();

            ExportViewModel vm = new ExportViewModel();
            vm.ArchiveNumber = Model.header.ArchiveId;
            vm.isCollapsible = isCollapsible;
            vm.isDirectOutput = !isEmail;
            vm.isHtml = isHtml;
            vm.NoteFile = Model.noteFile;
            vm.NoteOrdinal = Model.header.NoteOrdinal;
            vm.Email = emailaddr;

            parameters.Add("Model", vm);
            parameters.Add("FileName", Model.noteFile.NoteFileName + (isHtml ? ".html" : ".txt"));

            Modal.Show<ExportUtil1>("", parameters);
        }

        private async Task DoEmail()
        {
            string emailaddr;
            var parameters = new ModalParameters();
            var formModal = Modal.Show<Email>("", parameters);
            var result = await formModal.Result;
            if (result.Cancelled)
                return;
            emailaddr = (string)result.Data;
            if (string.IsNullOrEmpty(emailaddr))
                return;

            DoExport(true, true, true, emailaddr);

        }


        private void ShowMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("MessageInput", message);
            Modal.Show<MessageBox>("", parameters);
        }
    }
}
