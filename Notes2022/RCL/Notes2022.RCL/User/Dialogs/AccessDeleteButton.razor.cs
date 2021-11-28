using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Notes2022.Shared;

namespace Notes2022.RCL.User.Dialogs
{
    public partial class AccessDeleteButton
    {
        [Parameter]
        public NoteAccess noteAccess { get; set; }

        [Parameter]
        public EventCallback<string> OnClick { get; set; }

        [Inject] GrpcChannel Channel { get; set; }

        public AccessDeleteButton()
        {
        }

        protected async Task Delete()
        {
            UpdateAccessRequest item = new UpdateAccessRequest();
            item.item = noteAccess;
            item.eMail = Globals.UserData.UserId;


            await DAL.DeleteAccessItem(Channel, item);
            await OnClick.InvokeAsync("Delete");
        }
    }
}