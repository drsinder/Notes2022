﻿using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Notes2022.Client.Pages.User
{
    public partial class ShowNote
    {
        [Parameter] public long NoteId { get; set; }

        public int FileId { get; set; }

        [Inject] HttpClient Http { get; set; }
        public ShowNote()
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            // find the file id for this note - get note header

            FileId = await Http.GetFromJsonAsync<int>("api/GetFIleIdForNoteId/" + NoteId);
        }

    }

}
