/*--------------------------------------------------------------------------
    **
    ** Copyright © 2022, Dale Sinder
    **
    ** Name: TZone.cs
    **
    ** Description:
    **      Time Zones of the world for user selection
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
    public class TZone
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // ReSharper disable once InconsistentNaming
        [ProtoMember( 1)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [ProtoMember( 2)]
        public string? Name { get; set; }

        [Required]
        [StringLength(10)]
        [ProtoMember( 3)]
        public string? Abbreviation { get; set; }

        [Required]
        [ProtoMember( 4)]
        public string? Offset { get; set; }

        [Required]
        [ProtoMember( 5)]
        public int OffsetHours { get; set; }

        [Required]
        [ProtoMember( 6)]
        public int OffsetMinutes { get; set; }

        public DateTime Local(DateTime dt)
        {
            return dt.AddHours(OffsetHours).AddMinutes(OffsetMinutes);
        }

        //public DateTime LocalBlazor(DateTime dt)
        //{
        //    int OHours = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
        //    int OMinutes = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Minutes;

        //    return dt.AddHours(OHours).AddMinutes(OMinutes);
        //}


        public DateTime Universal(DateTime dt)
        {
            return dt.AddHours(-OffsetHours).AddMinutes(-OffsetMinutes);
        }
    }

}
