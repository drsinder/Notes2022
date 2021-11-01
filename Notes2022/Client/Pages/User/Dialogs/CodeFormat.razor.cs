using Blazored.Modal;
using Blazored.Modal.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Notes2022.Client.Pages.User.Dialogs
{
    public partial class CodeFormat
    {
        [CascadingParameter] BlazoredModalInstance ModalInstance { get; set; }
        [Parameter] public string stuff { get; set; }

        private string stringChecked = "none";
        protected string message { get; set; }

        protected async override Task OnParametersSetAsync()
        {
            message = "<pre>" + stuff + "</pre>";
            //message = ((MarkupString)message).Value;
        }

        private void Ok()
        {
            switch (stringChecked)
            {
                case "none":
                    break;

                default:
                    stuff = MakeCode(stuff, stringChecked);
                    break;
            }
            ModalInstance.CloseAsync(ModalResult.Ok(stuff));
        }

        private string MakeCode (string stuff, string codeType)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<code class=\"language-");
            sb.Append(codeType);
            sb.Append("\">");
            sb.Append(stuff);
            sb.Append("</code>");

            return sb.ToString();
        }


        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }

    }
}
