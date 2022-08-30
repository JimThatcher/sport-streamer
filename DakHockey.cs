using System;
using System.Text.Json;
using WebAPI.Models;

namespace DakAccess
{
    /*
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
    */    
    public class DakHockey : DakDefault
    {

        public override string GetDefaultData() {
            return "12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Period   ";
        }
        public DakHockey(string code) : base(code) {
            _code = code;
            _sport = "Hockey";
            _data = GetDefaultData();
        }
        public override DbScoreData GetScoreData() {
            DbScoreData _score = base.GetScoreData();
            _score.Hpn = HomePenaltyTrue;
            _score.Gpn = GuestPenaltyTrue;
            _score.Hto = HomeTimeOutTrue;
            _score.Gto = GuestTimeOutTrue;
            return _score;
        }
        public bool HomePenaltyTrue { get {return (_data.Substring(345, 1) == "<") ? true : false;}}
        public bool GuestPenaltyTrue { get {return (_data.Substring(353, 1) == ">") ? true : false;}}
        public string HomePenaltyText { get {return (_data.Substring(346, 7).Trim());}}
        public string GuestPenaltyText { get {return (_data.Substring(354, 7).Trim());}}
    }
}
