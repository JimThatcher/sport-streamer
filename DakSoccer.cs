using System;
using System.Text.Json;
using WebAPI.Models;

namespace DakAccess
{
    /*
    public class SoccerData {
        private DakSoccer _fb;
        public SoccerData(DakSoccer fb) { _fb = fb; }
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
    }
    */
    
    public class DakSoccer : DakDefault
    {
        public override string GetDefaultData() {
            return "12:0012:00   12:0012:00    s   00:00           HOME                GUEST               HOME      GUEST        0   0 3 0 0 3 3 0 0 3           11ST 1ST Quarter  ";
        }
        public DakSoccer(string code) : base(code) {
            _code = code;
            _sport = "Soccer";
            _data = GetDefaultData();
        }
        /*
        public override DbScoreData GetScoreData() {
            DbScoreData _score = base.GetScoreData();
            _score.Hpo = HomePossession;
            _score.Gpo = GuestPossession;
            return _score;
        }
        */
        // public bool HomePossession { get {return (_data.Substring(200, 1) == "<") ? true : false;}}
        // public bool GuestPossession { get {return (_data.Substring(205, 1) == ">") ? true : false;}}
    }
}
