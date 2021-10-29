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
using Syncfusion.Blazor.Inputs;
using Notes2022.Client.Pages.User.Menus;

namespace Notes2022.Client.Pages.User.Panels
{
    public partial class NotePanel
    {
        [CascadingParameter] public IModalService Modal { get; set; }

        [Parameter] public long NoteId { get; set; }
        [Parameter] public bool ShowChild { get; set; }
        [Parameter] public bool IsRootNote { get; set; }
        [Parameter] public bool ShowButtons { get; set; } = true;
        [Parameter] public bool AltStyle { get; set; }

        protected List<NoteHeader> respHeaders { get; set; }

        //[Parameter] public string MyStyle { get; set; }

        protected string HeaderStyle { get; set; }
        protected string BodyStyle { get; set; }

        protected bool RespShown { get; set; }

        protected DisplayModel model { get; set; }

        public NoteMenu MyMenu { get; set; }

        SfTextBox sfTextBox { get; set; }
        public string NavString { get; set; }
        public string NavCurrentVal { get; set; }

        public string respX { get; set; }
        public string respY { get; set; }


        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        public NotePanel()
        {
            ShowChild = false;
            IsRootNote = true;
        }

        protected override async Task OnParametersSetAsync()
        {
            await GetData();
        }

        protected async Task GetData()
        {
            //MyStyle = "note-display";

            RespShown = false;

            HeaderStyle = "noteheader";
            BodyStyle = "notebody";

            if (AltStyle)
            {
                HeaderStyle += "-alt";
                BodyStyle += "-alt";
            }

            model = await Http.GetFromJsonAsync<DisplayModel>("api/notecontent/" + NoteId);

            // set text to be displayed re responses
            respX = respY = "";
            if (model.header.ResponseCount > 0)
            {
                respX = " - " + model.header.ResponseCount + " Responses ";
            }
            else if (model.header.ResponseOrdinal > 0)
            {
                respX = " Response " + model.header.ResponseOrdinal;
                respY = "." + model.header.ResponseOrdinal;
            }


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
            long bnId = model.header.Id;           // if base note
            if (model.header.ResponseOrdinal > 0)   // if response
            {
                bnId = model.header.BaseNoteId;
            }

            Navigation.NavigateTo("newnote/" + model.noteFile.Id + "/" + bnId + "/" + model.header.Id);
        }

        private async Task OnClickShowResp(MouseEventArgs args)
        {
            RespShown = !RespShown;

            if (RespShown)
            {
                respHeaders = await Http.GetFromJsonAsync<List<NoteHeader>>("api/GetResponseHeaders/" + model.header.Id);
            }
        }

