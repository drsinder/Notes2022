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
using Notes2022.Shared;
using Grpc.Net.Client;

namespace Notes2022.RCL.User
{
    public partial class About2
    {
        private AboutModel model { get; set; }

        private TimeSpan upTime { get; set; }

        [Inject] GrpcChannel Channel { get; set; }
        public About2()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                model = await DAL.GetAboutModel(Channel);
                upTime = DateTime.Now.ToUniversalTime() - model.StartupDateTime;
            }
            finally
            {
            }
        }
    }
}