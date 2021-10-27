
using System.Timers;
using Notes2022.Shared;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Syncfusion.Blazor.SplitButtons;
using Blazored.Modal.Services;
using Blazored.Modal;
using Notes2022.Client.Pages.User.Dialogs;
using System.Text;
using System.Net.Http.Json;

namespace Notes2022.Client.Shared
{
    public partial class ListMenu
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Parameter] public NoteDisplayIndexModel Model { get; set; }
        //[Parameter] public bool ToolTips { get; set; } = false;

        private static List<MenuItem> menuItems { get; set; }
        protected SfMenu<MenuItem> topMenu { get; set; }

        private bool HamburgerMode { get; set; } = false;

        //protected string Tip1 = "Go to the list of available notefiles (L)";
        //protected string Tip2 = "Write a New base note (N)";
        //protected string Tip3 = "<p>Export the file as text (X)</p><p>Export the file as expandable Html (H)</p><p>Export the file as flat Html (h)</p>p>mail the file (m)</p><p>Mark note strings you have written in for output</p><p>Output marked notes (O)</p><p>Print entire file (P)</p><p>Json file export (J)</p>";
        //protected string Tip4 = "Search the file for something (S)";
        //protected string Tip5 = "Show the HELP dialog (Z)";
        //protected string Tip6 = "View or Edit the Access Controls (A)";


        private bool IsPrinting { get; set; } = false;
        protected string sliderValueText { get; set; }
        protected int baseNotes { get; set; }
        protected int currNote { get; set; }


        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        public ListMenu()
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            baseNotes = Model.Notes.Count;
            sliderValueText = "1/" + baseNotes;
            menuItems = new List<MenuItem>();

            MenuItem item = new MenuItem (){ Id = "ListNoteFiles", Text = "List Note Files" };
            menuItems.Add(item);
            if (Model.myAccess.Write)
            {
                item = new MenuItem() { Id = "NewBaseNote", Text = "New Base Note" };
                menuItems.Add(item);
            }
            if (Model.myAccess.ReadAccess)
            {
                MenuItem item2 = new MenuItem() { Id = "OutPutMenu", Text = "Output" };
                item2.Items = new List<MenuItem>();
                item2.Items.Add(new MenuItem() { Id = "eXport", Text = "eXport" });
                item2.Items.Add(new MenuItem() { Id = "HtmlFromIndex", Text = "Html (expandable)" });
                item2.Items.Add(new MenuItem() { Id = "htmlFromIndex", Text = "html (flat)" });
                item2.Items.Add(new MenuItem() { Id = "mailFromIndex", Text = "mail" });
                //item2.Items.Add(new MenuItem() { Id = "MarkMine", Text = "Mark my notes for output" });
                item2.Items.Add(new MenuItem() { Id = "PrintFile", Text = "Print entire file" });

                //if (Model.isMarked)
                //{
                //    item2.Items.Add(new MenuItem() { Id = "OutputMarked", Text = "Output marked notes" });
                //}

                item2.Items.Add(new MenuItem { Id = "JsonExport", Text = "Json Export - for later import" });

                menuItems.Add(item2);

                menuItems.Add(new MenuItem() { Id = "SearchFromIndex", Text = "Search" });
                menuItems.Add(new MenuItem() { Id = "ListHelp", Text = "Z for HELP" });
                if (Model.myAccess.EditAccess || Model.myAccess.ViewAccess)
                {
                    menuItems.Add(new MenuItem() { Id = "AccessControls", Text = "Access Controls" });
                }
            }

            //Width = await jsRuntime.InvokeAsync<int>("getWidth", "x");
        }

        public async Task OnSelect(MenuEventArgs<MenuItem> e)
        {
            await ExecMenu(e.Item.Id);
        }

        public async Task ExecMenu(string id)
        {
            switch (id)
            {
                case "ListNoteFiles":
                    Navigation.NavigateTo("/notesfiles/");
                    break;

                case "NewBaseNote":
                    Navigation.NavigateTo("/newnote/" + Model.noteFile.Id + "/0" + "/0");
                    break;

                case "ListHelp":
                    Modal.Show<HelpDialog>();
                    break;

                case "eXport":
                    DoExport(false, false);
                    break;

                case "HtmlFromIndex":
                    DoExport(true, true);
                    break;

                case "htmlFromIndex":
                    DoExport(true, false);
                    break;

                case "JsonExport":
                    DoJson();
                    break;

                case "mailFromIndex":
                    await DoEmail();
                    break;

                case "PrintFile":
                    await PrintFile();
                    break;

                default:
                    ShowMessage(id);
                    break;

            }

            //ShowMessage(id);

        }

        private async Task PrintFile()
        {
            currNote = 1;
            IsPrinting = true;


            await PrintString();
        }

        /// <summary>
        /// Print a whole file if PrintFile is true
        /// </summary>
        protected async Task PrintString()
        {
            string respX = String.Empty;

            // keep track of base note
            NoteHeader baseHeader = Model.Notes[0];

            NoteHeader currentHeader = Model.Notes[0];

            StringBuilder sb = new StringBuilder();

            sb.Append("<h4 class=\"text-center\">" + Model.noteFile.NoteFileTitle + "</h4>");

        reloop: // come back here to do another note
            respX = "";
            if (currentHeader.ResponseCount > 0)
                respX = " - " + currentHeader.ResponseCount + " Responses ";
            else if (currentHeader.ResponseOrdinal > 0)
                respX = " Response " + currentHeader.ResponseOrdinal;


            sb.Append("<div class=\"noteheader\"><p> <span class=\"keep-right\">Note: ");
            sb.Append(currentHeader.NoteOrdinal + " " + respX);
            sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;</span></p><h4>Subject: ");
            sb.Append(currentHeader.NoteSubject);
            sb.Append("<br />Author: ");
            sb.Append(currentHeader.AuthorName + "    ");
            sb.Append((Globals.LocalTimeBlazor(currentHeader.LastEdited).ToLongDateString()) + " " + (Globals.LocalTimeBlazor(currentHeader.LastEdited).ToShortTimeString())/* + " " + Model.tZone.Abbreviation*/);

            NoteContent currentContent = await Http.GetFromJsonAsync<NoteContent>("api/Export2/" + currentHeader.Id);
            
            if (!string.IsNullOrEmpty(currentContent.DirectorMessage))
            {
                sb.Append("<br /><span>Director Message: ");
                sb.Append(currentContent.DirectorMessage);
                sb.Append("</span>");
            }
            //if (tags != null && tags.Count > 0)
            //{
            //    sb.Append(" <br /><span>Tags: ");
            //    foreach (Tags tag in tags)
            //    {
            //        sb.Append(tag.Tag + " ");
            //    }
            //    sb.Append("</span>");
            //}
            sb.Append("</h4></div><div class=\"notebody\" >");
            sb.Append(currentContent.NoteBody);
            sb.Append("</div>");

            if (currentHeader.ResponseOrdinal < baseHeader.ResponseCount) // more responses in string
            {
                currentHeader = Model.AllNotes.Single(p => p.NoteOrdinal == currentHeader.NoteOrdinal && p.ResponseOrdinal == currentHeader.ResponseOrdinal + 1);
                
                goto reloop;        // print another note
            }

            currentHeader = baseHeader; // set back to base note

            //if (PrintFile)  // whole file printing
            {
                NoteHeader next = Model.Notes.SingleOrDefault(p => p.NoteOrdinal == currentHeader.NoteOrdinal + 1);
                if (next != null)       // still base notes left to print
                {
                    currentHeader = next;   // set current note and base note
                    baseHeader = next;
                    //await SetNote();        // set important stuff
                    sliderValueText = currentHeader.NoteOrdinal + "/" + baseNotes;  // update progress test
                    currNote = currentHeader.NoteOrdinal;                           // update progress bar

                    goto reloop;    // print another string
                }
            }

            string stuff = sb.ToString();           // turn accumulated output into a string

            var parameters = new ModalParameters();
            parameters.Add("PrintStuff", stuff);    // pass string to print dialog
            Modal.Show<PrintDlg>("", parameters);   // invloke print dialog with our output

            IsPrinting = false;
        }

        private void DoExport(bool isHtml, bool isCollapsible, bool isEmail = false, string emailaddr = null)
        {
            var parameters = new ModalParameters();

            ExportViewModel vm = new ExportViewModel();
            vm.ArchiveNumber = Model.ArcId;
            vm.isCollapsible = isCollapsible;
            vm.isDirectOutput = !isEmail;
            vm.isHtml = isHtml;
            vm.NoteFile = Model.noteFile;
            vm.NoteOrdinal = 0;
            vm.Email = emailaddr;

            parameters.Add("Model", vm);
            parameters.Add("FileName", Model.noteFile.NoteFileName + (isHtml ? ".html" : ".txt"));

            Modal.Show<ExportUtil1>("", parameters);
        }

        private void DoJson()
        {
            var parameters = new ModalParameters();

            ExportViewModel vm = new ExportViewModel();
            vm.ArchiveNumber = Model.ArcId;
            vm.NoteFile = Model.noteFile;
            vm.NoteOrdinal = 0;

            parameters.Add("model", vm);

            Modal.Show<ExportJson>("", parameters);
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



            //            ShowMessage(emailaddr);

        }


        private void ShowMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("MessageInput", message);
            Modal.Show<MessageBox>("", parameters);
        }
    }
}
