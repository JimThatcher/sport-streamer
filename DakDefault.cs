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
using System;
using System.Text.Json;
using WebAPI.Models;

namespace DakAccess
{
    // This base class parses the RTD stream from a Daktronics scoreboard controller.
    // It recognizes and parses all common fields, and provides a default implementation
    // that recognizes most fields for Football, Basktball, Soccer, and Hockey.
    // Sport-specific fields for other sports can be implemented in a new class that
    // inherits from this one.
    // This class parses data based on the following zero-based data in the RTD stream:
    // -- 200-208: PlayClock - public string PlayClock {get {return _data.Substring(200, 8).Trim();}}
    // -- 200-200: HomePossession (Soccer) - public bool HomePossession { get {return (_data.Substring(200, 1) == "<") ? true : false;}}
    // -- 205-205: GuestPossession (Soccer) - public bool HomePossession { get {return (_data.Substring(200, 1) == ">") ? true : false;}}
    // -- 209-209: HomePossession (Foot/Basket) - public bool HomePossession { get {return (_data.Substring(209, 1) == "<") ? true : false;}}
    // -- 214-214: GuestPossession - (Football) public bool GuestPossession { get {return (_data.Substring(214, 1) == ">") ? true : false;}}
    // -- 215-215: GuestPossession (Basketball) - public bool GuestPossession { get {return (_data.Substring(215, 1) == ">") ? true : false;}}
    // -- 219-220: BallOn (Football) - public string BallOn { get {return _data.Substring(219, 2).TrimStart();}}
    // -- 221-223: Down (Football) - public string Down { get {return _data.Substring(221, 3).TrimEnd();}}
    // -- 221-221: HomeBonus (Basketball) - public bool HomeBonus { get {return (_data.Substring(221, 1) == "<") ? true : false;}}
    // -- 224-225: Distance (Football) - public string Distance { get {return _data.Substring(224, 2).TrimStart();}}
    // -- 228-228: GuestBonus (Basketball) - public bool GuestBonus { get {return (_data.Substring(228, 1) == ">") ? true : false;}}
    // -- 235-236: HomeFouls (Basketball) - public string HomeFouls { get {return _data.Substring(235, 2).TrimStart();}}
    // -- 237-238: GuestFouls (Basketball) - public string GuestFouls { get {return _data.Substring(237, 2).TrimStart();}}
    // -- 310-313: Flag (Football) - public string Flag { get {return _data.Substring(310, 4).Trim();}}
    // -- 345-345: HomePenaltyTrue (Hockey) - public bool HomePenaltyTrue { get {return (_data.Substring(345, 1) == "<") ? true : false;}}
    // -- 353-353: GuestPenaltyTrue (Hockey) - public bool GuestPenaltyTrue { get {return (_data.Substring(353, 1) == ">") ? true : false;}}

    public class DakDefault : ISportData
    {
        protected string _data;
        protected bool _clkUpdated = false;
        protected bool _dataUpdated = false;
        protected bool _teamsUpdated = false;
        protected string _lastTime = "";
        protected string _code = "1000";
        protected string _sport = "Unknown";

        public virtual string GetDefaultData() {
            return "12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Quarter  cc                     66015000418       9:00";
        }

        public virtual string GetSportCode() {
            return _code;
        }

        public DakDefault(string code) {
            _code = code;
            _sport = "Unknown";
            _data = GetDefaultData();
        }

