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
        public string HomePenaltyText { get {return (_data.Substring(346, 7).Trim());}}
        public string GuestPenaltyText { get {return (_data.Substring(354, 7).Trim());}}
    }
}
