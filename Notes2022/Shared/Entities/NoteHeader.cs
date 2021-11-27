/*--------------------------------------------------------------------------
    **
    ** Copyright © 2022, Dale Sinder
    **
    ** Name: NoteHeader.cs
    **
    ** Description:
    **      Header for a note - every note, base or response has one
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


using ProtoBuf;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Notes2022.Shared
{
    [ProtoContract]
    public class NoteHeader
    {
        // Uniquely identifies the note
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ProtoMember( 1)]
        public long Id { get; set; }

        // The fileid the note belongs to
        [Required]
        [ProtoMember( 2)]
        public int NoteFileId { get; set; }

        [Required]
        [ProtoMember( 3)]
        public int ArchiveId { get; set; }

        [ProtoMember( 4)]
        public long BaseNoteId { get; set; }

        // the ordinal on a Base note and all its responses
        [Required]
        [Display(Name = "Note #")]
        [ProtoMember( 5)]
        public int NoteOrdinal { get; set; }

        // The ordinal of the response where 0 is a Base Note
        [Required]
        [Display(Name = "Response #")]
        [ProtoMember( 6)]
        public int ResponseOrdinal { get; set; }

        // Subject/Title of a note
        [Required]
        [StringLength(200)]
        [Display(Name = "Subject")]
        [ProtoMember( 7)]
        public string? NoteSubject { get; set; }

        // When the note was created or last edited
        [Required]
        [Display(Name = "Last Edited")]
        [ProtoMember( 8)]
        public DateTime LastEdited { get; set; }

        // When the thread was last edited
        [Required]
        [Display(Name = "Thread Last Edited")]
        [ProtoMember( 9)]
        public DateTime ThreadLastEdited { get; set; }

        [Required]
        [Display(Name = "Created")]
        [ProtoMember( 10)]
        public DateTime CreateDate { get; set; }

        // Meaningful only if ResponseOrdinal = 0
        [Required]
        [ProtoMember( 11)]
        public int ResponseCount { get; set; }

        // ReSharper disable once InconsistentNaming
        [StringLength(450)]
        [ProtoMember( 12)]
        public string? AuthorID { get; set; }

        [Required]
        [StringLength(50)]
        [ProtoMember( 13)]
        public string? AuthorName { get; set; }

        [StringLength(100)]
        [ProtoMember( 14)]
        public string? LinkGuid { get; set; }

        [ProtoMember( 15)]
        public long RefId { get; set; }

        [ProtoMember( 16)]
        public bool IsDeleted { get; set; }

        [ProtoMember( 17)]
        public int Version { get; set; }

        [StringLength(200)]
        [Display(Name = "Director Message")]
        [ProtoMember( 18)]
        public string? DirectorMessage { get; set; }

        public NoteContent? NoteContent { get; set; }

        public List<Tags>? Tags { get; set; }


        public NoteHeader Clone()
        {
            NoteHeader nh = new NoteHeader()
            {
                Id = Id,
                NoteFileId = NoteFileId,
                ArchiveId = ArchiveId,
                BaseNoteId = BaseNoteId,
                NoteOrdinal = NoteOrdinal,
                NoteSubject = NoteSubject,
                DirectorMessage = DirectorMessage,
                LastEdited = LastEdited,
                ThreadLastEdited = ThreadLastEdited,
                CreateDate = CreateDate,
                ResponseCount = ResponseCount,
                AuthorID = AuthorID,
                AuthorName = AuthorName,
                LinkGuid = LinkGuid,
                RefId = RefId,
                IsDeleted = IsDeleted,
                Version = Version,
                ResponseOrdinal = ResponseOrdinal
            };

            return nh;
        }


        public NoteHeader CloneForLink()
        {
            NoteHeader nh = new NoteHeader()
            {
                Id = Id,
                NoteSubject = NoteSubject,
                DirectorMessage = DirectorMessage,
                LastEdited = LastEdited,
                ThreadLastEdited = ThreadLastEdited,
                CreateDate = CreateDate,
                AuthorID = AuthorID,
                AuthorName = AuthorName,
                LinkGuid = LinkGuid
            };

            return nh;
        }

        public NoteHeader CloneForLinkR()
        {
            NoteHeader nh = new NoteHeader()
            {
                Id = Id,
                NoteSubject = NoteSubject,
                DirectorMessage = DirectorMessage,
                LastEdited = LastEdited,
                ThreadLastEdited = ThreadLastEdited,
                CreateDate = CreateDate,
                AuthorID = AuthorID,
                AuthorName = AuthorName,
                LinkGuid = LinkGuid,
                ResponseOrdinal = ResponseOrdinal
            };

            return nh;
        }

        public static explicit operator NoteHeader(Task<NoteHeader> v)
        {
            throw new NotImplementedException();
        }
    }
}
