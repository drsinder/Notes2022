﻿
using System.Timers;
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

namespace Notes2022.Client.Shared
{
    public partial class NoteMenu
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Parameter] public DisplayModel Model { get; set; }
        //[Parameter] public bool ToolTips { get; set; } = true;

        private static List<MenuItem> menuItems { get; set; }
        protected SfMenu<MenuItem> topMenu { get; set; }

        private bool HamburgerMode { get; set; } = false;

        //protected string Tip1 = "Go to the list of available notefiles (L)";
        //protected string Tip2 = "Write a New base note (N)";
        //protected string Tip3 = "<p>Export the file as text (X)</p><p>Export the file as expandable Html (H)</p><p>Export the file as flat Html (h)</p>p>mail the file (m)</p><p>Mark note strings you have written in for output</p><p>Output marked notes (O)</p><p>Print entire file (P)</p><p>Json file export (J)</p>";
        //protected string Tip4 = "Search the file for something (S)";
        //protected string Tip5 = "Show the HELP dialog (Z)";
        //protected string Tip6 = "View or Edit the Access Controls (A)";


        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }

        public NoteMenu()
        {
        }

        protected override async Task OnParametersSetAsync()
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
                item2.Items.Add(new MenuItem() { Id = "mailFromNote", Text = "mail" });
                //item2.Items.Add(new MenuItem() { Id = "Mark", Text = "Mark for output" });
                item2.Items.Add(new MenuItem() { Id = "HtmlFromNote", Text = "Html (expandable)" });
                item2.Items.Add(new MenuItem() { Id = "htmlFromNote", Text = "html (flat)" });
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

            //Width = await jsRuntime.InvokeAsync<int>("getWidth", "x");
        }

        public async Task OnSelect(MenuEventArgs<MenuItem> e)
        {
            await ExecMenu(e.Item.Id);
        }

        private async Task ExecMenu(string id)
        {
            LongWrapper lw = null;

            switch (id)
            {
                case "ListNotes":
                    Navigation.NavigateTo("/noteindex/" + Model.noteFile.Id);
                    break;

                case "NewResponse":
                    long bnId = Model.header.Id;           // if base note
                    if (Model.header.ResponseOrdinal > 0)   // if response
                    {
                        bnId = Model.header.BaseNoteId;
                    }
                    Navigation.NavigateTo("/newnote/" + Model.noteFile.Id + "/" + bnId + "/" + Model.header.Id);
                    break;

                case "Edit":
                    if (Model.CanEdit)
                        Navigation.NavigateTo("/editnote/" + Model.header.Id, true);
                    break;

                case "NextBase":
                    lw = await Http.GetFromJsonAsync<LongWrapper>("api/NextBaseNote/" + Model.header.Id);
                    if (lw.mylong > 0)
                        Navigation.NavigateTo("/notedisplay/" + lw.mylong, true);
                    break;

                case "PreviousBase":
                    lw = await Http.GetFromJsonAsync<LongWrapper>("api/PreviousBaseNote/" + Model.header.Id);
                    if (lw.mylong > 0)
                        Navigation.NavigateTo("/notedisplay/" + lw.mylong, true);
                    break;

                case "NextNote":
                    lw = await Http.GetFromJsonAsync<LongWrapper>("api/NextNote/" + Model.header.Id);
                    if (lw.mylong > 0)
                        Navigation.NavigateTo("/notedisplay/" + lw.mylong, true);
                    break;

                case "PreviousNote":
                    lw = await Http.GetFromJsonAsync<LongWrapper>("api/PreviousNote/" + Model.header.Id);
                    if (lw.mylong > 0)
                        Navigation.NavigateTo("/notedisplay/" + lw.mylong, true);
                    break;

                case "NoteHelp":
                    Modal.Show<HelpDialog2>();
                    break;

            }

            //ShowMessage(id);
        }

        private void ShowMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("MessageInput", message);
            Modal.Show<MessageBox>("", parameters);
        }
    }
}
