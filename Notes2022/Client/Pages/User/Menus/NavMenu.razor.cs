using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Notes2022.Client.Pages.User.Dialogs;
using Notes2022.Shared;
using Syncfusion.Blazor.Navigations;
using System.Net.Http.Json;

namespace Notes2022.Client.Pages.User.Menus
{
    public partial class NavMenu
    {
        [CascadingParameter] public IModalService Modal { get; set; }

        protected static List<MenuItem> menuItemsTop { get; set; }
        protected SfMenu<MenuItem> topMenu { get; set; }

        private bool collapseNavMenu = true;

        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        [Inject] HttpClient Http { get; set; }
        [Inject] AuthenticationStateProvider AuthProv { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        //[Inject] Blazored.SessionStorage.ISessionStorageService sessionStorage { get; set; }
        public NavMenu()
        {
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }


        protected override async Task OnParametersSetAsync()
        {
            await UpdateMenu();
        }

        /// <summary>
        /// Enable only items available to logged in user
        /// </summary>
        /// <returns></returns>
        public async Task UpdateMenu()
        {

            bool isAdmin = false;
            bool isUser = false;
            AuthenticationState authstate = await AuthProv.GetAuthenticationStateAsync();
            if (authstate.User.Identity.IsAuthenticated)
            {
                try
                {
                    // session...

                    //Globals.EditUserVModel = await sessionStorage.GetItemAsync<EditUserViewModel>("EditUserView");

                    if (Globals.EditUserVModel == null)
                    {
                        UserData udata = await Http.GetFromJsonAsync<UserData>("api/User");
                        string uid = udata.UserId;
                        Globals.UserData = udata;

                        Globals.EditUserVModel = await Http.GetFromJsonAsync<EditUserViewModel>("api/UserEdit/" + uid);
                        //await sessionStorage.SetItemAsync("EditUserView", Globals.EditUserVModel);

                    }

                    if (Globals.RolesValid)
                        goto Found;

                    foreach (CheckedUser u in Globals.EditUserVModel.RolesList)
                    {
                        if (u.theRole.NormalizedName == "ADMIN" && u.isMember)
                        {
                            isUser = isAdmin = true;
                        }
                        if (u.theRole.NormalizedName == "USER" && u.isMember)
                        {
                            isUser = true;
                        }
                    }

                    Globals.IsAdmin = isAdmin;
                    Globals.IsUser = isUser;
                    Globals.RolesValid = true;

                Found:
                    isAdmin = Globals.IsAdmin;
                    isUser = Globals.IsUser;
                }
                catch (Exception e)
                {
                    ShowMessage("In NavMenu: " + e.Message);
                }

            }

            menuItemsTop = new List<MenuItem>();
            MenuItem item;

            item = new MenuItem() { Id = "Recent", Text = "Recent Notes" };
            menuItemsTop.Add(item);

            MenuItem item2 = new MenuItem() { Id = "MRecent", Text = "Recent" };
            MenuItem item2a = new MenuItem() { Id = "Subscriptions", Text = "Subscriptions" };
            MenuItem item2b = new MenuItem() { Id = "Preferences", Text = "Preferences" };

            MenuItem item3 = new MenuItem() { Id = "Manage", Text = "Manage" };
            item3.Items = new List<MenuItem>();
            item3.Items.Add(item2);
            item3.Items.Add(item2a);
            item3.Items.Add(item2b);
            menuItemsTop.Add(item3);


            item = new MenuItem() { Id = "Help", Text = "Help" };
            item.Items = new List<MenuItem>();
            MenuItem item4 = new MenuItem() { Id = "MainHelp", Text = "Help" };
            MenuItem item4a = new MenuItem() { Id = "About", Text = "About" };
            MenuItem item4b = new MenuItem() { Id = "License", Text = "License" };
            item.Items.Add(item4);
            item.Items.Add(item4b);
            item.Items.Add(item4a);
            menuItemsTop.Add(item);


            item = new MenuItem() { Id = "Admin", Text = "Admin" };
            item.Items = new List<MenuItem>();
            MenuItem item5 = new MenuItem() { Id = "NoteFiles", Text = "NoteFiles" };
            MenuItem item5a = new MenuItem() { Id = "Roles", Text = "Roles" };
            MenuItem item5b = new MenuItem() { Id = "Linked", Text = "Linked" };
            item.Items.Add(item5);
            item.Items.Add(item5a);
            item.Items.Add(item5b);
            menuItemsTop.Add(item);


            if (!isAdmin)
            {
                menuItemsTop.RemoveAt(3);
            }

            if (isUser || isAdmin)
            {
            }
            else
            {
                menuItemsTop.RemoveAt(1);
                menuItemsTop.RemoveAt(0);
            }

        }

        public async Task OnSelect(MenuEventArgs<MenuItem> e)
        {
            await ExecMenu(e.Item.Id);
        }

        private async Task ExecMenu(string id)
        {
            switch (id)
            {
                case "MainHelp":
                    Navigation.NavigateTo("help");
                    break;
                case "About":
                    Navigation.NavigateTo("about");
                    break;
                case "License":
                    Navigation.NavigateTo("license");
                    break;

                case "NoteFiles":
                    Navigation.NavigateTo("admin/notefilelist");
                    break;

                case "Preferences":
                    Navigation.NavigateTo("preferences");
                    break;
            }
        }

        private void ShowMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("MessageInput", message);
            Modal.Show<MessageBox>("Error", parameters);
        }
    }
}
