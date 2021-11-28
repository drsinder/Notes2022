using ProtoBuf;

namespace Notes2022.Shared
{
    [ProtoContract]
    public class JsonExport
    {
        [ProtoMember(1)]
        public NoteFile NoteFile { get; set; }
        [ProtoMember(2)]
        public List<NoteHeader> NoteHeaders { get; set; }
    }
}
