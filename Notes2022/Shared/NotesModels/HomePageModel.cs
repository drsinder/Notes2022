/*--------------------------------------------------------------------------
**
**  Copyright © 2019, Dale Sinder
**
**  Name: HomePageModel.cs
**
**  Description:
**      Used for the root "/" of the app
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
using System.Runtime.Serialization;

namespace Notes2022.Shared
{
    [ProtoContract]
    public class HomePageModel
    {
        [ProtoMember( 1)]
        public List<NoteFile> NoteFiles { get; set; }

        [ProtoMember( 2)]
        public List<NoteAccess> NoteAccesses { get; set; }

        [ProtoMember( 3)]
        public string Message { get; set; }

        [ProtoMember( 4)]
        public UserData UserData { get; set; }

        [ProtoMember( 5)]
        public List<UserData> UserListData { get; set; }


    }
}
