/*--------------------------------------------------------------------------
**
**  Copyright © 2019, Dale Sinder
**
**  Name: TextViewModel.cs
**
**  Description:
**      USed for creating and editing notes
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
using System.ComponentModel.DataAnnotations;

namespace Notes2022.Shared
{
    /// <summary>
    /// Model used to input data for a note.
    /// </summary>
    /// 

    [ProtoContract]
    public class TextViewModel
    {
        //[Required(ErrorMessage = "A Note body is required.")]
        //[StringLength(100000)]
        //[Display(Name = "MyNote")]
        [ProtoMember(1)]
        public string MyNote { get; set; }

        //[Required(ErrorMessage = "A Subject is required.")]
        [StringLength(200)]
        [Display(Name = "MySubject")]
        [Required(ErrorMessage = "A Subject is required.")]
        [ProtoMember(2)]
        public string MySubject { get; set; }

        //[Required]
        [ProtoMember(3)]
        public int NoteFileID { get; set; }

        //[Required]
        [ProtoMember(4)]
        public long BaseNoteHeaderID { get; set; }

        [ProtoMember(5)]
        public long NoteID { get; set; }

        [StringLength(200)]
        [Display(Name = "Tags")]
        [ProtoMember(6)]
        public string TagLine { get; set; }

        [StringLength(200)]
        [Display(Name = "Director Message")]
        [ProtoMember(7)]
        public string DirectorMessage { get; set; }

        [ProtoMember(8)]
        public long RefId { get; set; }

        [ProtoMember(9)]
        public string UserId { get; set; }

        //public NoteHeader NoteHeader { get; set; }

    }



    [ProtoContract]
    public class SCheckModel
    {
        [ProtoMember(1)]
        public bool isChecked { get; set; }
        [ProtoMember(2)]
        public int fileId { get; set; }
        [ProtoMember(3)]
        public string userId { get; set; }
    }
}


