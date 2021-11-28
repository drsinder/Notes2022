using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Notes2022.Shared;
using System.Net.Http.Json;

namespace Notes2022.RCL.User.Comp
{
    public partial class SubCheckBox
    {
        [Parameter]
        public int fileId { get; set; }

        [Parameter]
        public bool isChecked { get; set; }

        [Parameter]
        public string userId { get; set; }

        public SCheckModel Model { get; set; }

        [Inject] GrpcChannel Channel { get; set; }
        public SubCheckBox()
        { }

        protected override void OnParametersSet()
        {
            Model = new SCheckModel
            {
                isChecked = isChecked,
                fileId = fileId,
                userId = userId
            };
        }

        public async Task OnClick()
        {
            isChecked = !isChecked;

            if (isChecked) // create item
            {
                await DAL.CreateSubscription(Channel, Model);
            }
            else // delete it
            {
                await DAL.DeleteSubscription(Channel, Model);
            }

            StateHasChanged();
        }
    }
}