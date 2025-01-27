﻿/*--------------------------------------------------------------------------
    **
    ** Copyright© 2022, Dale Sinder
    **
    ** Name: LinkProcessor.cs
    **
    ** Description:
    **      Process link requests
    **
    ** This program is free software: you can redistribute it and/or modify
    ** it under the terms of the GNU General Public License version 3 as
    ** published by the Free Software Foundation.   
    **
    ** This program is distributed in the hope that it will be useful,
    ** but WITHOUT ANY WARRANTY; without even the implied warranty of
    ** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    ** GNU General Public License version 3 for more details.
    **
    **  You should have received a copy of the GNU General Public License
    **  version 3 along with this program in file "license-gpl-3.0.txt".
    **  If not, see<http://www.gnu.org/licenses/gpl-3.0.txt>.
    **
    **--------------------------------------------------------------------------*/

using Microsoft.EntityFrameworkCore;
using Notes2022.Server.Controllers;
using Notes2022.Server.Data;
using Notes2022.Shared;
using System.Net.Http.Formatting;

namespace Notes2022.Server.Services
{
    public class LinkProcessor
    {
        private readonly NotesDbContext db;

        public LinkProcessor(NotesDbContext context)
        {
            db = context;
        }

        public async Task<string> ProcessLinkAction(long linkId)
        {
            LinkQueue q;
            try
            {
                q = await db.LinkQueue.SingleAsync(p => p.Id == linkId);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            if (q is null)
            {
                return "Job not in Queue";
            }

            NoteFile notefile = await db.NoteFile.SingleAsync(p => p.Id == q.LinkedFileId);
            string notefilename = notefile.NoteFileName;

            HttpClient MyClient = new HttpClient
            {
                BaseAddress = new Uri(q.BaseUri)
            };


            switch (q.Activity)
            {
                case LinkAction.CreateBase:
                    // create base note
                    LinkCreateModel inputModel = new LinkCreateModel();

                    inputModel.linkedfile = notefilename;

                    inputModel.header = (await db.NoteHeader.SingleAsync(p => p.LinkGuid == q.LinkGuid)).CloneForLink();
                    inputModel.content = (await db.NoteContent.SingleAsync(p => p.NoteHeaderId == inputModel.header.Id)).CloneForLink();
                    try
                    {
                        inputModel.tags = Tags.CloneForLink(await db.Tags.Where(p =>
                            p.NoteFileId == notefile.Id && p.NoteHeaderId == inputModel.header.Id)
                            .ToListAsync());
                    }
                    catch
                    {
                        inputModel.tags = null;
                    }

                    inputModel.header.Id = 0;
                    inputModel.Secret = q.Secret;

                    HttpResponseMessage resp;
                    try
                    {
                        resp = MyClient.PostAsync("api/ApiLink",
                                new ObjectContent(typeof(LinkCreateModel), inputModel, new JsonMediaTypeFormatter()))
                            .GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                    string result = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    LinkLog ll = new LinkLog()
                    {
                        EventType = "SendCreateNote",
                        EventTime = DateTime.UtcNow,
                        Event = result
                    };

                    db.LinkLog.Add(ll);

                    if (result == "Ok")
                        db.LinkQueue.Remove(q);

                    await db.SaveChangesAsync();


                    return result;

                case LinkAction.CreateResponse:
                    // create response note
                    LinkCreateRModel inputModel2 = new LinkCreateRModel();

                    inputModel2.linkedfile = notefilename;

                    inputModel2.header = (await db.NoteHeader.SingleAsync(p => p.LinkGuid == q.LinkGuid)).CloneForLinkR();
                    inputModel2.content = (await db.NoteContent.SingleAsync(p => p.NoteHeaderId == inputModel2.header.Id)).CloneForLink();
                    try
                    {
                        inputModel2.tags = Tags.CloneForLink(await db.Tags.Where(p =>
                            p.NoteFileId == notefile.Id && p.NoteHeaderId == inputModel2.header.Id)
                            .ToListAsync());
                    }
                    catch
                    {
                        inputModel2.tags = null;
                    }

                    NoteHeader basehead = await NoteDataManager.GetBaseNoteHeader(db, inputModel2.header.Id);
                    inputModel2.baseGuid = basehead.LinkGuid;

                    inputModel2.header.Id = 0;
                    inputModel2.Secret = q.Secret;

                    HttpResponseMessage resp2;
                    try
                    {
                        resp2 = MyClient.PostAsync("api/ApiLinkR",
                                new ObjectContent(typeof(LinkCreateRModel), inputModel2, new JsonMediaTypeFormatter()))
                            .GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                    string result2 = resp2.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    LinkLog ll2 = new LinkLog()
                    {
                        EventType = "SendRespNote",
                        EventTime = DateTime.UtcNow,
                        Event = result2
                    };

                    db.LinkLog.Add(ll2);

                    if (result2 == "Ok")
                        db.LinkQueue.Remove(q);

                    await db.SaveChangesAsync();


                    return result2;

                case LinkAction.Edit:


                    LinkCreateEModel model = new LinkCreateEModel()
                    {
                        tags = string.Empty,
                        linkedfile = notefilename,
                        myGuid = q.LinkGuid
                    };

                    model.header = (await db.NoteHeader.SingleAsync(p => p.LinkGuid == q.LinkGuid)).CloneForLinkR();
                    model.content = (await db.NoteContent.SingleAsync(p => p.NoteHeaderId == model.header.Id)).CloneForLink();

                    List<Tags> myTags;

                    try
                    {
                        myTags = await db.Tags.Where(p =>
                            p.NoteFileId == notefile.Id && p.NoteHeaderId == model.header.Id).ToListAsync();

                        if (myTags is null || myTags.Count < 1)
                        {
                            model.tags = string.Empty;
                        }
                        else
                        {
                            foreach (var tag in myTags)
                            {
                                model.tags += tag.Tag + " ";
                            }

                            model.tags.TrimEnd(' ');
                        }

                    }
                    catch
                    {
                        model.tags = string.Empty;
                    }

                    model.header.Id = 0;
                    model.Secret = q.Secret;

                    HttpResponseMessage resp3;
                    try
                    {
                        resp3 = MyClient.PutAsync("api/ApiLinkE",
                                new ObjectContent(typeof(LinkCreateEModel), model, new JsonMediaTypeFormatter()))
                            .GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                    string result3 = resp3.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    LinkLog ll3 = new LinkLog()
                    {
                        EventType = "EditNote",
                        EventTime = DateTime.UtcNow,
                        Event = result3
                    };

                    db.LinkLog.Add(ll3);

                    if (result3 == "Ok")
                        db.LinkQueue.Remove(q);

                    await db.SaveChangesAsync();

                    return result3;

                default:
                    return "Bad Link Activity Request";

            }

        }

        //public async Task<string> ProcessLinkDelete(long linkId)
        //{
        //    LinkQueue q;
        //    try
        //    {
        //        q = await db.LinkQueue.SingleAsync(p => p.Id == linkId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }

        //    if (q is null)
        //    {
        //        return "Job not in Queue";
        //    }

        //    //NoteFile notefile = await db.NoteFile.SingleAsync(p => p.Id == q.LinkedFileId);
        //    //string notefilename = notefile.NoteFileName;

        //    HttpClient MyClient = new HttpClient
        //    {
        //        BaseAddress = new Uri(q.BaseUri)
        //    };

        //    LinkDeleteModel delModel = new LinkDeleteModel()
        //    {
        //        baseGuid = q.LinkGuid
        //    };

        //    HttpResponseMessage resp3;

        //    try
        //    {
        //        resp3 = MyClient.PostAsync("api/ApiLinkD",
        //                new ObjectContent(typeof(LinkDeleteModel), delModel, new JsonMediaTypeFormatter()))
        //            .GetAwaiter().GetResult();
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //    string result3 = resp3.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        //    return result3;
        //}


        public async Task<bool> Test(string Uri)
        {
            HttpClient MyClient = new HttpClient
            {
                BaseAddress = new Uri(Uri)
            };

            HttpResponseMessage resp3;

            try
            {
                resp3 = await MyClient.GetAsync("api/ApiLink");
            }
            catch
            {
                return false;
            }
            string result3 = await resp3.Content.ReadAsStringAsync();

            return result3 == "Hello Notes2022";
        }

        public async Task<bool> Test2(string Uri, string file)
        {
            //string file;
            //string uri;

            //int index = Uri.LastIndexOf("/");

            //uri = Uri.Substring(0, index - 1);
            //file = Uri.Substring(index, Uri.Length - index);

            HttpClient MyClient = new HttpClient
            {
                BaseAddress = new Uri(Uri)
            };

            HttpResponseMessage resp3;

            try
            {
                resp3 = await MyClient.GetAsync("api/ApiLinkR/" + file);
            }
            catch
            {
                return false;
            }
            string result3 = await resp3.Content.ReadAsStringAsync();

            return result3 == "Ok";
        }
    }
}
