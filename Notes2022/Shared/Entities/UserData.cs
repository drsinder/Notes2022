/*--------------------------------------------------------------------------
    **
    ** Copyright © 2022, Dale Sinder
    **
    ** Name: UserData.cs
    **
    ** Description:
    **      User Preferences
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
using System.Runtime.Serialization;

namespace Notes2022.Shared
{
    [ProtoContract]
    public class UserData
    {
        [Required]
        [Key]
        [StringLength(450)]
        [ProtoMember( 1)]
        public string UserId { get; set; }

        //[Display(Name = "Display Name")]
        [StringLength(50)]
        [ProtoMember( 2)]
        public string DisplayName { get; set; }

        public string DisplayName2
        {
            get { return DisplayName.Replace(" ", "_"); }
        }

        [StringLength(150)]
        [ProtoMember( 3)]
        public string Email { get; set; }

        [ProtoMember( 4)]
        public int TimeZoneID { get; set; }

        [ProtoMember( 5)]
        public int Ipref0 { get; set; }

        [ProtoMember( 6)]
        public int Ipref1 { get; set; }

        [ProtoMember( 7)]
        public int Ipref2 { get; set; } // user choosen page size

        [ProtoMember( 8)]
        public int Ipref3 { get; set; }

        [ProtoMember( 9)]
        public int Ipref4 { get; set; }

        [ProtoMember( 10)]
        public int Ipref5 { get; set; }

        [ProtoMember( 11)]
        public int Ipref6 { get; set; }

        [ProtoMember( 12)]
        public int Ipref7 { get; set; }

        [ProtoMember( 13)]
        public int Ipref8 { get; set; }

        [ProtoMember( 14)]
        public int Ipref9 { get; set; } // bits extend bool properties


        [ProtoMember( 15)]
        public bool Pref0 { get; set; }

        [ProtoMember( 16)]
        public bool Pref1 { get; set; } // false = use paged note index, true= scrolled

        [ProtoMember( 17)]
        public bool Pref2 { get; set; } // use alternate editor

        [ProtoMember( 18)]
        public bool Pref3 { get; set; } // show responses by default

        [ProtoMember( 19)]
        public bool Pref4 { get; set; } // multiple expanded responses

        [ProtoMember( 20)]
        public bool Pref5 { get; set; } // expanded responses

        [ProtoMember( 21)]
        public bool Pref6 { get; set; } // alternate text editor

        [ProtoMember( 22)]
        public bool Pref7 { get; set; } // show content when expanded

        [ProtoMember( 23)]
        public bool Pref8 { get; set; }

        [ProtoMember( 24)]
        public bool Pref9 { get; set; }

        [StringLength(100)]
        [ProtoMember( 25)]
        public string? MyGuid { get; set; }

    }
}
