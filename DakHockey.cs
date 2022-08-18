using System;
using System.Text.Json;

namespace DakAccess
{
    public class HockeyData {
        private DakHockey _dd;
        public HockeyData(DakHockey dd) { _dd = dd; }
        public string Hs { get {return _dd.HomeScore;}}
        public string Gs { get {return _dd.GuestScore;}}
        public string Htol { get {return _dd.HomeTOL;}}
        public string Gtol { get {return _dd.GuestTOL;}}
        public string Htx { get {return _dd.HomeTimeOut;}}
        public string Gtx { get {return _dd.GuestTimeOut;}}
        public bool Hto { get {return _dd.HomeTimeOutTrue;}}
        public bool Gto { get {return _dd.GuestTimeOutTrue;}}
        public string Pd { get {return _dd.PeriodNum;}}
        public string PdO { get {return _dd.PeriodOrd;}}
        public string PdTx { get {return _dd.PeriodText;}}
        public bool Hpn { get {return _dd.HomePenaltyTrue;}}
        public bool Gpn { get {return _dd.GuestPenaltyTrue;}}
        public string HpnTx { get {return _dd.HomePenaltyText;}}
        public string GpnTx { get {return _dd.GuestPenaltyText;}}
    }
    
    public class DakHockey : ISportData
    {
        private int _maxOffset = 361;
        private string _data;
        private bool _clkUpdated = false;
        private bool _dataUpdated = false;
        private bool _teamsUpdated = false;
        private string _lastTime = "";
        private string _code = "4000";

        public string GetDefaultData() {
            return "12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Period   ";
        }

        public int MaxDataLen() {
            return _maxOffset;
        }
        public string GetSportCode() {
            return _code;
        }

        public DakHockey(string code) {
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
            return JsonSerializer.Serialize(new HockeyData(this));
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
        public bool HomePenaltyTrue { get {return (_data.Substring(345, 1) == "<") ? true : false;}}
        public bool GuestPenaltyTrue { get {return (_data.Substring(353, 1) == ">") ? true : false;}}
        public string HomePenaltyText { get {return (_data.Substring(346, 7).Trim());}}
        public string GuestPenaltyText { get {return (_data.Substring(354, 7).Trim());}}
        public string Dump() { return _data; }  
    }
}
