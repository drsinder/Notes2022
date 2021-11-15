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
using Microsoft.AspNetCore.Identity;
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
using Notes2022.Shared;
using Notes2022.RCL;
using Notes2022.RCL.User;
using Notes2022.RCL.User.Dialogs;
using Notes2022.RCL.User.Panels;
using Notes2022.RCL.User.Menus;
using Notes2022.RCL.User.Comp;

namespace Notes2022.RCL.Admin.Dialogs
{
    public partial class UserEdit
    {
        [CascadingParameter] public BlazoredModalInstance ModalInstance { get; set; }

        [Parameter] public string UserId { get; set; }

        protected EditUserViewModel Model { get; set; }

        [Inject] HttpClient Http { get; set; }

        public UserEdit()
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            Model = await Http.GetFromJsonAsync<EditUserViewModel>("api/useredit/" + UserId);
        }

        private async Task Submit()
        {
            await Http.PutAsJsonAsync("api/useredit/", Model);

            await ModalInstance.CancelAsync();
        }


        private async Task Done()
        {
            await ModalInstance.CancelAsync();
        }


    }
}