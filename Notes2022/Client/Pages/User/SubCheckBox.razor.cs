using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Notes2022.Client;
using Notes2022.Client.Shared;
using Notes2022.RCL.User.Panels;
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
using Notes2022.Shared;
using System.Text;
using Newtonsoft.Json;

namespace Notes2022.Client.Pages.User
{
    public partial class SubCheckBox
    {
        [Parameter]
        public int fileId { get; set; }

        [Parameter]
        public bool isChecked { get; set; }

        public SCheckModel Model { get; set; }
        protected override void OnParametersSet()
        {
            Model = new SCheckModel
            {
                isChecked = isChecked,
                fileId = fileId
            };
        }

        public async Task OnClick()
        {
            isChecked = !isChecked;

            if (isChecked) // create item
            {
                await Http.PostAsJsonAsync("api/Subscription", Model);
            }
            else // delete it
            {
                await Http.DeleteAsync("api/Subscription/" + fileId);
            }

            StateHasChanged();
        }
    }
}