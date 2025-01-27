﻿using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Notes2022.RCL;
using Notes2022.Shared;
using System.Net.Http.Json;

namespace Notes2022.Client.Pages.User
{
    public partial class FindFile
    {
        [Parameter] public string filename { get; set; }

        private HomePageModel? hpModel { get; set; }

        protected string message { get; set; }

        [Inject] HttpClient Http { get; set; }
        [Inject] GrpcChannel Channel { get; set; }
        [Inject] AuthenticationStateProvider AuthProv { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        public FindFile()
        {
        }

        protected override async Task OnParametersSetAsync()
        {

            AuthenticationState authstate = await AuthProv.GetAuthenticationStateAsync();
            if (authstate.User.Identity.IsAuthenticated)
            {
                hpModel = await DAL.GetHomePageData(Channel, Globals.UserData.Email);

                NoteFile nf = hpModel.NoteFiles.SingleOrDefault(p => p.NoteFileName == filename);
                if (nf is not null)
                {
                    Navigation.NavigateTo("noteindex/" + nf.Id);
                }
                else
                {
                    message = "Note File '" + filename + "' not found...";
                }

            }

        }
    }
}