        public virtual void UpdateData(int dataLen, int offset, ref string data) {
            _data = data;
            // If the change starts at zero and extends beyond Home Score field, it's a full update
            if (offset == 0 && dataLen >= 107) {
                _clkUpdated = true;
                _dataUpdated = true;
                _teamsUpdated = true;
                _lastTime = MainClock;
            }
            // if it starts at zero and the clock has changed, or if the update is the timeout timer field or the play clock timer field
            else if ((offset == 0 && _lastTime != MainClock) || (((offset > 0) && (offset <= 31)) && ((offset + dataLen) >= 31)) || offset == 200) {
                _clkUpdated = true;
                _lastTime = MainClock;
            }
            // If the team names were changed
            else if (offset >= 47 && offset < 98)
                _teamsUpdated = true;
            // If data beyond home score field was changed
            else if (offset >= 107) {
                _dataUpdated = true;
            }
        }
        public string GetTeams(ref string data) {
            _data = data;
            _teamsUpdated = false;
            return JsonSerializer.Serialize(new Teams(this.HomeTeamName, this.GuestTeamName, this.HomeTeamShort, this.GuestTeamShort));
        }

        public bool ClockUpdated() { return _clkUpdated; }
        public bool DataUpdated() { return _dataUpdated; }
        public bool TeamsUpdated() { return _teamsUpdated; }
        public virtual ClockData GetClockData() {
            ClockData _clock = new ClockData();
            try {
                _clock.Clk = (long) TimeSpan.Parse("0:" + MainClock).TotalSeconds * 1000;

            } catch (Exception) {}
            try {
                _clock.Pck = (long) TimeSpan.Parse("0:00:" + PlayClock).TotalMilliseconds;
            } catch (Exception) {}
            _clock.Id = 1;
            _clock.isRunning = ClockIsRunning;
            _clock.lastChange = DateTime.UtcNow;
            _clkUpdated = false;
            return _clock;
        }
        public virtual DbScoreData GetScoreData() {
            DbScoreData _score = new DbScoreData();
            _score.id = 1;
            int _out = 0;
            bool _ok = Int32.TryParse(PeriodNum, out _out);
            _score.Pd = _ok ? _out : 0;
            _ok = Int32.TryParse(HomeScore, out _out);
            _score.Hs = _ok ? _out : 0;
            _ok = Int32.TryParse(GuestScore, out _out);
            _score.Gs = _ok ? _out : 0;
            _ok = Int32.TryParse(HomeTOL, out _out);
            _score.Htol = _ok ? _out : 0;
            _ok = Int32.TryParse(GuestTOL, out _out);
            _score.Gtol = _ok ? _out : 0;
            _score.Hto = HomeTimeOutTrue;
            _score.Gto = GuestTimeOutTrue;
            _score.Hpo = HomePossession;
            _score.Gpo = GuestPossession;
            _score.Dn = Down;
            _score.Dt = Distance;
            _score.BO = BallOn;
            _score.Fl = Flag;
            _score.Hb = HomeBonus;
            _score.Gb = GuestBonus;
            _score.Hf = HomeFouls;
            _score.Gf = GuestFouls;
            _score.Hpn = HomePenaltyTrue;
            _score.Gpn = GuestPenaltyTrue;
            _dataUpdated = false;
            return _score;
        }
        public virtual ConsoleInfo GetConsoleInfo() {
            ConsoleInfo _console = new ConsoleInfo();
            _console.id = 1;
            _console.SportCode = _code;
            _console.Sport = _sport;
            _console.ConsoleModel = ConsoleModel;
            _console.FirmwareVersion = ConsoleFirmware;
            return _console;
        }
        // Common to all Daktronics sports with clock
        public string MainClock { get {return _data.Substring(5, 8).Trim();}}
        public bool ClockIsRunning { get {return (_data.Substring(27, 1) != "s") ? true : false; }}
        public string TimeOutClock { get {return _data.Substring(31, 8).TrimEnd();}}
        public string HomeTeamName { get {return _data.Substring(47, 20).TrimEnd();}}
        public string GuestTeamName { get {return _data.Substring(67, 20).TrimEnd();}}
        public string HomeTeamShort { get {return _data.Substring(87, 10).TrimEnd();}}
        public string GuestTeamShort { get {return _data.Substring(97, 10).TrimEnd();}}
        public string HomeScore { get {return _data.Substring(107, 4).TrimStart();}}
        public string GuestScore { get {return _data.Substring(111, 4).TrimStart();}}
        public string HomeTOL { get {return _data.Substring(121, 2).TrimStart();}}
        public string GuestTOL { get {return _data.Substring(129, 2).TrimStart();}}
        public string HomeTimeOut { get {return _data.Substring(132, 4).TrimStart();}}
        public string GuestTimeOut { get {return _data.Substring(137, 4).TrimStart();}}
        public bool HomeTimeOutTrue { get {return (_data.Substring(131, 1) == "<") ? true : false;}}
        public bool GuestTimeOutTrue { get {return (_data.Substring(136, 1) == ">") ? true : false;}}
        public string PeriodNum { get {return _data.Substring(141, 2).TrimStart();}}
        public string PeriodOrd { get {return _data.Substring(143, 4).TrimEnd();}}
        public string PeriodText { get {return _data.Substring(147, 12).TrimEnd();}}
        // Sport-specific fields we will make a best attempt to parse when we don't know the sport
        public string PlayClock { get {
            if (_data.Substring(200, 1) == "<") {
                _sport = "Soccer";
                return "";
            } else
            return _data.Substring(200, 8).Trim();
        }}
        public bool HomePossession { get {
            if (_data.Substring(200, 1) == "<") {
                _sport = "Soccer";
                return true;
            } else if (_data.Substring(209, 1) == "<") {
                // Football and Basketball
                return true;
            }
            return false;
        }}
        public bool GuestPossession { get {
            if (_data.Substring(214, 1) == ">") {
                _sport = "Football";
                return true;
            } else if (_data.Substring(215, 1) == ">") {
                _sport = "Basketball";
                return true;
            } else if (_data.Substring(205, 1) == ">") {
                _sport = "Soccer";
                return true;
            } else
                return false;
        }}
        public int BallOn { get {
            int _out = 0;
            if (int.TryParse(_data.Substring(219, 2).TrimStart(), out _out)) {
                _sport = "Football";
                return _out;
            } else {
                return 0;
            }
        }}
        public int Down { get {
            int _out = 0;
            if (int.TryParse(_data.Substring(221, 1).TrimStart(), out _out)) {
                _sport = "Football";
                return _out;
            } else {
                return 0;
            }
        }}
        public int Distance { get {
            int _out = 0;
            if (int.TryParse(_data.Substring(224, 2).TrimStart(), out _out)) {
                _sport = "Football";
                return _out;
            } else {
                return 0;
            }
        }}
        public bool HomeBonus { get {
            if (_data.Substring(221, 1) == "<") {
                _sport = "Basketball";
                return true;
            } else
                return false;
        }}
        public bool GuestBonus { get {
            if (_data.Substring(228, 1) == ">") {
                _sport = "Basketball";
                return true;
            } else
                return false;
        }}
        public int HomeFouls { get {
            int _out = 0;
            if (int.TryParse(_data.Substring(235, 2).TrimStart(), out _out)) {
                _sport = "Basketball";
                return _out;
            } else {
                return 0;
            }
        }}
        public int GuestFouls { get {
            int _out = 0;
            if (int.TryParse(_data.Substring(237, 2).TrimStart(), out _out)) {
                _sport = "Basketball";
                return _out;
            } else {
                return 0;
            }
        }}
        public bool Flag { get {
            if (_data.Length >= 313 && _data.Substring(310, 4) == "FLAG") {
                _sport = "Football";
                return true;
            } else
                return false;
        }}
        public bool HomePenaltyTrue { get {
            if (_data.Length > 345)
                return (_data.Substring(345, 1) == "<") ? true : false;
            else
                return false;
        }}
        public bool GuestPenaltyTrue { get {
            if (_data.Length > 353)
                return (_data.Substring(353, 1) == ">") ? true : false;
            else
                return false;
        }}

        public string ConsoleModel { get {return _data.Substring(187, 4);}}
        public string ConsoleFirmware { get {return _data.Substring(191, 3);}}

        public string Dump() { return _data; }
    }
}
