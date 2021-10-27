using Microsoft.AspNetCore.Components;
using Notes2022.Client.Pages.User.Menus;
using Notes2022.Client.Shared;
using Notes2022.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using System.Net.Http.Json;
using System.Timers;

using Blazored.Modal.Services;
using Blazored.Modal;
using Notes2022.Client.Pages.User.Dialogs;
using Microsoft.AspNetCore.Components.Web;

namespace Notes2022.Client.Pages.User
{
    public partial class NoteIndex
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Parameter] public int NotesfileId { get; set; }

        protected ListMenu MyMenu { get; set; }
        public string NavString { get; set; }
        protected string stuff { get; set; }

        protected SfTextBox sfTextBox { get; set; }
        protected string NavCurrentVal { get; set; }

        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        public NoteIndex()
        {
        }

        public NoteDisplayIndexModel Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Model = await Http.GetFromJsonAsync<NoteDisplayIndexModel>("api/NoteIndex/" + NotesfileId);
            Model.Notes = Model.AllNotes.FindAll(p => p.ResponseOrdinal == 0).OrderBy(p => p.NoteOrdinal).ToList();
        }

        protected void DisplayIt(RowSelectEventArgs<NoteHeader> args)
        {
            Navigation.NavigateTo("notedisplay/" + args.Data.Id);
        }

        private async Task KeyPressHandler(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                stuff = NavCurrentVal.Replace(";", "").Replace(" ", "");

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
                        //sfTextBox.Enabled = false;
                        myTimer.Enabled = true;
                    }
                    else
                    {
                        LongWrapper wrapper = await Http.GetFromJsonAsync<LongWrapper>("api/GetNoteHeaderId/" + NotesfileId + "/" + noteNum + "/0");
                        long headerId = wrapper.mylong;
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
                        LongWrapper wrapper = await Http.GetFromJsonAsync<LongWrapper>("api/GetNoteHeaderId/" + NotesfileId + "/" + noteNum + "/" + noteRespOrd);
                        long headerId = wrapper.mylong;
                        if (headerId != 0)
                            Navigation.NavigateTo("notedisplay/" + headerId);
                        else
                            ShowMessage("Could not find note : " + stuff);
                    }
                }
            }
        }

        private async void NavInputHandler(InputEventArgs args)
        {
            string IdString = args.Value;
            NavCurrentVal = IdString;
            stuff = IdString;

            switch (stuff)
            {
                case "L":
                    await MyMenu.ExecMenu("ListNoteFiles");
                    return;

                case "N":
                    await MyMenu.ExecMenu("NewBaseNote");
                    return;

                case "X":
                    await MyMenu.ExecMenu("eXport");
                    return;

                case "J":
                    await MyMenu.ExecMenu("JsonExport");
                    return;

                case "m":
                    await MyMenu.ExecMenu("mailFromIndex");
                    return;

                case "P":
                    await MyMenu.ExecMenu("PrintFile");
                    return;

                case "Z":
                    Modal.Show<HelpDialog>();
                    return;

                case "H":
                    await MyMenu.ExecMenu("HtmlFromIndex");
                    return;

                case "h":
                    await MyMenu.ExecMenu("htmlFromIndex");
                    return;

                case "R":
                    await MyMenu.ExecMenu("ReloadIndex");
                    return;



                default:
                    break;
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
            myTimer.Enabled = false;
            myTimer.Stop();
            //myTimer.Elapsed -= TimeUp;
            sfTextBox.Enabled = true;
            sfTextBox.FocusAsync();
        }

    }
}
