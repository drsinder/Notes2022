using Microsoft.AspNetCore.Components;
using Blazored.Modal;
using Blazored.Modal.Services;

namespace Notes2022.Client.Pages.User.Dialogs
{
    public partial class Email
    {
        [CascadingParameter]
        public BlazoredModalInstance ModalInstance { get; set; }

        public string emailaddr { get; set; }

        private void Ok()
        {
            ModalInstance.CloseAsync(ModalResult.Ok(emailaddr));
        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }
    }
}