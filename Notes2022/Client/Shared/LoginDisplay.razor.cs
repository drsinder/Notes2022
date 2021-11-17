using Notes2022.RCL;

namespace Notes2022.Client.Shared
{
    public partial class LoginDisplay
    {
        private async Task BeginSignOut()
        {
            Globals.RolesValid = false;
            await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("authentication/logout");
        }

        private void GotoProfile()
        {
            Navigation.NavigateTo("authentication/profile");
        }

        private void GotoRegister()
        {
            Navigation.NavigateTo("authentication/register");
        }

        private void GotoLogin()
        {
            Globals.RolesValid = false;
            Navigation.NavigateTo("authentication/login");
        }

        private void GotoHome()
        {
            Navigation.NavigateTo("");
        }
    }
}