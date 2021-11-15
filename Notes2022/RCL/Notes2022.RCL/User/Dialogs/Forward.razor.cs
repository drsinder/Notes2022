using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using Notes2022.Shared;
using System.Net.Http.Json;

namespace Notes2022.RCL.User.Dialogs
{
    public partial class Forward
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public ForwardViewModel ForwardView { get; set; }

        private async Task Forwardit()
        {
            if (ForwardView.ToEmail == null || ForwardView.ToEmail.Length < 8 || !ForwardView.ToEmail.Contains("@") || !ForwardView.ToEmail.Contains("."))
                return;
            HttpResponseMessage result = await Http.PostAsJsonAsync("api/Forward/", ForwardView);
            await ModalInstance.CancelAsync();
        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }
    }
}