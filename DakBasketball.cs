using System;
using System.Text.Json;

namespace DakAccess
{

/*
12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Quarter  cc                     66015000418       9:00                                                                0   0   0   0   0   0 0 0                                             BALLON
*/
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
    
    public class DakBasketball : ISportData
    {
        private int _maxOffset = 304;
        private string _data;
        private bool _clkUpdated = false;
        private bool _dataUpdated = false;
        private bool _teamsUpdated = false;
        private string _lastTime = "";
        private string _code = "1101";

        public string GetDefaultData() {
            return "12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Quarter  cc                     11015000418       9:00                                                                0   0   0   0   0   0 0 0                                                   ";
        }

        public int MaxDataLen() {
            return _maxOffset;
        }
        public string GetSportCode() {
            return _code;
        }

        public DakBasketball(string code) {
            _code = code;
            _data = GetDefaultData();
        }

        public void UpdateData(int dataLen, int offset, ref string data) {
            _data = data;
            if (offset + dataLen > _maxOffset)
                return;
            if (offset == 0 && dataLen > 224) {
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
            return JsonSerializer.Serialize(new BasketballData(this));
        }

        public string GetClocks(ref string data) {
            _clkUpdated = false;
            _data = data;
            return JsonSerializer.Serialize(new Clocks(this.MainClock, this.PlayClock, this.TimeOutClock));
        }
        public string GetTeams(ref string data) {
            _data = data;
            return "";
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
        public bool GuestPossession { get {return (_data.Substring(215, 1) == ">") ? true : false;}}
        public bool HomeBonus { get {return (_data.Substring(221, 1) == "<") ? true : false;}}
        public bool GuestBonus { get {return (_data.Substring(228, 1) == ">") ? true : false;}}
        public string HomeFouls { get {return _data.Substring(235, 2).TrimStart();}}
        public string GuestFouls { get {return _data.Substring(237, 2).TrimStart();}}
        public string Dump() { return _data; }  
    }
}
