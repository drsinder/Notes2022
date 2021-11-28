/*--------------------------------------------------------------------------
    **
    ** Copyright © 2022, Dale Sinder
    **
    ** Name: DisplayModel.cs
    **
    ** Description:
    **      NoteContent + tags
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

namespace Notes2022.Shared
{
    [ProtoContract]
    public class DisplayModel
    {
        [ProtoMember(1)]
        public NoteFile noteFile { get; set; }
        [ProtoMember(2)]
        public NoteHeader header { get; set; }
        [ProtoMember(3)]
        public NoteContent content { get; set; }
        [ProtoMember(4)]
        public List<Tags> tags { get; set; }
        [ProtoMember(5)]
        public NoteAccess access { get; set; }
        [ProtoMember(6)]
        public bool CanEdit { get; set; }
        [ProtoMember(7)]
        public bool IsAdmin { get; set; }
    }
}
