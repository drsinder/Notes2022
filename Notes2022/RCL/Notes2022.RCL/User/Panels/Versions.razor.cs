using Microsoft.AspNetCore.Components;
using Notes2022.Shared;
using System.Net.Http.Json;

namespace Notes2022.RCL.User.Panels
{
    public partial class Versions
    {
        [Parameter] public int FileId { get; set; }
        [Parameter] public int NoteOrdinal { get; set; }
        [Parameter] public int ResponseOrdinal { get; set; }
        [Parameter] public int ArcId { get; set; }


        protected List<NoteHeader> Headers { get; set; }

        [Inject] HttpClient Http { get; set; }
        public Versions()
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            Headers = await Http.GetFromJsonAsync<List<NoteHeader>>("api/Versions/" + FileId + "/"
                + NoteOrdinal + "/" + ResponseOrdinal + "/" + ArcId);
        }
    }
}
