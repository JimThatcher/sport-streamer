using System;
using System.Text.Json;

namespace DakAccess
{

/*
12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Quarter  cc                     66015000418       9:00                                                                0   0   0   0   0   0 0 0                                             BALLON
*/
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
        public string Dn { get {return _fb.Down;}}
        public string Dt { get {return _fb.Distance;}}
        public string Fl { get {return _fb.Flag;}}
    }
    
    public class DakFootball : ISportData
    {
        private int _maxOffset = 345;
        private string _data;
        private bool _clkUpdated = false;
        private bool _dataUpdated = false;
        private bool _teamsUpdated = false;
        private string _lastTime = "";
        private string _code = "6601";

        public string GetDefaultData() {
            return "12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Quarter  cc                     66015000418       9:00                                                                0   0   0   0   0   0 0 0                                             BALLON";
        }

        public int MaxDataLen() {
            return _maxOffset;
        }
        public string GetSportCode() {
            return _code;
        }

        public DakFootball(string code) {
            _code = code;
            _data = GetDefaultData();
        }

        public void UpdateData(int dataLen, int offset, ref string data) {
            _data = data;
            if (offset + dataLen > _maxOffset)
                return;
            if (offset == 0 && dataLen >= 107) {
                _clkUpdated = true;
                _dataUpdated = true;
                _teamsUpdated = true;
                _lastTime = MainClock;
            }
            else if ((offset == 0 && _lastTime != MainClock) || (((offset > 0) && (offset <= 31)) && ((offset + dataLen) >= 31)) || offset == 200) {
                _clkUpdated = true;
                _lastTime = MainClock;
            }
            else if (offset >= 47 && offset < 98)
                _teamsUpdated = true;
            else if (offset >= 107) {
                _dataUpdated = true;
            }
        }

        public string GetData(ref string data) {
            _dataUpdated = false;
            _data = data;
            return JsonSerializer.Serialize(new FootballData(this));
        }

        public string GetClocks(ref string data) {
            _clkUpdated = false;
            _data = data;
            return JsonSerializer.Serialize(new Clocks(this.MainClock, this.PlayClock, this.TimeOutClock));
        }
        public string GetTeams(ref string data) {
            _data = data;
            return JsonSerializer.Serialize(new Teams(this.HomeTeamName, this.GuestTeamName, this.HomeTeamShort, this.GuestTeamShort));
        }

        public bool ClockUpdated() { return _clkUpdated; }
        public bool DataUpdated() { return _dataUpdated; }
        public bool TeamsUpdated() { return _teamsUpdated; }

        public string MainClock { get {return _data.Substring(5, 8).Trim();}}
        public string PlayClock {get {return _data.Substring(200, 8).Trim();}}
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
        public bool HomePossession { get {return (_data.Substring(209, 1) == "<") ? true : false;}}
        public bool GuestPossession { get {return (_data.Substring(214, 1) == ">") ? true : false;}}
        public string BallOn { get {return _data.Substring(219, 2).TrimStart();}}
        public string Down { get {return _data.Substring(221, 3).TrimEnd();}}
        public string Distance { get {return _data.Substring(224, 2).TrimStart();}}
        public string Flag { get {return _data.Substring(310, 4).Trim();}}
        public string Dump() { return _data; }  
    }
}
