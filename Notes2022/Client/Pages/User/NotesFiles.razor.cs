using System.Net.Http.Json;
using Notes2022.Shared;
using Syncfusion.Blazor.Grids;

namespace Notes2022.Client.Pages.User
{
    public partial class NotesFiles
    {
        private List<NoteFile> Files { get; set; }

        private UserData UserData { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await sessionStorage.SetItemAsync<int>("ArcId", 0);
            await sessionStorage.SetItemAsync<int>("IndexPage", 1);
            HomePageModel model = await Http.GetFromJsonAsync<HomePageModel>("api/HomePageData");
            Files = model.NoteFiles;
            UserData = model.UserData;
            if (UserData.Ipref2 == 0)
                UserData.Ipref2 = 10;
        }

        protected void DisplayIt(RowSelectEventArgs<NoteFile> args)
        {
            Navigation.NavigateTo("noteindex/" + args.Data.Id);
        }
    }
}