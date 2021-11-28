using Blazored.Modal;
using Blazored.Modal.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Notes2022.RCL.Admin.Dialogs;
using Notes2022.RCL.User.Dialogs;
using Notes2022.Shared;
using System.Net.Http.Json;

namespace Notes2022.RCL.Admin
{
    public partial class LinkIndex
    {
        private List<LinkedFile> Model { get; set; }

        private int deleteId { get; set; }

        [Inject] HttpClient Http { get; set; }
        [Inject] GrpcChannel Channel { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] IModalService Modal { get; set; }
        public LinkIndex()
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            Model = await DAL.GetLinked(Channel);
        }

        protected async Task Create()
        {
            var parameters = new ModalParameters();
            var x = Modal.Show<CreateLinked>("", parameters);
            await x.Result;
            Model = await DAL.GetLinked(Channel);
            Navigation.NavigateTo("admin/linkindex");
        }

        protected void DeleteLink(int id)
        {
            deleteId = id;
            Confirm();
        }

        protected void EditLink(int id)
        {

        }

        private async Task Confirm()
        {
            if (!await YesNo("Are you sure you want to delete this Linked file?"))
                return;

            await DAL.DeleteLinked(Channel, deleteId.ToString());

            Model = await DAL.GetLinked(Channel);

            StateHasChanged();
        }

        private async Task<bool> YesNo(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("MessageInput", message);
            var formModal = Modal.Show<YesNo>("", parameters);
            var result = await formModal.Result;
            return !result.Cancelled;
        }
    }
}