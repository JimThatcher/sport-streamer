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
/*
        [Display(Name = "Sport")]
        public string sport { get; set; }
*/
    }
/*
    public class GameConfig
    {
        public long Id { get; set; }
        public string gameDate { get; set; } = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd");
        public string HomeName { get; set; } = "Home";
        public string HomeColor { get; set; } = "#ff0000";
        public string HomeIcon { get; set; } = string.Empty;
        public int HomeWin {get; set; } = 0;
        public int HomeLoss {get; set; } = 0;
        public string GuestName { get; set; } = "Guest";
        public string GuestColor { get; set; } = "#0000ff";
        public string GuestIcon { get; set; } = string.Empty;
        public int GuestWin {get; set; } = 0;
        public int GuestLoss {get; set; } = 0;
        // public bool LiveFeed { get; set; }
    }
*/
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
    /*
    public class NextGame
    {
        public long id { get; set; }
        public string gameDate { get; set; } = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd");
        public string HomeName { get; set; } = "Home";
        public string HomeColor { get; set; } = "#ff0000";
        public string HomeIcon { get; set; } = string.Empty;
        public string HomeRecord {get; set; } = "0-0";
        public string GuestName { get; set; } = "Guest";
        public string GuestColor { get; set; } = "#0000ff";
        public string GuestIcon { get; set; } = string.Empty;
        public string GuestRecord {get; set; } = "0-0";
    }
    public class TempGame
    {
        public long id { get; set; }
        public long awayId { get; set; }
        public long homeId { get; set; }
        public string HomeName { get; set; } = "Home";
        public string HomeColor { get; set; } = "#ff0000";
        public string HomeIcon { get; set; } = string.Empty;
        public string HomeRecord {get; set; } = "0-0";
        public string GuestName { get; set; } = "Guest";
        public string GuestColor { get; set; } = "#0000ff";
        public string GuestIcon { get; set; } = string.Empty;
        public string GuestRecord {get; set; } = "0-0";
    }
    public class TempSchool
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
    }
    */
}