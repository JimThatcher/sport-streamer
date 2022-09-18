/*
Copyright (c) 2022 Jim Thatcher

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace WebAPI.Models
{
    // This class is used for PUT mothods to set a Player or Sponsor from the database
    // as the currently displayed item in an on-screen overlay. The 'id' must match the
    // id of an existing item in the Players or Sponsors table in the database. If a 
    // match is found, the matching item will be copied to the corresponding in-memory
    // table and a server-sent event will be sent to any subscribed overlays. 
    public class CurrentItem
    {
        public long id { get; set; }
        public string requester { get; set; } = string.Empty;
    }

    [Table("Players")]
    public class Player
    {
        public long id { get; set; }
        [Display(Name = "Number")]
        public long jersey { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; } = string.Empty;
        [Display(Name = "Position")]
        public string position { get; set; } = string.Empty;
        [Display(Name = "Photo file")]
        public string image { get; set; } = string.Empty;
        [Display(Name = "Grade")]
        public int year { get; set; }
        [Display(Name = "Height (inches)")]
        public int height { get; set; }
        [Display(Name = "Weight (lbs)")]
        public int weight { get; set; }
        public string school {get; set;} = string.Empty;
    }
    /*
    [Table("Teams")]
    public class PlayerSorted
    {
        public long id { get; set; }
        [Display(Name = "Number")]
        public long jersey { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; } = string.Empty;
        [Display(Name = "Position")]
        public string position { get; set; } = string.Empty;
        [Display(Name = "Photo file")]
        public string image { get; set; } = string.Empty;
        [Display(Name = "Grade")]
        public int year { get; set; }
        [Display(Name = "Height (inches)")]
        public int height { get; set; }
        [Display(Name = "Weight (lbs)")]
        public int weight { get; set; }
        public string school {get; set;} = string.Empty;
    }
    */
    public class Sponsor
    {
        public long id { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; } = string.Empty;
        [Display(Name = "Logo file")]
        public string image { get; set; } = string.Empty;
    }
}