﻿/*--------------------------------------------------------------------------
    **
    ** Copyright © 2022, Dale Sinder
    **
    ** Name: Subscription.cs
    **
    ** Description:
    **      A subscription on a file to have new items emailed...
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
    public class Subscription
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ProtoMember( 1)]
        public long Id { get; set; }

        [Required]
        [ProtoMember( 2)]
        public int NoteFileId { get; set; }

        [Required]
        [StringLength(450)]
        [ProtoMember( 3)]
        public string? SubscriberId { get; set; }

        [ForeignKey("NoteFileId")]
        [ProtoMember( 4)]
        public NoteFile? NoteFile { get; set; }
    }
}
