using System;
using System.Text.Json;
using WebAPI.Models;

namespace DakAccess
{

/*
12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Quarter  cc                     66015000418       9:00                                                                0   0   0   0   0   0 0 0                                             BALLON
*/
    /*
    public class FootballData {
        private DakFootball _fb;
        public FootballData(DakFootball fb) { _fb = fb; }
        public string Hs { get {return _fb.HomeScore;}}
        public string Gs { get {return _fb.GuestScore;}}
        public string Htol { get {return _fb.HomeTOL;}}
        public string Gtol { get {return _fb.GuestTOL;}}
        public string Htx { get {return _fb.HomeTimeOut;}}
        public string Gtx { get {return _fb.GuestTimeOut;}}
        public bool Hto { get {return _fb.HomeTimeOutTrue;}}
        public bool Gto { get {return _fb.GuestTimeOutTrue;}}
        public string Pd { get {return _fb.PeriodNum;}}
        public string PdO { get {return _fb.PeriodOrd;}}
        public string PdTx { get {return _fb.PeriodText;}}
        public bool Hpo { get {return _fb.HomePossession;}}
        public bool Gpo { get {return _fb.GuestPossession;}}
        public string BO { get {return _fb.BallOn;}}
        public string Dn { get {return _fb.Down.Length > 1 ? _fb.Down.Substring(0, 1) : "0";}}
        public string Dt { get {return _fb.Distance;}}
        public string Fl { get {return _fb.Flag;}}
    }
    */
    
    public class DakFootball : DakDefault
    {

        public override string GetDefaultData() {
            return "12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Quarter  cc                     66015000418       9:00                                                                0   0   0   0   0   0 0 0                                             BALLON";
        }
        /*
        public override string GetSportCode() {
            return _code;
        }
        */
        public DakFootball(string code) : base(code) {
            _code = code;
            _sport = "Football";
            _data = GetDefaultData();
        }

        /*
        public void UpdateData(int dataLen, int offset, ref string data) {
            _data = data;
            // _code = data.Substring(183, 4);
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
            return JsonSerializer.Serialize(new Teams(this.HomeTeamName, this.GuestTeamName, this.HomeTeamShort, this.GuestTeamShort));
        }

        public bool ClockUpdated() { return _clkUpdated; }
        public bool DataUpdated() { return _dataUpdated; }
        public bool TeamsUpdated() { return _teamsUpdated; }
        */
        public override ClockData GetClockData() {
            ClockData _clock = base.GetClockData();
            try {
                _clock.Pck = (long) TimeSpan.Parse("0:00:" + PlayClock).TotalSeconds * 1000;
            } catch (Exception) {}
            return _clock;
        }
        public override DbScoreData GetScoreData() {
            DbScoreData _score = base.GetScoreData();
            int _out = 0;
            bool _ok = Int32.TryParse(BallOn, out _out);
            _score.BO = _ok ? _out : 0;
            if (Down.Length > 0) {
                _ok = Int32.TryParse(Down.Substring(0, 1), out _out);
                _score.Dn = _ok ? _out : 0;
            } else {
                _score.Dn = 0;
            }
            _ok = Int32.TryParse(Distance, out _out);
            _score.Dt = _ok ? _out : 0;
            _score.Hpo = HomePossession;
            _score.Gpo = GuestPossession;
            _score.Hto = HomeTimeOutTrue;
            _score.Gto = GuestTimeOutTrue;
            _score.Fl = (Flag == "") ? false : true;
            return _score;
        }
        public string PlayClock {get {return _data.Substring(200, 8).Trim();}}
        public bool HomePossession { get {return (_data.Substring(209, 1) == "<") ? true : false;}}
        public bool GuestPossession { get {return (_data.Substring(214, 1) == ">") ? true : false;}}
        public string BallOn { get {return _data.Substring(219, 2).TrimStart();}}
        public string Down { get {return _data.Substring(221, 3).TrimEnd();}}
        public string Distance { get {return _data.Substring(224, 2).TrimStart();}}
        public string Flag { get {return _data.Substring(310, 4).Trim();}}
    }
}
