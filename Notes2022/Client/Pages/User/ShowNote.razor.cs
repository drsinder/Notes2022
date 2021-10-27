using Microsoft.AspNetCore.Components;

namespace Notes2022.Client.Pages.User
{
    public partial class ShowNote
    {
        [Parameter] public long NoteId { get; set; }
    }

}
