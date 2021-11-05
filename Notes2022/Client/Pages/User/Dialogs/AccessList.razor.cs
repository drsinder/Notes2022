using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using Notes2022.Shared;
using Syncfusion.Blazor.Grids;
using System.Net.Http.Json;

namespace Notes2022.Client.Pages.User.Dialogs
{
    public partial class AccessList
    {
        [CascadingParameter] public BlazoredModalInstance ModalInstance { get; set; }
        [Parameter] public int fileId { get; set; }

        private SfGrid<NoteAccess> MyGrid;
        private List<NoteAccess> myList { get; set; }
        private List<NoteAccess> temp { get; set; }
        private List<UserData> userList { get; set; }
        private NoteAccess myAccess { get; set; }
        private int arcId { get; set; }

        private string message { get; set; }

        [Inject] HttpClient Http { get; set; }
        [Inject] Blazored.SessionStorage.ISessionStorageService sessionStorage { get; set; }
        public AccessList()
        {
        }

        protected async override Task OnParametersSetAsync()
        {
            arcId = await sessionStorage.GetItemAsync<int>("ArcId");

            temp = await Http.GetFromJsonAsync<List<NoteAccess>>("api/accesslist/" + fileId);
            myList = new List<NoteAccess>();

            foreach (NoteAccess item in temp)
            {
                if (item.ArchiveId == arcId)
                {
                    myList.Add(item);
                }
            }

            userList = await Http.GetFromJsonAsync<List<UserData>>("api/userlists");

            try
            {
                myAccess = await Http.GetFromJsonAsync<NoteAccess>("api/myaccess/" + fileId);
            }
            catch (Exception ex)
            {
                message += ex.Message; 
                myAccess = new NoteAccess();
            }
        }

        private void Cancel()
        {
            ModalInstance.CloseAsync();
        }

        protected void CreateNew()
        {
            //ModalService.Close(ModalResult.Ok<List<UserData>>(userList));
        }

        protected async Task ClickHandler(string newMessage)
        {
            arcId = await sessionStorage.GetItemAsync<int>("ArcId");

            temp = await Http.GetFromJsonAsync<List<NoteAccess>>("api/AccessList/" + fileId);
            myList = new List<NoteAccess>();

            foreach (NoteAccess item in temp)
            {
                if (item.ArchiveId == arcId)
                {
                    myList.Add(item);
                }
            }
            StateHasChanged();
            MyGrid.Refresh();
        }

    }
}
