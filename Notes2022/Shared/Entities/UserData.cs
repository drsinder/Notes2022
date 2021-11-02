/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
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
    **  If not, see<http: //www.gnu.org/licenses/gpl-3.0.txt>.
    **
    **--------------------------------------------------------------------------*/




using System.ComponentModel.DataAnnotations;

namespace Notes2022.Shared
{
    public class UserData
    {
        [Required]
        [Key]
        [StringLength(450)]
        public string UserId { get; set; }

        //[Display(Name = "Display Name")]
        [StringLength(50)]
        public string DisplayName { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        public int TimeZoneID { get; set; }

        public int Ipref0 { get; set; }

        public int Ipref1 { get; set; }

        public int Ipref2 { get; set; } // user choosen page size

        public int Ipref3 { get; set; }

        public int Ipref4 { get; set; }

        public int Ipref5 { get; set; }

        public int Ipref6 { get; set; }

        public int Ipref7 { get; set; }

        public int Ipref8 { get; set; }

        public int Ipref9 { get; set; } // bits extend bool properties


        public bool Pref0 { get; set; }

        public bool Pref1 { get; set; } // false = use paged note index, true= scrolled

        public bool Pref2 { get; set; } // use alternate editor

        public bool Pref3 { get; set; } // show responses by default

        public bool Pref4 { get; set; } // multiple expanded responses

        public bool Pref5 { get; set; } // expanded responses

        public bool Pref6 { get; set; } // alternate text editor

        public bool Pref7 { get; set; } // show content when expanded

        public bool Pref8 { get; set; }

        public bool Pref9 { get; set; }

        //[Display(Name = "Style Preferences")]
        //[StringLength(7000)]
        //public string? MyStyle { get; set; }

        [StringLength(100)]
        public string? MyGuid { get; set; }


        //public UserData(string userId, string displayName, 
        //    int timeZoneID, int ipref2, int ipref3, int ipref4, int ipref5, int ipref6, int ipref7, int ipref8,
        //    bool pref1, bool pref2, bool pref3, bool pref4, bool pref5, bool pref6, bool pref7, bool pref8,
        //    string mstyle, string mguid )
        //{
        //    UserId = userId;
        //    DisplayName = displayName;
        //    TimeZoneID = timeZoneID;
        //    Ipref2 = ipref2;
        //    Ipref3 = ipref3;
        //    Ipref4 = ipref4;
        //    Ipref5 = ipref5;
        //    Ipref6 = ipref6;
        //    Ipref7 = ipref7;
        //    Ipref8 = ipref8;
        //    Pref1 = pref1;
        //    Pref2 = pref2;
        //    Pref3 = pref3;
        //    Pref4 = pref4;
        //    Pref5 = pref5;
        //    Pref6 = pref6;
        //    Pref7 = pref7;
        //    Pref8 = pref8;
        //    MyStyle = mstyle;
        //    MyGuid = mguid;
        //}
    }
}
