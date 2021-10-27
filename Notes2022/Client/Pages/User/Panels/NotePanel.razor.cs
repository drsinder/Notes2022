using System.Timers;
using System.Text;
using Notes2022.Shared;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Modal.Services;
using Blazored.Modal;
using Newtonsoft.Json;
using System.Net.Http.Json;

using Syncfusion.Blazor.SplitButtons;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Notes2022.Client.Pages.User.Dialogs;

namespace Notes2022.Client.Pages.User.Panels
{
    public partial class NotePanel
    {
        [CascadingParameter] public IModalService Modal { get; set; }

        [Parameter] public long NoteId { get; set; }
        [Parameter] public bool ShowChild { get; set; }
        [Parameter] public bool IsRootNote { get; set; }
        [Parameter] public bool ShowButtons { get; set; } = true;

        protected DisplayModel model { get; set; }

        public string respX { get; set; }


        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        public NotePanel()
        {
            ShowChild = false;
            IsRootNote = true;
        }

        protected override async Task OnParametersSetAsync()
        {
            model = await Http.GetFromJsonAsync<DisplayModel>("api/notecontent/" + NoteId);

            // set text to be displayed re responses
            respX = "";
            if (model.header.ResponseCount > 0)
                respX = " - " + model.header.ResponseCount + " Responses ";
            else if (model.header.ResponseOrdinal > 0)
                respX = " Response " + model.header.ResponseOrdinal;


        }

        private void OnClickRef(MouseEventArgs args)
        {
            ShowChild = true;
        }

        private void OnClickRefHide(MouseEventArgs args)
        {
            ShowChild = false;
        }

        private void OnClickResp(MouseEventArgs args)
        {
            long bnId  = model.header.Id;           // if base note
            if (model.header.ResponseOrdinal > 0)   // if response
            {
                bnId = model.header.BaseNoteId;
            }

            Navigation.NavigateTo("/newnote/" + model.noteFile.Id + "/" + bnId + "/" + model.header.Id);
        }

        private void OnDone(MouseEventArgs args)
        {
            Navigation.NavigateTo("/noteindex/" + model.noteFile.Id);
        }

        private async void OnPrint(MouseEventArgs args)
        {
            await PrintString(false);
        }

        private async void OnPrintString(MouseEventArgs args)
        {
            await PrintString(true);
        }

        /// <summary>
        /// Print a whole file if PrintFile is true
        /// </summary>
        protected async Task PrintString(bool wholeString)
        {
            NoteDisplayIndexModel Model = null;

            Model = await Http.GetFromJsonAsync<NoteDisplayIndexModel>("api/NoteIndex/" + model.noteFile.Id);
            Model.Notes = Model.AllNotes.FindAll(p => p.ResponseOrdinal == 0).OrderBy(p => p.NoteOrdinal).ToList();

            string respX = String.Empty;

            // keep track of base note
            NoteHeader baseHeader = Model.AllNotes.SingleOrDefault(p => p.Id == model.header.Id);

            NoteHeader currentHeader = baseHeader;

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

            if (wholeString && currentHeader.ResponseOrdinal < baseHeader.ResponseCount) // more responses in string
            {
                currentHeader = Model.AllNotes.Single(p => p.NoteOrdinal == currentHeader.NoteOrdinal && p.ResponseOrdinal == currentHeader.ResponseOrdinal + 1);

                goto reloop;        // print another note
            }

            currentHeader = baseHeader; // set back to base note

            string stuff = sb.ToString();           // turn accumulated output into a string

            var parameters = new ModalParameters();
            parameters.Add("PrintStuff", stuff);    // pass string to print dialog
            Modal.Show<PrintDlg>("", parameters);   // invloke print dialog with our output
        }

    }
}
