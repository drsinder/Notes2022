using System.Net.Http.Json;
using Notes2022.Shared;

namespace Notes2022.RCL.User
{
    public partial class Tracker
    {
        private List<NoteFile> stuff { get; set; }

        private List<NoteFile> files { get; set; }

        private List<Sequencer> trackers { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            trackers = await Http.GetFromJsonAsync<List<Sequencer>>("api/sequencer");
            HomePageModel model = await Http.GetFromJsonAsync<HomePageModel>("api/HomePageData");
            stuff = model.NoteFiles.OrderBy(p => p.NoteFileName).ToList();
            await Shuffle();
        }

        public async Task Shuffle()
        {
            files = new List<NoteFile>();

            trackers = await Http.GetFromJsonAsync<List<Sequencer>>("api/sequencer");
            if (trackers != null)
            {
                trackers = trackers.OrderBy(p => p.Ordinal).ToList();
                foreach (var tracker in trackers)
                {
                    files.Add(stuff.Find(p => p.Id == tracker.NoteFileId));
                }
            }
            foreach (var s in stuff)
            {
                if (files.Find(p => p.Id == s.Id) == null)
                    files.Add(s);
            }
            StateHasChanged();
        }

        private async Task Cancel()
        {
            NavMan.NavigateTo("");
        }
    }
}