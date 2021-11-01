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
        }

        private void Ok()
        {
            switch (stringChecked)
            {
                case "none":
                    break;
                case "C#":
                    stuff = MakeCode(stuff, "csharp");
                    break;

                case "Css":
                    stuff = MakeCode(stuff, "css");
                    break;

                case "Javascript":
                    stuff = MakeCode(stuff, "js");
                    break;

                case "Json":
                    stuff = MakeCode(stuff, "json");
                    break;

                case "Razor":
                    stuff = MakeCode(stuff, "razor");
                    break;

                case "Html":
                    stuff = MakeCode(stuff, "html");
                    break;

                case "C++":
                    stuff = MakeCode(stuff, "cpp");
                    break;

                case "C":
                    stuff = MakeCode(stuff, "c");
                    break;


                default:
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