        private void OnDone(MouseEventArgs args)
        {
            Navigation.NavigateTo("noteindex/" + model.noteFile.Id);
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

        private async void NavInputHandler(InputEventArgs args)
        {
            NavCurrentVal = args.Value;

            switch (NavCurrentVal)
            {
                case "I":
                case "L":
                    await MyMenu.ExecMenu("ListNotes");
                    await ClearNav();
                    return;

                case "N":
                    await MyMenu.ExecMenu("NewResponse");
                    await ClearNav();
                    return;

                case "X":
                    await MyMenu.ExecMenu("eXport");
                    await ClearNav();
                    return;

                case "J":
                    await MyMenu.ExecMenu("JsonExport");
                    await ClearNav();
                    return;

                case "m":
                    await MyMenu.ExecMenu("mailFromIndex");
                    await ClearNav();
                    return;

                case "P":
                    await MyMenu.ExecMenu("PrintFile");
                    await ClearNav();
                    return;

                case "Z":
                    var formModal = Modal.Show<HelpDialog2>();
                    await formModal.Result;

                    await ClearNav();
                    return;

                case "H":
                    await MyMenu.ExecMenu("HtmlFromIndex");
                    await ClearNav();
                    return;

                case "h":
                    await MyMenu.ExecMenu("htmlFromIndex");
                    await ClearNav();
                    return;

                default:
                    break;
            }
        }

        private async Task ClearNav()
        {
            NavCurrentVal = null;
            NavString = null;
        }

        private async Task KeyPressHandler(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                if (args.ShiftKey && string.IsNullOrEmpty(NavCurrentVal))
                {
                    string req = "api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + (model.header.NoteOrdinal + 1) + "/0";
                    long headerId0 = await Http.GetFromJsonAsync<long>(req);
                    
                    if (headerId0 != 0)
                        Navigation.NavigateTo("notedisplay/" + headerId0);
                    else
                        ShowMessage("Could not find note : " + req);

                    return;
                }
                else if (args.ShiftKey && NavCurrentVal == "-")
                {
                    // back one base note
                    string req = "api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + (model.header.NoteOrdinal - 1) + "/0";
                    long headerId0 = await Http.GetFromJsonAsync<long>(req);
                    if (headerId0 != 0)
                        Navigation.NavigateTo("notedisplay/" + headerId0);
                    else
                        ShowMessage("Could not find note : " + req);

                    return;
                }
                else if (NavCurrentVal == "-")
                {
                    // back one base note
                    string req = "api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + model.header.NoteOrdinal + "/" + (model.header.ResponseOrdinal - 1);
                    long headerId0 = await Http.GetFromJsonAsync<long>(req);
                    if (headerId0 != 0)
                        Navigation.NavigateTo("notedisplay/" + headerId0);
                    else
                        ShowMessage("Could not find note : " + req);

                    return;
                }

                else if (string.IsNullOrEmpty(NavCurrentVal))
                {
                    long headerId0 = await Http.GetFromJsonAsync<long>("api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + model.header.NoteOrdinal + "/" + (model.header.ResponseOrdinal + 1));
                    if (headerId0 != 0)
                        Navigation.NavigateTo("notedisplay/" + headerId0);
                    else
                        ShowMessage("Could not find note : " + NavCurrentVal);

                    return;
                }
                bool IsPlus = false;
                bool IsMinus = false;
                bool IsRespOnly = false;

                string stuff = NavCurrentVal.Replace(";", "").Replace(" ", "");

                if (stuff.StartsWith("+"))
                    IsPlus = true;
                if (stuff.StartsWith("-"))
                    IsMinus = true;

                stuff = stuff.Replace("+", "").Replace("-", "");

                if (stuff.StartsWith('.'))
                {
                    IsRespOnly = true;
                    stuff = stuff.Replace(".", "");
                }
                // parse string for # or #.#

                string[] parts = stuff.Split('.');
                if (parts.Length > 2)
                {
                    ShowMessage("Too many '.'s : " + parts.Length);
                }
                int noteNum;
                if (parts.Length == 1)
                {
                    if (!int.TryParse(parts[0], out noteNum))
                    {
                        ShowMessage("Could not parse : " + parts[0]);
                        //myTimer.Enabled = true;
                    }
                    else
                    {
                        if (!IsRespOnly)
                        {
                            if (IsPlus)
                                noteNum = model.header.NoteOrdinal + noteNum;
                            else if (IsMinus)
                                noteNum = model.header.NoteOrdinal - noteNum;
                        }
                        else
                        {
                            if (IsPlus)
                                noteNum = model.header.ResponseOrdinal + noteNum;
                            else if (IsMinus)
                                noteNum = model.header.ResponseOrdinal - noteNum;

                            long headerId2 = await Http.GetFromJsonAsync<long>("api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + model.header.NoteOrdinal + "/" + noteNum);
                            if (headerId2 != 0)
                                Navigation.NavigateTo("notedisplay/" + headerId2);
                            else
                                ShowMessage("Could not find note : " + NavCurrentVal);
                            return;
                        }
                        long headerId = await Http.GetFromJsonAsync<long>("api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + noteNum + "/0");
                        if (headerId != 0)
                            Navigation.NavigateTo("notedisplay/" + headerId);
                        else
                            ShowMessage("Could not find note : " + NavCurrentVal);
                        return;
                    }
                }
                else if (parts.Length == 2)
                {
                    if (!int.TryParse(parts[0], out noteNum))
                    {
                        ShowMessage("Could not parse : " + parts[0]);
                    }
                    int noteRespOrd;
                    if (!int.TryParse(parts[1], out noteRespOrd))
                    {
                        ShowMessage("Could not parse : " + parts[1]);
                    }
                    if (noteNum != 0 && noteRespOrd != 0)
                    {
                        {
                            if (IsPlus)
                                noteNum = model.header.NoteOrdinal + noteNum;
                            else if (IsMinus)
                                noteNum = model.header.NoteOrdinal - noteNum;
                            long headerId2 = await Http.GetFromJsonAsync<long>("api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + noteNum + "/0");
                            if (headerId2 != 0)
                                Navigation.NavigateTo("notedisplay/" + headerId2);
                            else
                                ShowMessage("Could not find note : " + NavCurrentVal);

                        }
                        long headerId = await Http.GetFromJsonAsync<long>("api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + noteNum + "/" + noteRespOrd);
                        if (headerId != 0)
                            Navigation.NavigateTo("notedisplay/" + headerId);
                        else
                            ShowMessage("Could not find note : " + NavCurrentVal);
                    }
                }
            }
        }

        private void ShowMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("MessageInput", message);
            Modal.Show<MessageBox>("", parameters);
        }

        System.Timers.Timer myTimer { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {   // have to wait a bit before putting focus in textbox
                myTimer = new System.Timers.Timer(300);
                myTimer.Enabled = true;
                myTimer.Elapsed += TimeUp;
            }
        }

        protected void TimeUp(Object source, ElapsedEventArgs e)
        {
            myTimer.Stop();
            myTimer.Enabled = false;

            //if (myTimer.Interval > 1000)
            //{
            //    sfTextBox.FocusOutAsync();
            //    NavCurrentVal = null;
            //    NavString = null;
            //}

            sfTextBox.FocusAsync();
        }
    }
}
