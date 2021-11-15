using Microsoft.AspNetCore.Components;
using Notes2022.Shared;
using Syncfusion.Blazor.Grids;

namespace Notes2022.RCL.User.Panels
{
    public partial class Responses
    {
        [Parameter] public List<NoteHeader> Headers { get; set; }
        [Parameter] public bool ShowContentR { get; set; }
        [Parameter] public bool ExpandAllR { get; set; }

        public bool ShowContent { get; set; }
        public bool ExpandAll { get; set; }


        protected SfGrid<NoteHeader> sfGrid2 { get; set; }

        [Inject] NavigationManager Navigation { get; set; }
        public Responses()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            ShowContent = ShowContentR;
            ExpandAll = ExpandAllR;
        }

        public void DataBoundHandler()
        {
            if (ExpandAll)
            {
                sfGrid2.ExpandAllDetailRowAsync();
            }
        }

        private async void ExpandAllChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
        {
            if (ExpandAll)
            {
                await sfGrid2.ExpandAllDetailRowAsync();
            }
            else
            {
                await sfGrid2.CollapseAllDetailRowAsync();
            }
        }
        protected void DisplayIt(RowSelectEventArgs<NoteHeader> args)
        {
            Navigation.NavigateTo("notedisplay/" + args.Data.Id);
        }
    }
}
