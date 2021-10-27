using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Notes2022.Client.Shared;
using Notes2022.Shared;
using Syncfusion.Blazor.Grids;
using System.Net.Http.Json;



namespace Notes2022.Client.Pages.User
{
    public partial class NoteIndex
    {
        [Parameter] public int NotesfileId { get; set; }

        protected ListMenu MyMenu { get; set; }

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
            Navigation.NavigateTo("/notedisplay/" + args.Data.Id);
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    base.OnAfterRenderAsync(firstRender);

        //    if (!firstRender )
        //        await MyMenu.ExecMenu("PrintFile");
        //}

    }
}
