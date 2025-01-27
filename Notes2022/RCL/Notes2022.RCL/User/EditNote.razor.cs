﻿using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Notes2022.Shared;
using System.Net.Http.Json;

namespace Notes2022.RCL.User
{
    public partial class EditNote
    {
        [Parameter] public long NoteId { get; set; }   //  what we are editing

        protected TextViewModel Model { get; set; } = new TextViewModel();

        protected DisplayModel stuff { get; set; }

        [Inject] HttpClient Http { get; set; }
        [Inject] GrpcChannel Channel { get; set; }
        public EditNote()
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            IntWrapper req = new IntWrapper
            {
                myLong = NoteId,
                userId = Globals.UserData.UserId
            };

            stuff = await DAL.GetNoteContent(Channel, req);

            Model.NoteFileID = stuff.noteFile.Id;
            Model.NoteID = NoteId;
            Model.BaseNoteHeaderID = stuff.header.BaseNoteId;
            Model.RefId = stuff.header.RefId;
            Model.MyNote = stuff.content.NoteBody;
            Model.MySubject = stuff.header.NoteSubject;
            Model.DirectorMessage = stuff.header.DirectorMessage;

            string tags = "";
            foreach (var tag in stuff.tags)
            {
                tags += tag + " ";
            }
            Model.TagLine = tags;
        }
    }
}
