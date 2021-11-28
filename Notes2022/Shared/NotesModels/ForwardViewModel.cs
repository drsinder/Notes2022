using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace Notes2022.Shared
{
    [ProtoContract]
    public class ForwardViewModel
    {
        [ProtoMember(1)]
        public NoteFile NoteFile { get; set; }
        [ProtoMember(2)]
        public long NoteID { get; set; }
        [ProtoMember(3)]
        public int FileID { get; set; }
        [ProtoMember(4)]
        public int ArcID { get; set; }
        [ProtoMember(5)]
        public int NoteOrdinal { get; set; }

        [ProtoMember(6)]
        [Display(Name = "Subject")]
        public string NoteSubject { get; set; }

        [ProtoMember(7)]
        [Display(Name = "Forward whole note string")]
        public bool wholestring { get; set; }
        [ProtoMember(8)]
        public bool hasstring { get; set; }
        [ProtoMember(9)]
        public bool IsAdmin { get; set; }
        [ProtoMember(10)]
        public bool toAllUsers { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Forward to Email Address")]
        [ProtoMember(11)]
        public string? ToEmail { get; set; }

        [ProtoMember(12)]
        public string userId { get; set; }  

    }
}
