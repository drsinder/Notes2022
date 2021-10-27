using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Notes2022.Shared;
using System.Net.Http.Json;

namespace Notes2022.Client.Pages.User
{
    public partial class Index
    {
        private class localFile : NoteFile
        {
        }

        private localFile dummyFile = new localFile { Id = 0, NoteFileName = " ", NoteFileTitle = " " };
        private List<localFile> fileList { get; set; }
        private List<localFile> nameList { get; set; }
        private List<localFile> impfileList { get; set; }
        private List<localFile> histfileList { get; set; }


        //private bool isChecked = true;
        //private AboutModel? model { get; set; }
        //private TimeSpan upTime { get; set; }
        private HomePageModel? hpModel { get; set; }
        private DateTime mytime { get; set; }


        [Inject] HttpClient Http { get; set; }
        [Inject] AuthenticationStateProvider AuthProv { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        public Index()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            fileList = new List<localFile>();
            nameList = new List<localFile>();
            histfileList = new List<localFile>();
            impfileList = new List<localFile>();

            mytime = DateTime.Now;

            AuthenticationState authstate = await AuthProv.GetAuthenticationStateAsync();
            if (authstate.User.Identity.IsAuthenticated)
            {
                try
                {
                    //model = await Http.GetFromJsonAsync<AboutModel>("api/About");
                    //upTime = DateTime.Now.ToUniversalTime() - model.StartupDateTime;

                    hpModel = await Http.GetFromJsonAsync<HomePageModel>("api/HomePageData");


                    List<NoteFile> fileList1 = hpModel.NoteFiles.OrderBy(p => p.NoteFileName).ToList();
                    List<NoteFile> nameList1 = hpModel.NoteFiles.OrderBy(p => p.NoteFileTitle).ToList();
                    histfileList = new List<localFile>();
                    impfileList = new List<localFile>();


                    for (int i = 0; i < fileList1.Count; i++)
                    {
                        localFile work = new localFile { Id = fileList1[i].Id, NoteFileName = fileList1[i].NoteFileName, NoteFileTitle = fileList1[i].NoteFileTitle };
                        localFile work2 = new localFile { Id = nameList1[i].Id, NoteFileName = nameList1[i].NoteFileName, NoteFileTitle = nameList1[i].NoteFileTitle };
                        fileList.Add(work);
                        nameList.Add(work2);

                        string fname = work.NoteFileName;
                        if (fname == "Opbnotes" || fname == "Gnotes")
                            histfileList.Add(work);

                        if (fname == "announce" || fname == "pbnotes" || fname == "noteshelp")
                            impfileList.Add(work);
                    }
                }
                finally { }
            }
        }

        protected void TextHasChanged(string value)
        {
            value = value.Trim().Replace("'\n", "").Replace("'\r", "").Replace(" ", "");

            try
            {
                foreach (var item in fileList)
                {
                    if (value == item.NoteFileName)
                    {
                        Navigation.NavigateTo("/noteindex/" + item.Id);
                        return;
                    }
                }
            }
            catch { }
        }
    }
}
