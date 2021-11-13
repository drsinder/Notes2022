using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Notes2022.Client.Pages.User.Dialogs;
using Notes2022.Client.Pages.User.Menus;
using Notes2022.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using System.Net.Http.Json;

namespace Notes2022.Client.Pages.User
{
    public partial class NoteIndex
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Parameter] public int NotesfileId { get; set; }

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
            Navigation.NavigateTo("notedisplay/" + args.Data.Id);
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
                    await sessionStorage.SetItemAsync<bool>("IsSeq", false);
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

                await sessionStorage.SetItemAsync<int>("SeqIndex", seqIndex);

                Navigation.NavigateTo("noteindex/" + (-currSeq.NoteFileId));
                return;
            }

            await sessionStorage.SetItemAsync<List<NoteHeader>>("SeqHeaders", noteHeaders);
            await sessionStorage.SetItemAsync<int>("SeqHeaderIndex", 0);

            NoteHeader currHeader = noteHeaders[0];

            await sessionStorage.SetItemAsync<NoteHeader>("CurrentSeqHeader", currHeader);

            seq.Active = true;

            await Http.PutAsJsonAsync("api/sequencer", seq);

            Navigation.NavigateTo("notedisplay/" + currHeader.Id);
        }

        public async void ActionCompleteHandler(ActionEventArgs<NoteHeader> args)
        {
            await sessionStorage.SetItemAsync<int>("IndexPage", sfGrid1.PageSettings.CurrentPage);
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
                                Navigation.NavigateTo("notedisplay/" + headerId);
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
                                Navigation.NavigateTo("notedisplay/" + headerId);
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
