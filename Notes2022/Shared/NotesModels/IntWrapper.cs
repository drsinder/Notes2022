using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes2022.Shared
{
    [ProtoContract]
    public class IntWrapper
    {
        [ProtoMember(1)]
        public int myInt { get; set; }
        [ProtoMember(2)]
        public string userId { get; set; }

        [ProtoMember(3)]
        public long myLong { get; set; }

        [ProtoMember(4)]
        public int myInt2 { get; set;}
        [ProtoMember(5)]
        public int myInt3 { get; set; }
        [ProtoMember(6)]
        public int myInt4 { get; set;}
    }
}
