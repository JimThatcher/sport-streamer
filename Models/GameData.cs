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
    public class School
    {
        public long id { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; } = string.Empty;
        [Display(Name = "Mascot")]
        public string mascot { get; set; } = string.Empty;
        [Display(Name = "Color")]
        public string color { get; set; } = "#000000";
        [Display(Name = "Alternate Color")]
        public string color2 { get; set; } = string.Empty;
        [Display(Name = "Logo file")]
        public string logo { get; set; } = string.Empty;
        [Display(Name = "Wins (this season)")]
        public int win { get; set; } = 0;
        [Display(Name = "Losses (this season)")]
        public int loss { get; set; } = 0;

/*
        [NotMapped]
        public List<Game>? HomeGames { get; }

        [NotMapped]
        public List<Game>? AwayGames { get; }
*/
    }

    public class Game
    {
        public long id { get; set; }

        public long awayId { get; set; }

        public long homeId { get; set; }

        [Display(Name = "Game Date")]
        public long date { get; set; } = DateTime.UtcNow.AddYears(1).Ticks;
    }
    public class GameView
    {
        public long id { get; set; }
        // public string gameDate { get; set; } = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd");
        public long gameDate { get; set; } = DateTime.UtcNow.AddYears(1).Ticks;
        public string HomeName { get; set; } = "Home";
        public string HomeColor { get; set; } = "#ff0000";
        public string HomeIcon { get; set; } = string.Empty;
        public string HomeRecord {get; set; } = "0-0";
        public string GuestName { get; set; } = "Guest";
        public string GuestColor { get; set; } = "#0000ff";
        public string GuestIcon { get; set; } = string.Empty;
        public string GuestRecord {get; set; } = "0-0";
    }
}