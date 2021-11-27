/*--------------------------------------------------------------------------
    **
    ** Copyright © 2022, Dale Sinder
    **
    ** Name: LinkedFile.cs
    **
    ** Description:
    **      Represents a linked file
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
    public class LinkedFile
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ProtoMember( 1)]
        public int Id { get; set; }

        [Required]
        [ProtoMember( 2)]
        public int HomeFileId { get; set; }

        [Required]
        [StringLength(20)]
        [ProtoMember( 3)]
        public string? HomeFileName { get; set; }

        [Required]
        [StringLength(20)]
        [ProtoMember( 4)]
        public string? RemoteFileName { get; set; }

        [Required]
        [StringLength(450)]
        [ProtoMember( 5)]
        public string? RemoteBaseUri { get; set; }

        [Required]
        [ProtoMember( 6)]
        public bool AcceptFrom { get; set; }

        [Required]
        [ProtoMember( 7)]
        public bool SendTo { get; set; }

        [StringLength(50)]
        [ProtoMember( 8)]
        public string? Secret { get; set; }
    }
}
