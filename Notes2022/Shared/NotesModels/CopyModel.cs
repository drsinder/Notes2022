using ProtoBuf;

namespace Notes2022.Shared
{
    [ProtoContract]
    public class CopyModel
    {
        [ProtoMember(1)]
        public NoteHeader Note { get; set; }
        [ProtoMember(2)]
        public int FileId { get; set; }
        [ProtoMember(3)]
        public bool WholeString { get; set; }
        [ProtoMember(4)]
        public string UserId { get; set; }

        //public UserData UserData { get; set; }
    }
}
