using System.Net.Http.Json;
using Notes2022.Shared;

namespace Notes2022.Client.Pages.User
{
    public partial class Tracker
    {
        private List<string> todo { get; set; }

        private List<NoteFile> files { get; set; }

        private List<Sequencer> trackers { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            trackers = await Http.GetFromJsonAsync<List<Sequencer>>("api/sequencer");
            HomePageModel model = await Http.GetFromJsonAsync<HomePageModel>("api/HomePageData");
            files = model.NoteFiles;
        }

        private async Task Cancel()
        {
            NavMan.NavigateTo("");
        }
    }
}