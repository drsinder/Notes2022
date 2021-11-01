using Blazored.Modal;
using Blazored.Modal.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;

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
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(stuff);

            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//pre");

            foreach (var node in htmlNodes)
            {
                message = node.InnerHtml;
            }

        }

        private void Ok()
        {

            switch (stringChecked)
            {
                case "none": break;
                case "C#":
                    stuff = stuff.Replace("<pre>", "<pre><code class=\"language-csharp\">").Replace("</pre>", "</code></pre>");
                    break;

                case "Css":
                    stuff = stuff.Replace("<pre>", "<pre><code class=\"language-css\">").Replace("</pre>", "</code></pre>");
                    break;

                case "Javascript":
                    stuff = stuff.Replace("<pre>", "<pre><code class=\"language-js\">").Replace("</pre>", "</code></pre>");
                    break;

                case "Json":
                    stuff = stuff.Replace("<pre>", "<pre><code class=\"language-json\">").Replace("</pre>", "</code></pre>");
                    break;

                case "Razor":
                    stuff = stuff.Replace("<pre>", "<pre><code class=\"language-razor\">").Replace("</pre>", "</code></pre>");
                    break;

                case "Html":
                    stuff = stuff.Replace("<pre>", "<pre><code class=\"language-html\">").Replace("</pre>", "</code></pre>");
                    break;

                case "C++":
                    stuff = stuff.Replace("<pre>", "<pre><code class=\"language-cpp\">").Replace("</pre>", "</code></pre>");
                    break;

                case "C":
                    stuff = stuff.Replace("<pre>", "<pre><code class=\"language-c\">").Replace("</pre>", "</code></pre>");
                    break;


                default:
                    break;
            }


            ModalInstance.CloseAsync(ModalResult.Ok(stuff));
        }


        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }

    }
}
