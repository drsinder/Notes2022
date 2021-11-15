using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Notes2022.RCL.User.Dialogs;
using Notes2022.Shared;
using Syncfusion.Blazor.RichTextEditor;
using System.Net.Http.Json;

namespace Notes2022.Client.Pages.User.Panels
{
    public partial class NoteEditor
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Parameter] public TextViewModel Model { get; set; }

        private bool ShowChild = false;
        private NoteFile noteFile { get; set; } = new NoteFile();
        private SfRichTextEditor EditObj { get; set; }
        private RichTextEditorToolbarSettings ToolBarObj { get; set; }

        protected string PreparedCode { get; set; }

        private List<ToolbarItemModel> Tools = new List<ToolbarItemModel>()
        {
            new ToolbarItemModel() { Command = ToolbarCommand.Undo },
            new ToolbarItemModel() { Command = ToolbarCommand.Redo },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },
            new ToolbarItemModel() { Command = ToolbarCommand.Bold },
            new ToolbarItemModel() { Command = ToolbarCommand.Italic },
            new ToolbarItemModel() { Command = ToolbarCommand.Underline },
            new ToolbarItemModel() { Command = ToolbarCommand.StrikeThrough },
            new ToolbarItemModel() { Command = ToolbarCommand.FontName },
            new ToolbarItemModel() { Command = ToolbarCommand.FontSize },
            new ToolbarItemModel() { Command = ToolbarCommand.FontColor },
            new ToolbarItemModel() { Command = ToolbarCommand.BackgroundColor },
            new ToolbarItemModel() { Command = ToolbarCommand.LowerCase },
            new ToolbarItemModel() { Command = ToolbarCommand.UpperCase },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },
            new ToolbarItemModel() { Command = ToolbarCommand.Formats },
            new ToolbarItemModel() { Command = ToolbarCommand.Alignments },
            new ToolbarItemModel() { Command = ToolbarCommand.OrderedList },
            new ToolbarItemModel() { Command = ToolbarCommand.UnorderedList },
            new ToolbarItemModel() { Command = ToolbarCommand.Outdent },
            new ToolbarItemModel() { Command = ToolbarCommand.Indent },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },
            new ToolbarItemModel() { Command = ToolbarCommand.CreateLink },
            new ToolbarItemModel() { Command = ToolbarCommand.Image },
            new ToolbarItemModel() { Command = ToolbarCommand.CreateTable },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },
            new ToolbarItemModel() { Command = ToolbarCommand.ClearFormat },
            new ToolbarItemModel() { Command = ToolbarCommand.Print },
            //new ToolbarItemModel() { Command = ToolbarCommand.InsertCode },
            new ToolbarItemModel() { Name = "PCode1", TooltipText = "Prepare Code for Insertion" },
            new ToolbarItemModel() { Name = "PCode", TooltipText = "Insert Prepared Code" },
            new ToolbarItemModel() { Command = ToolbarCommand.SourceCode },
            new ToolbarItemModel() { Command = ToolbarCommand.FullScreen }
        };

        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        public NoteEditor()
        {
        }

        protected async override Task OnParametersSetAsync()
        {
            if (Model.NoteFileID != 0)
                noteFile = await Http.GetFromJsonAsync<NoteFile>("api/NewNote/" + Model.NoteFileID);
        }

        protected async Task HandleValidSubmit()
        {
            if (string.IsNullOrEmpty(Model.MySubject))
            {
                ShowMessage("Please provide a note Subject");
                return;
            }
            //Model.MySubject = "*none*";  // must have title

            if (Model.NoteID == 0)    // new note
            {
                await Http.PostAsJsonAsync("api/NewNote/", Model);
                NoteHeader nh = await Http.GetFromJsonAsync<NoteHeader>("api/NewNote2");
                Navigation.NavigateTo("notedisplay/" + nh.Id);
                return;
            }
            else // editing
            {
                await Http.PutAsJsonAsync("api/NewNote/", Model);
                NoteHeader nh = await Http.GetFromJsonAsync<NoteHeader>("api/NewNote2");
                Navigation.NavigateTo("notedisplay/" + nh.Id);
                return;
            }
        }

        //public async Task OnToolbarClickHandler(ToolbarClickEventArgs args)
        //{
        //    if (args.Item.Id == "InsertCode")
        //    {
        //        string xx = await EditObj.GetSelectedHtmlAsync();
        //        if (xx != null && xx.Length > 0)
        //        {
        //            ShowMessage("Code can not be edited.  Please Copy, Delete, and Reinsert");
        //            return;
        //        }
        //        // get insertion point?? how??

        //        var parameters = new ModalParameters();
        //        parameters.Add("stuff", xx);
        //        parameters.Add("EditObj", EditObj);
        //        var formModal = Modal.Show<CodeFormat>("", parameters);
        //        var result = await formModal.Result;
        //        if (!result.Cancelled)
        //        {
        //            PreparedCode = (string)result.Data;
        //        }
        //    }
        //}

        public async Task InsertCode1()
        {
            string xx = await EditObj.GetSelectedHtmlAsync();
            if (xx != null && xx.Length > 0)
            {
                ShowMessage("Code can not be edited.  Please Copy, Delete, and Reinsert");
                return;
            }
            // get insertion point?? how??

            var parameters = new ModalParameters();
            parameters.Add("stuff", xx);
            parameters.Add("EditObj", EditObj);
            var formModal = Modal.Show<CodeFormat>("", parameters);
            var result = await formModal.Result;
            if (!result.Cancelled)
            {
                PreparedCode = (string)result.Data;
            }
        }

        public async Task InsertCode2()
        {
            if (!string.IsNullOrEmpty(PreparedCode))
                await EditObj.ExecuteCommandAsync(CommandName.InsertHTML, PreparedCode);
        }

        private void ShowMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("MessageInput", message);
            Modal.Show<MessageBox>("", parameters);
        }

        protected void CancelEdit()
        {
            Navigation.NavigateTo("noteindex/" + Model.NoteFileID);
        }

        protected void OnClickRef(MouseEventArgs args)
        {
            ShowChild = true;
        }

        private void OnClickRefHide(MouseEventArgs args)
        {
            ShowChild = false;
        }
    }
}
