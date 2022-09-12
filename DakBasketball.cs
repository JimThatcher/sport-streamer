using System;
using System.Text.Json;
using WebAPI.Models;

namespace DakAccess
{

/*
 7:53 7:53    7:53 7:53    s           12:00:00HOME                GUEST               HOME      GUEST        0   0 3 2 0 5 3 2 0 5           11ST 1ST Quarter  cc                     11015000427 4545  0:45                               0 0                                                                                                              0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0                                                                      0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0                                                        
*/
    /*
    public class BasketballData {
        private DakBasketball _bb;
        public BasketballData(DakBasketball bb) { _bb = bb; }
        public string Hs { get {return _bb.HomeScore;}}
        public string Gs { get {return _bb.GuestScore;}}
        public string Htol { get {return _bb.HomeTOL;}}
        public string Gtol { get {return _bb.GuestTOL;}}
        public string Htx { get {return _bb.HomeTimeOut;}}
        public string Gtx { get {return _bb.GuestTimeOut;}}
        public bool Hto { get {return _bb.HomeTimeOutTrue;}}
        public bool Gto { get {return _bb.GuestTimeOutTrue;}}
        public string Pd { get {return _bb.PeriodNum;}}
        public string PdO { get {return _bb.PeriodOrd;}}
        public string PdTx { get {return _bb.PeriodText;}}
        public bool Hpo { get {return _bb.HomePossession;}}
        public bool Gpo { get {return _bb.GuestPossession;}}
        public bool Hb { get {return _bb.HomeBonus;}}
        public bool Gb { get {return _bb.GuestBonus;}}
        public string Hf { get {return _bb.HomeFouls;}}
        public string Gf { get {return _bb.GuestFouls;}}
    }
    */
    
    public class DakBasketball : DakDefault
    {
        public override string GetDefaultData() {
            return "12:0012:00   12:0012:00    s           12:00:00HOME                GUEST               HOME      GUEST        0   0 3 2 0 5 3 2 0 5           11ST 1ST Quarter  cc                     11015000427 4545  0:45                               0 0                                                                                                              0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0                                                                      0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0                                                        ";
        }
        /*
        public override string GetSportCode() {
            return _code;
        }
        */

        public DakBasketball(string code) : base(code) {
            _code = code;
            _sport = "Basketball";
            _data = GetDefaultData();
        }
        public override ClockData GetClockData() {
            ClockData _clock = base.GetClockData();
            if (_clock.Pck == 0) {
                try {
                    _clock.Pck = (long) TimeSpan.Parse("0:00:" + PlayClock).TotalMilliseconds;
                } catch (Exception) {} 
            }
            return _clock;
        }
        /*
        public override DbScoreData GetScoreData() {
            DbScoreData _score = base.GetScoreData();
            _score.Hpo = HomePossession;
            _score.Gpo = GuestPossession;
            int _out = 0;
            bool _ok = Int32.TryParse(HomeFouls, out _out);
            _score.Hf = _ok ? _out : 0;
            _ok = Int32.TryParse(GuestFouls, out _out);
            _score.Gf = _ok ? _out : 0;
            _score.Hb = HomeBonus;
            _score.Gb = GuestBonus;
            return _score;
        }
        */
        /*
        public override ConsoleInfo GetConsoleInfo() {
            ConsoleInfo _console = base.GetConsoleInfo();
            _console.Sport = "Basketball";
            return _console;
        }
        */
        // public string PlayClock {get {return _data.Substring(200, 8).Trim();}}
        // public bool HomePossession { get {return (_data.Substring(209, 1) == "<") ? true : false;}}
        // public bool GuestPossession { get {return (_data.Substring(215, 1) == ">") ? true : false;}}
        // public bool HomeBonus { get {return (_data.Substring(221, 1) == "<") ? true : false;}}
        // public bool GuestBonus { get {return (_data.Substring(228, 1) == ">") ? true : false;}}
        // public string HomeFouls { get {return _data.Substring(235, 2).TrimStart();}}
        // public string GuestFouls { get {return _data.Substring(237, 2).TrimStart();}}
    }
}
