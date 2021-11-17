/*--------------------------------------------------------------------------
    **
    **Copyright © 2022, Dale Sinder
    **
    **  Name: NoteIndex.razor
    **
    ** Description: Displays the main file index grid
    **     Base notes and expands to show responses
    **
    **  This program is free software: you can redistribute it and/or modify
    **  it under the terms of the GNU General Public License version 3 as
    **  published by the Free Software Foundation.
    **
    **  This program is distributed in the hope that it will be useful,
    **  but WITHOUT ANY WARRANTY; without even the implied warranty of
    **  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    **  GNU General Public License version 3 for more details.
    **
    **  You should have received a copy of the GNU General Public License
    **  version 3 along with this program in file "license-gpl-3.0.txt".
    **  If not, see <http://www.gnu.org/licenses/gpl-3.0.txt>.
    **
    **--------------------------------------------------------------------------*/

using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Notes2022.RCL.User.Dialogs;
using Notes2022.RCL.User.Menus;
using Notes2022.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using System.Net.Http.Json;
using SearchOption = Notes2022.Shared.SearchOption;

namespace Notes2022.RCL.User
{
    public partial class NoteIndex
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Parameter] public int NotesfileId { get; set; }
        [Parameter] public long CurrentNoteId { get; set; }

        protected ListMenu MyMenu { get; set; }

        public string NavString { get; set; }

        protected SfTextBox sfTextBox { get; set; }

        protected SfGrid<NoteHeader> sfGrid1 { get; set; }
        protected GridFilterSettings FilterSettings { get; set; }
        protected GridPageSettings PageSettings { get; set; }


        protected int PageSize { get; set; }
        protected int CurPage { get; set; }

        protected bool ShowContent { get; set; }
        protected bool ShowContentR { get; set; }
        protected bool ExpandAll { get; set; }

        protected bool Tdone { get; set; }

        protected bool IsSeq { get; set; }

        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] Blazored.SessionStorage.ISessionStorageService sessionStorage { get; set; }
        public NoteIndex()
        {
        }

        public NoteDisplayIndexModel Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await sessionStorage.SetItemAsync<bool>("InSearch", false);
            await sessionStorage.RemoveItemAsync("SearchIndex");
            await sessionStorage.RemoveItemAsync("SearchList");

            IsSeq = await sessionStorage.GetItemAsync<bool>("IsSeq");
            if (IsSeq && NotesfileId < 0)
            {
                NotesfileId = -NotesfileId;
            }

            Model = await Http.GetFromJsonAsync<NoteDisplayIndexModel>("api/NoteIndex/" + NotesfileId);
            PageSize = Model.UserData.Ipref2;
            ShowContent = Model.UserData.Pref7;
            ExpandAll = Model.UserData.Pref3;

            CurPage = await sessionStorage.GetItemAsync<int>("IndexPage");

            if (IsSeq)
                await StartSeq();
        }

        protected void DisplayIt(RowSelectEventArgs<NoteHeader> args)
        {
            CurrentNoteId = args.Data.Id;
            StateHasChanged();

            //Navigation.NavigateTo("notedisplay/" + args.Data.Id);
        }

        public void GotoNote(long Id)
        {
            CurrentNoteId = Id;
            StateHasChanged();
        }

        public long GetNextBaseNote(NoteHeader oh)
        {
            long newId = 0;
            NoteHeader nh =Model.Notes.SingleOrDefault(p => p.NoteFileId == oh.NoteFileId && p.NoteOrdinal == oh.NoteOrdinal + 1 && p.ResponseOrdinal == 0 && p.Version == 0);
            if (nh != null)
                newId = nh.Id;
            return newId;
        }

        public long GetNextNote(NoteHeader oh)
        {
            long newId = 0;

            NoteHeader nh = null;
            nh = Model.AllNotes.SingleOrDefault(p => p.NoteFileId == oh.NoteFileId && p.NoteOrdinal == oh.NoteOrdinal && p.ResponseOrdinal == (oh.ResponseOrdinal + 1) && p.Version == 0);

            if (nh == null)
                nh = Model.AllNotes.SingleOrDefault(p => p.NoteFileId == oh.NoteFileId && p.NoteOrdinal == (oh.NoteOrdinal + 1) && p.ResponseOrdinal == 0 && p.Version == 0);

            if (nh != null)
                newId = nh.Id;

            return newId;
        }

        public long GetPreviousBaseNote(NoteHeader oh)
        {
            long newId = 0;
            NoteHeader nh = Model.Notes.SingleOrDefault(p => p.NoteFileId == oh.NoteFileId && p.NoteOrdinal == oh.NoteOrdinal - 1 && p.ResponseOrdinal == 0 && p.Version == 0);
            if (nh != null)
                newId = nh.Id;
            return newId;
        }

        public long GetPreviousNote(NoteHeader oh)
        {
            long newId = 0;

            NoteHeader nh = null;
            nh = Model.AllNotes.SingleOrDefault(p => p.NoteFileId == oh.NoteFileId && p.NoteOrdinal == oh.NoteOrdinal && p.ResponseOrdinal == oh.ResponseOrdinal - 1 && p.Version == 0);

            if (nh == null)
                nh = Model.Notes.SingleOrDefault(p => p.NoteFileId == oh.NoteFileId && p.NoteOrdinal == oh.NoteOrdinal - 1 && p.ResponseOrdinal == 0 && p.Version == 0);

            if (nh != null)
                newId = nh.Id;

            return newId;
        }

        public void Listing()
        {
            CurrentNoteId = 0;
            StateHasChanged();
        }

        private List<NoteHeader> results { get; set; }
        private bool isSearch { get; set; }
        private long mode { get; set; }


        public async Task StartSearch(Search target)
        {
            //message = "Searching... Please Wait...";
            //StateHasChanged();

            switch (target.Option)
            {
                case SearchOption.Author:
                case SearchOption.Title:
                case SearchOption.TimeIsAfter:
                case SearchOption.TimeIsBefore:
                    await SearchHeader(target);
                    break;

                case SearchOption.Content:
                case SearchOption.DirMess:
                    await SearchContents(target);
                    break;

                //case SearchOption.Tag:
                //    await SearchTags(target);
                //    break;

                default:
                    break;
            }

            //message = null;
            //StateHasChanged();
        }

        protected async Task SearchHeader(Search target)
        {
            results = new List<NoteHeader>();
            List<NoteHeader> lookin = Model.AllNotes;

            foreach (NoteHeader nh in lookin)
            {
                bool isMatch = false;
                switch (target.Option)
                {
                    case SearchOption.Author:
                        isMatch = nh.AuthorName.Contains(target.Text);
                        break;
                    case SearchOption.Title:
                        isMatch = nh.NoteSubject.ToLower().Contains(target.Text.ToLower());
                        break;
                    case SearchOption.TimeIsAfter:
                        isMatch = DateTime.Compare(nh.LastEdited, target.Time) > 0;
                        break;
                    case SearchOption.TimeIsBefore:
                        isMatch = DateTime.Compare(nh.LastEdited, target.Time) < 0;
                        break;
                }
                if (isMatch)
                    results.Add(nh);
            }

            if (results.Count == 0)
            {
                ShowMessage("Nothing Found.");
                return;
            }

            results = results.OrderBy(p => p.LastEdited).ToList();

            mode = results[0].Id;
            isSearch = true;

            await sessionStorage.SetItemAsync<bool>("InSearch", true);
            await sessionStorage.SetItemAsync<int>("SearchIndex", 0);
            await sessionStorage.SetItemAsync<List<NoteHeader>>("SearchList", results);

            CurrentNoteId = mode;
            StateHasChanged();

            //Navigation.NavigateTo("notedisplay/" + mode);
        }

        protected async Task SearchContents(Search target)
        {
            results = new List<NoteHeader>();
            List<NoteHeader> lookin = Model.AllNotes;

            foreach (NoteHeader nh in lookin)
            {
                DisplayModel dm = await Http.GetFromJsonAsync<DisplayModel>("api/NoteContent/" + nh.Id);
                NoteContent nc = dm.content;
                List<Tags> tags = dm.tags;

                bool isMatch = false;
                switch (target.Option)
                {
                    case SearchOption.Content:
                        isMatch = nc.NoteBody.ToLower().Contains(target.Text.ToLower());
                        break;
                    case SearchOption.DirMess:
                        isMatch = nc.DirectorMessage.ToLower().Contains(target.Text.ToLower());
                        break;
                }
                if (isMatch)
                    results.Add(nh);
            }

            if (results.Count == 0)
            {
                ShowMessage("Nothing Found.");
                return;
            }

            results = results.OrderBy(p => p.LastEdited).ToList();

            mode = results[0].Id;

            await sessionStorage.SetItemAsync<bool>("InSearch", true);
            await sessionStorage.SetItemAsync<int>("SearchIndex", 0);
            await sessionStorage.SetItemAsync<List<NoteHeader>>("SearchList", results);

            CurrentNoteId = mode;
            StateHasChanged();

            //Navigation.NavigateTo("notedisplay/" + mode);
        }



        protected async Task StartSeq()
        {
            Sequencer seq = await sessionStorage.GetItemAsync<Sequencer>("SeqItem");
            if (seq == null)
                return;

            List<NoteHeader> noteHeaders = Model.AllNotes.FindAll(p => p.LastEdited >= seq.LastTime
                && p.IsDeleted == false && p.Version == 0)
                .OrderBy(p => p.NoteOrdinal)
                .ThenBy(p => p.ResponseOrdinal)
                .ToList();

            if (noteHeaders.Count == 0)
            {
                List<Sequencer> sequencers = await sessionStorage.GetItemAsync<List<Sequencer>>("SeqList");
                int seqIndex = await sessionStorage.GetItemAsync<int>("SeqIndex");
                if (sequencers.Count <= ++seqIndex)
                {
                    await sessionStorage.SetItemAsync("IsSeq", false);
                    await sessionStorage.RemoveItemAsync("SeqList");
                    await sessionStorage.RemoveItemAsync("SeqItem");
                    await sessionStorage.RemoveItemAsync("SeqIndex");

                    await sessionStorage.RemoveItemAsync("SeqHeaders");
                    await sessionStorage.RemoveItemAsync("SeqHeaderIndex");
                    await sessionStorage.RemoveItemAsync("CurrentSeqHeader");

                    ShowMessage("You have seen all the new notes!");

                    Navigation.NavigateTo("");

                    return;  // end it all
                }

                Sequencer currSeq = sequencers[seqIndex];

                await sessionStorage.SetItemAsync("SeqIndex", seqIndex);

                Navigation.NavigateTo("noteindex/" + -currSeq.NoteFileId);
                return;
            }

            await sessionStorage.SetItemAsync("SeqHeaders", noteHeaders);
            await sessionStorage.SetItemAsync("SeqHeaderIndex", 0);

            NoteHeader currHeader = noteHeaders[0];

            await sessionStorage.SetItemAsync("CurrentSeqHeader", currHeader);

            seq.Active = true;

            await Http.PutAsJsonAsync("api/sequencer", seq);

            CurrentNoteId = currHeader.Id;
            StateHasChanged();

            //Navigation.NavigateTo("notedisplay/" + currHeader.Id);
        }

        public async void ActionCompleteHandler(ActionEventArgs<NoteHeader> args)
        {
            await sessionStorage.SetItemAsync("IndexPage", sfGrid1.PageSettings.CurrentPage);
        }

        private async Task KeyUpHandler(KeyboardEventArgs args)
        {

            switch (NavString)
            {
                case "L":
                    await ClearNav();
                    await MyMenu.ExecMenu("ListNoteFiles");
                    return;

                case "N":
                    await ClearNav();
                    await MyMenu.ExecMenu("NewBaseNote");
                    return;

                case "X":
                    await ClearNav();
                    await MyMenu.ExecMenu("eXport");
                    return;

                case "J":
                    await ClearNav();
                    await MyMenu.ExecMenu("JsonExport");
                    return;

                case "m":
                    await ClearNav();
                    await MyMenu.ExecMenu("mailFromIndex");
                    return;

                case "P":
                    await ClearNav();
                    await MyMenu.ExecMenu("PrintFile");
                    return;

                case "Z":
                    await ClearNav();
                    Modal.Show<HelpDialog>();
                    return;

                case "H":
                    await ClearNav();
                    await MyMenu.ExecMenu("HtmlFromIndex");
                    return;

                case "h":
                    await ClearNav();
                    await MyMenu.ExecMenu("htmlFromIndex");
                    return;

                case "R":
                    await ClearNav();
                    await MyMenu.ExecMenu("ReloadIndex");
                    return;

                case "A":
                    await ClearNav();
                    await MyMenu.ExecMenu("AccessControls");
                    return;

                case "S":
                    await ClearNav();
                    await MyMenu.ExecMenu("SearchFromIndex");
                    return;

                default:
                    break;
            }

            if (args.Key == "Enter")
            {
                if (!string.IsNullOrEmpty(NavString))
                {
                    string stuff = NavString.Replace(";", "").Replace(" ", "");

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
                        }
                        else
                        {
                            long headerId = await Http.GetFromJsonAsync<long>("api/GetNoteHeaderId/" + NotesfileId + "/" + noteNum + "/0");
                            if (headerId != 0)
                            {
                                CurrentNoteId = headerId;
                                StateHasChanged();
                                return;

                                //Navigation.NavigateTo("notedisplay/" + headerId);
                            }
                            else
                                ShowMessage("Could not find note : " + stuff);
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
                            long headerId = await Http.GetFromJsonAsync<long>("api/GetNoteHeaderId/" + NotesfileId + "/" + noteNum + "/" + noteRespOrd);
                            if (headerId != 0)
                            {
                                CurrentNoteId = headerId;
                                StateHasChanged();
                                return;

                                //Navigation.NavigateTo("notedisplay/" + headerId);
                            }
                            else
                                ShowMessage("Could not find note : " + stuff);
                        }
                    }
                    await ClearNav();
                }
            }
        }

        private async void NavInputHandler(InputEventArgs args)
        {
            NavString = args.Value;
            await Task.CompletedTask;
        }

        private async Task ClearNav()
        {
            NavString = null;

            await Task.CompletedTask;
        }

        private async void ExpandAllChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            if (ExpandAll)
            {
                await sfGrid1.ExpandAllDetailRowAsync();
            }
            else
            {
                await sfGrid1.CollapseAllDetailRowAsync();
            }
        }

        private void ShowMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("MessageInput", message);
            Modal.Show<MessageBox>("", parameters);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender && !Tdone)
            {   // have to wait a bit before putting focus in textbox

                await Task.Delay(300);

                if (ExpandAll)
                    await sfGrid1.ExpandAllDetailRowAsync();

                if (sfTextBox != null)
                    await sfTextBox.FocusAsync();

                Tdone = true;
            }
            else
            {
            }
        }
    }
}
