/*--------------------------------------------------------------------------
**
**  Copyright © 2019, Dale Sinder
**
**  Name: NoteDisplayIndexModel.cs
**
**  Description:
**      Data for the Main notes file list/index
**
**  This program is free software: you can redistribute it and/or modify
**  it under the terms of the GNU General Public License version 3 as
**  published by the Free Software Foundation.
**  
**  This program is distributed in the hope that it will be useful,
**  but WITHOUT ANY WARRANTY; without even the implied warranty of
**  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
**  GNU General Public License version 3 for more details.
**  
**  You should have received a copy of the GNU General Public License
**  version 3 along with this program in file "license-gpl-3.0.txt".
**  If not, see <http://www.gnu.org/licenses/gpl-3.0.txt>.
**
**--------------------------------------------------------------------------
*/

using ProtoBuf;

namespace Notes2022.Shared
{
    [ProtoContract]
    public class NoteDisplayIndexModel
    {
        [ProtoMember(1)]
        public NoteFile noteFile { get; set; }
        [ProtoMember(2)]
        public int ArcId { get; set; }
        [ProtoMember(3)]
        public NoteAccess myAccess { get; set; }
        [ProtoMember(4)]
        public bool isMarked { get; set; }
        [ProtoMember(5)]
        public string rPath { get; set; }
        [ProtoMember(6)]
        public string scroller { get; set; }
        [ProtoMember(7)]
        public int ExpandOrdinal { get; set; }
        [ProtoMember(8)]
        public List<NoteHeader> Notes { get; set; }
        [ProtoMember(9)]
        public NoteHeader myHeader { get; set; }
        [ProtoMember(10)]
        public TZone tZone { get; set; }
        [ProtoMember(11)]
        public List<NoteHeader> AllNotes { get; set; }
        [ProtoMember(12)]
        public string linkedText { get; set; }
        [ProtoMember(13)]
        public string message { get; set; }
        [ProtoMember(14)]
        public UserData UserData { get; set; }
        [ProtoMember(16)]
        public List<Tags> Tags { get; set; }
    }

}
