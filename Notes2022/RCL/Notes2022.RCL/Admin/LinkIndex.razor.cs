using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Blazored;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.SessionStorage;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.LinearGauge;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.SplitButtons;
using Syncfusion.Blazor.Calendars;
using Notes2022.Shared;
using Notes2022.RCL;
using Notes2022.RCL.User;
using Notes2022.RCL.User.Dialogs;
using Notes2022.RCL.User.Panels;
using Notes2022.RCL.User.Menus;
using Notes2022.RCL.User.Comp;
using Notes2022.RCL.Admin.Dialogs;

namespace Notes2022.RCL.Admin
{
    public partial class LinkIndex
    {
        private List<LinkedFile> Model { get; set; }

        private int deleteId { get; set; }

        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] IModalService Modal { get; set; }
        public LinkIndex()
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            Model = await Http.GetFromJsonAsync<List<LinkedFile>>("api/Linked");
        }

        protected async Task Create()
        {
            var parameters = new ModalParameters();
            var x = Modal.Show<CreateLinked>("", parameters);
            await x.Result;
            Model = await Http.GetFromJsonAsync<List<LinkedFile>>("api/Linked");
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

            await Http.DeleteAsync("api/Linked/" + deleteId);

            Model = await Http.GetFromJsonAsync<List<LinkedFile>>("api/Linked");

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