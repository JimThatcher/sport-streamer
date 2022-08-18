using System;
using System.Text.Json;

namespace DakAccess
{
    public class BasicData {
        private DakDefault _fb;
        public BasicData(DakDefault fb) { _fb = fb; }
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
    }
    
    public class DakDefault : ISportData
    {
        private int _maxOffset = 160;
        private string _data;
        private bool _clkUpdated = false;
        private bool _dataUpdated = false;
        private bool _teamsUpdated = false;
        private string _lastTime = "";
        private string _code = "1000";

        public string GetDefaultData() {
            return "12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Quarter  ";
        }

        public int MaxDataLen() {
            return _maxOffset;
        }
        public string GetSportCode() {
            return _code;
        }

        public DakDefault(string code) {
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
            else if ((offset == 0 && _lastTime != MainClock) || (offset > 0 && offset <= 31 && (offset + dataLen >= 31))) {
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
            return JsonSerializer.Serialize(new BasicData(this));
        }

        public string GetClocks(ref string data) {
            _clkUpdated = false;
            _data = data;
            return JsonSerializer.Serialize(new Clocks(this.MainClock, "0", this.TimeOutClock));
        }
        public string GetTeams(ref string data) {
            _data = data;
            return "";
        }

        public bool ClockUpdated() { return _clkUpdated; }
        public bool DataUpdated() { return _dataUpdated; }
        public bool TeamsUpdated() { return _teamsUpdated; }

        public string MainClock { get {return _data.Substring(5, 8).Trim();}}
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
        public string Dump() { return _data; }  
    }
}
