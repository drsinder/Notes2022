/*--------------------------------------------------------------------------
    **
    ** Copyright © 2022, Dale Sinder
    **
    ** Name: EmailModel.cs
    **
    ** Description:
    **      USed for sending email
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
    public class EmailModel
    {
        [ProtoMember(1)]
        public string email { get; set; }
        [ProtoMember(2)]
        public string subject { get; set; }
        [ProtoMember(3)]
        public string payload { get; set; }
    }
}
