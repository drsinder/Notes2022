using Blazored.Modal;
using Blazored.Modal.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Notes2022.RCL.Admin.Dialogs;
using Notes2022.Shared;
using System.Net.Http.Json;


namespace Notes2022.RCL.Admin
{
    public partial class UserList
    {
        [CascadingParameter] public IModalService Modal { get; set; }

        private List<UserData> UList { get; set; }

        [Inject] HttpClient Http { get; set; }
        [Inject] GrpcChannel Channel { get; set; }
        //[Inject] NavigationManager Navigation { get; set; }
        public UserList()
        { }

        protected override async Task OnParametersSetAsync()
        {
            UList = await DAL.GetUserList(Channel);
        }

        protected void EditLink(string Id)
        {
            ModalParameters Parameters = new ModalParameters();
            Parameters.Add("UserId", Id);

            Modal.Show<UserEdit>("", Parameters);
        }
    }
}
