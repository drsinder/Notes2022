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
using Microsoft.JSInterop;

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
        [Parameter] public bool IsMini { get; set; }
        [Parameter] public int Vers { get; set; } = 0;

        protected List<NoteHeader> respHeaders { get; set; }

        //[Parameter] public string MyStyle { get; set; }

        protected string HeaderStyle { get; set; }
        protected string BodyStyle { get; set; }

        protected bool RespShown { get; set; }
        protected bool? RespShownSw { get; set; }

        protected bool RespFlipped { get; set; }

        protected bool EatEnter { get; set; }

        protected bool ShowVers { get; set; } = false;

        protected DisplayModel model { get; set; }

        public NoteMenu MyMenu { get; set; }

        SfTextBox sfTextBox { get; set; }
        public string NavString { get; set; }
        //public string NavCurrentVal { get; set; }

        public string respX { get; set; }
        public string respY { get; set; }


        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
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
            RespShown = false;

            HeaderStyle = "noteheader";
            BodyStyle = "notebody";

            if (AltStyle)
            {
                HeaderStyle += "-alt";
                BodyStyle += "-alt";
            }

            model = await Http.GetFromJsonAsync<DisplayModel>("api/notecontent/" + NoteId + "/" + Vers);

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

        private void OnClickResp(MouseEventArgs args)
        {
            long bnId = model.header.Id;           // if base note
            if (model.header.ResponseOrdinal > 0)   // if response
            {
                bnId = model.header.BaseNoteId;
            }

            Navigation.NavigateTo("newnote/" + model.noteFile.Id + "/" + bnId + "/" + model.header.Id);
        }

        private async Task ShowRespChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            if (RespShown)
            {
                respHeaders = await Http.GetFromJsonAsync<List<NoteHeader>>("api/GetResponseHeaders/" + model.header.Id);
            }
        }

        private void FlipRespChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            if (RespFlipped)
                respHeaders = respHeaders.OrderByDescending(x => x.ResponseOrdinal).ToList();
            else
                respHeaders = respHeaders.OrderBy(x => x.ResponseOrdinal).ToList();
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
            //Model.Notes = Model.AllNotes.FindAll(p => p.ResponseOrdinal == 0).OrderBy(p => p.NoteOrdinal).ToList();

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
            NavString = args.Value;
            await Task.CompletedTask;
            EatEnter = false;
        }

        private async Task ClearNav()
        {
            NavString = null;
            await Task.CompletedTask;
        }

        private async Task KeyUpHandler(KeyboardEventArgs args)
        {
            switch (NavString)
            {
                case "I":
                case "L":
                    await ClearNav();
                    await MyMenu.ExecMenu("ListNotes");
                    return;

                case "N":
                    await ClearNav();
                    await MyMenu.ExecMenu("NewResponse");
                    return;

                case "Z":
                    await ClearNav();
                    Modal.Show<HelpDialog2>();
                    EatEnter = true;
                    return;

                case "E":
                    await ClearNav();
                    await MyMenu.ExecMenu("Edit");
                    return;

                case "B":
                    await ClearNav();
                    await MyMenu.ExecMenu("PreviousBase");
                    return;

                case "b":
                    await ClearNav();
                    await MyMenu.ExecMenu("PreviousNote");
                    return;

                case "D":
                    await ClearNav();
                    await MyMenu.ExecMenu("Delete");
                    EatEnter = true;
                    return;

                case "F":
                    await ClearNav();
                    await MyMenu.ExecMenu("Forward");
                    return;

                case "C":
                    await ClearNav();
                    await MyMenu.ExecMenu("Copy");
                    return;

                case "m":
                    await ClearNav();
                    await MyMenu.ExecMenu("mail");
                    return;

                case "H":
                    await ClearNav();
                    await MyMenu.ExecMenu("Html");
                    return;

                case "h":
                    await ClearNav();
                    await MyMenu.ExecMenu("html");
                    return;

                default:
                    break;
            }

            if (args.Key == "Enter" && EatEnter)
            {
                EatEnter = false;
                return;
            }

            if (args.Key == "Enter")
            {
                if (args.ShiftKey && string.IsNullOrEmpty(NavString))
                {
                    await MyMenu.ExecMenu("NextBase");
                    await ClearNav();
                    return;
                }
                else if (args.ShiftKey && NavString == "-")
                {
                    await MyMenu.ExecMenu("PreviousBase");
                    return;
                }
                else if (NavString == "-")
                {
                    await MyMenu.ExecMenu("PreviousNote");
                    return;
                }

                else if (string.IsNullOrEmpty(NavString))
                {
                    await MyMenu.ExecMenu("NextNote");
                    await ClearNav();
                    return;
                }

                bool IsPlus = false;
                bool IsMinus = false;
                bool IsRespOnly = false;

                string stuff = NavString.Replace(";", "").Replace(" ", "");

                if (stuff.StartsWith("+"))
                    IsPlus = true;
                if (stuff.StartsWith("-"))
                    IsMinus = true;

                stuff = stuff.Replace("+", "").Replace("-", "");

                if (stuff.StartsWith('.'))
                {
                    await ClearNav();
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
                        await ClearNav();
                        EatEnter = true;
                        ShowMessage("Could not parse : " + parts[0]);
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
                                ShowMessage("Could not find note : " + NavString);
                            return;
                        }
                        long headerId = await Http.GetFromJsonAsync<long>("api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + noteNum + "/0");
                        if (headerId != 0)
                            Navigation.NavigateTo("notedisplay/" + headerId);
                        else
                            ShowMessage("Could not find note : " + NavString);
                        await ClearNav();
                        return;
                    }
                }
                else if (parts.Length == 2)
                {
                    if (!int.TryParse(parts[0], out noteNum))
                    {
                        ShowMessage("Could not parse : " + parts[0]);
                        EatEnter = true;
                    }
                    int noteRespOrd;
                    if (!int.TryParse(parts[1], out noteRespOrd))
                    {
                        ShowMessage("Could not parse : " + parts[1]);
                        EatEnter = true;
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
                                ShowMessage("Could not find note : " + NavString);

                        }
                        long headerId = await Http.GetFromJsonAsync<long>("api/GetNoteHeaderId/" + model.header.NoteFileId + "/" + noteNum + "/" + noteRespOrd);
                        if (headerId != 0)
                            Navigation.NavigateTo("notedisplay/" + headerId);
                        else
                            ShowMessage("Could not find note : " + NavString);
                    }
                    await ClearNav();
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

            await JSRuntime.InvokeVoidAsync("Prism.highlightAll");
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
