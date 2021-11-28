using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes2022.Shared
{
    [ProtoContract]
    public class UpdateAccessRequest
    {
        [ProtoMember(1)]
        public string eMail { get; set; }

        [ProtoMember(2)]
        public NoteAccess item { get; set; }


    }
}
