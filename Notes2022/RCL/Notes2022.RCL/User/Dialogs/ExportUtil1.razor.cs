﻿using Blazored.Modal;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Notes2022.Shared;
using System.Net.Http.Json;
using System.Text;

namespace Notes2022.RCL.User.Dialogs
{
    public partial class ExportUtil1
    {
        [CascadingParameter] public BlazoredModalInstance ModalInstance { get; set; }
        [Parameter] public ExportViewModel model { get; set; }
        [Parameter] public string FileName { get; set; }

        private IJSObjectReference? module;

        private bool marked { get; set; }
        private string message = "Getting ready...";

        [Inject] HttpClient Http { get; set; }
        [Inject] GrpcChannel Channel { get; set; }
        public ExportUtil1()
        {
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import",
                    "./scripts.js");
            }
        }

        //public async ValueTask<string?> Prompt(string message) =>
        //    module is not null ?
        //        await module.InvokeAsync<string>("showPrompt", message) : null;

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }
        }
        protected async override Task OnInitializedAsync()
        {

            if (model.Marks is not null && model.Marks.Count > 0)
                marked = true;
            else
                marked = false;

            if (model.isDirectOutput)
            {
                message = "Exporting " + FileName + " ... ";
            }
            else
            {
                message = "Emailing ... ";
            }

            MemoryStream ms = await DoExport();

            if (model.isDirectOutput)
            {
                await SaveAs(FileName, ms.GetBuffer());
            }
            else
            {
                byte[] bytes = ms.GetBuffer();

                System.Text.Encoding enc = System.Text.Encoding.ASCII;
                string email = enc.GetString(bytes);

                EmailModel stuff = new EmailModel();
                stuff.email = model.Email;
                stuff.subject = "Notes 2022 - " + model.NoteFile.NoteFileTitle;
                stuff.payload = email;

                await DAL.SendEmail(Channel, stuff);
            }

            ms.Dispose();

            await ModalInstance.CancelAsync();
        }

        private async Task<MemoryStream> DoExport()
        {
            bool isHtml = model.isHtml;
            bool isCollapsible = model.isCollapsible;

            NoteFile nf = model.NoteFile;
            int nfid = nf.Id;

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            StringBuilder sb = new StringBuilder();

            List<Tags> tags = new List<Tags>(); //await Http.GetFromJsonAsync<List<Tags>>("api/Tags/" + nfid);

            if (isHtml)
            {
                // Start the document
                sb.AppendLine("<!DOCTYPE html>");
                sb.AppendLine("<html>");
                sb.AppendLine("<meta charset=\"utf-8\" />");
                sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                sb.AppendLine("<title>" + nf.NoteFileTitle + "</title>");

                sb.AppendLine("<link rel = \"stylesheet\" href = \"https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css\">");
                sb.AppendLine("<link href=\"https://www.drsinder.com/NotesStuff/Notes2022/NotesExport.css\" rel=\"stylesheet\" />");
                sb.AppendLine("<link href=\"https://www.drsinder.com/NotesStuff/Notes2022/prism.css\" rel=\"stylesheet\" />");


                sb.AppendLine("<script src = \"https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js\" ></script >");
                sb.AppendLine("<script src = \"https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js\" ></script >");
                sb.AppendLine("<script src = \"https://www.drsinder.com/NotesStuff/Notes2022/prism.min.js\" ></script >");

                sb.AppendLine("</head>");
                sb.AppendLine("<body><div>");
                await sw.WriteAsync(sb.ToString());

                // ready to start  writing content of file
                sb = new StringBuilder();
            }
            if (isHtml)
                sb.Append("<h2>");

            // File Header
            sb.Append("2022 NoteFile " + nf.NoteFileName + " - " + nf.NoteFileTitle);
            sb.Append(" - Created " + DateTime.Now.ToUniversalTime().ToLongDateString() + " " + DateTime.Now.ToUniversalTime().ToShortTimeString());
            if (isHtml)
            {
                sb.Append("</h2>");
                //sb.Append("<h4>");
                //sb.Append("<a href=\"");
                //sb.Append(/*ProdUri + */"/NoteDisplay/Create/" + nf.Id +
                //          "\" target=\"_blank\">New Base Note</a>");
                //sb.Append("</h4>");
            }

            await sw.WriteLineAsync(sb.ToString());
            await sw.WriteLineAsync();

            // get ordered list of basenoteheaders to start process
            List<NoteHeader> bnhl = null;
            //string req;
            int NoteOrd = 0;
            if (model.NoteOrdinal == 0)
            {
                //req = "" + nfid + "." + model.ArchiveNumber + ".0.0";
            }
            else
            {

                //req = "" + nfid + "." + model.ArchiveNumber + "." + model.NoteOrdinal + ".0";
                NoteOrd = model.NoteOrdinal;
            }
            bnhl = await DAL.GetExport(Http, nfid, model.ArchiveNumber, NoteOrd, 0);

            // loop over each base note in order
            foreach (NoteHeader bnh in bnhl)
            {
                if (bnh.IsDeleted || bnh.Version > 0)
                    continue;

                if (marked && !model.Marks.Where(p => p.NoteOrdinal == bnh.NoteOrdinal).Any())
                    continue;

                // get content for base note
                NoteContent nc = null;
                //req = bnh.Id.ToString();
                nc = await DAL.GetExport2(Http, bnh.Id);
                // format it and write it
                await WriteNote(sw, bnh, bnh, nc, isHtml, false, tags);

                await sw.WriteLineAsync();

                // extra stuff for collapsable responses

                if (isCollapsible && isHtml && bnh.ResponseCount > 0)
                {
                    await sw.WriteLineAsync("<div class=\"container\"><div class=\"panel-group\">" +
                        "<div class=\"panel panel-default\"><div class=\"panel-heading\"><div class=\"panel-title\"><a data-toggle=\"collapse\" href=\"#collapse" +
                        bnh.NoteOrdinal + "\"><h5>Toggle " + bnh.ResponseCount + " Response" + (bnh.ResponseCount > 1 ? "s" : "") + "</h5></a></div></div><div id = \"collapse" + bnh.NoteOrdinal +
                        "\" class=\"panel-collapse collapse\"><div class=\"panel-body\">");
                }

                for (int rnum = 1; rnum <= bnh.ResponseCount; rnum++)
                {
                    NoteHeader nh = null;
                    NoteContent ncr = null;

                    //req = "" + nfid + "." + model.ArchiveNumber + "." + bnh.NoteOrdinal + "." + rnum;
                    List<NoteHeader> zz = await DAL.GetExport(Http, nfid, model.ArchiveNumber, bnh.NoteOrdinal, rnum);

                    nh = zz[0];

                    if (!nh.IsDeleted && nh.Version == 0)
                    {

                        //req = nh.Id.ToString();

                        ncr = await DAL.GetExport2(Http, nh.Id);

                        await WriteNote(sw, bnh, nh, ncr, isHtml, true, tags);
                    }
                }

                // extra stuff to terminate collapsable responses
                if (isCollapsible && isHtml && bnh.ResponseCount > 0)
                {
                    await sw.WriteLineAsync("</div></div></div></div></div> ");
                }
            }

            if (isHtml)  // end the html
            {
                //sb.AppendLine("<script src = \"https://www.drsinder.com/Notes2021/js/prism.min.js\" ></script >");

                await sw.WriteLineAsync("</div></body></html>");
            }

            // make sure all output is written to stream and rewind it
            await sw.FlushAsync();
            ms.Seek(0, SeekOrigin.Begin);
            // send stream to caller
            return ms;
        }


        private async Task<bool> WriteNote(StreamWriter sw, NoteHeader bnh, NoteHeader nh, NoteContent nc, bool isHtml, bool isResponse, List<Tags> tags)
        {
            //StringBuilder sb;
            if (isHtml)
            {
                string extra = "";
                if (nc.NoteBody.StartsWith("<DIV STYLE="))
                    extra = "-client";

                if (!isResponse)
                    await sw.WriteLineAsync("<div class=\"base-note" + extra + "\">");
                else
                    await sw.WriteLineAsync("<div class=\"response-note" + extra + "\">");

                await sw.WriteLineAsync("<h3>");
            }

            if (!isResponse)
            {
                // write base note header
                await sw.WriteLineAsync("Note: " + nh.NoteOrdinal + " - Subject: "
                + nh.NoteSubject + (isHtml ? "<br />" : " - ") + "Author: " + nh.AuthorName + " - "
                + nh.LastEdited.ToShortDateString() + " " + nh.LastEdited.ToShortTimeString() + " - "
                + bnh.ResponseCount + " Response" + (bnh.ResponseCount > 1 || bnh.ResponseCount == 0 ? "s" : ""));
            }
            else
            {
                // write response note header
                await sw.WriteLineAsync("Note: " + nh.NoteOrdinal + " - Subject: "
                + nh.NoteSubject + (isHtml ? "<br />" : " - ") + "Author: " + nh.AuthorName + " - "
                + nh.LastEdited.ToShortDateString() + " " + nh.LastEdited.ToShortTimeString() + " - "
                + "Response " + nh.ResponseOrdinal + " of " + bnh.ResponseCount);
                await sw.WriteLineAsync((isHtml ? "<br />" : string.Empty) + "Base Note Subject: " + bnh.NoteSubject);
            }

            if (isHtml)
            {
                await sw.WriteLineAsync("</h3>");
            }
            // Do tags

            List<Tags> tl = tags.Where(p => p.NoteHeaderId == nc.NoteHeaderId).ToList();

            if (!isHtml || (tl is not null && tl.Count > 0))
            {
                await sw.WriteAsync((isHtml ? "<h5>" : "") + "Tags - ");
                foreach (Tags item in tl)
                {
                    await sw.WriteAsync(item.Tag + " ");
                }
                await sw.WriteLineAsync((isHtml ? "</h5>" : ""));
            }

            if (!isHtml || !string.IsNullOrEmpty(nh.DirectorMessage))
            {
                await sw.WriteLineAsync((isHtml ? "<h5>" : "") + "Director Message - " + nh.DirectorMessage + (isHtml ? "</h5>" : ""));
            }
            await sw.WriteLineAsync();
            //if (isHtml)
            //{
            //    sb = new StringBuilder();
            //    sb.Append("<h4>");
            //    sb.Append("<a href=\"");
            //    sb.Append(ProdUri + "/NoteDisplay/CreateResponse/" + nc.BaseNoteId +
            //              "\" target=\"_blank\">New Response</a>");
            //    sb.Append("</h4>");
            //    await sw.WriteLineAsync(sb.ToString());
            //}

            if (isHtml)
                await sw.WriteLineAsync(nc.NoteBody.Replace("<br />", "<br />\r\n") + "</div>");
            else
                await sw.WriteLineAsync(nc.NoteBody.Replace("<br />", "\r\n"));
            await sw.WriteLineAsync();

            return true;
        }


        public async Task SaveAs(string filename, byte[] data)
        {
            await module.InvokeVoidAsync("saveAsFile", filename, Convert.ToBase64String(data));
        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }
    }
}
