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

namespace WebAPI.Models
{
    public class ClockData
    {
        private long clockMillis;
#pragma warning disable 0169
        private long playMillis;
        public long Id { get; set; }

        public long Clk { 
            get {
                long result = clockMillis;
                if (isRunning) {
                    DateTime now = DateTime.UtcNow;
                    TimeSpan diff = now - lastChange;
                    long v = (long)((diff.TotalMilliseconds > clockMillis) ? 0 : (clockMillis - diff.TotalMilliseconds));
                    if (v <= 0) {
                        isRunning = false;
                        clockMillis = 0;
                        lastChange = now;
                    }
                    result = v;
                }
                return result;
            }
            set {
                clockMillis = value;
            }
        } // Remaining time in milliseconds
        public long Pck { get; set; }
        public bool isRunning { get; set; }
        public DateTime lastChange { get; set; }
        public string asJson() {
            string clockStr = (Clk > 60000) ? string.Format("{0}:{1:00}", (int) Clk / 60000, ((Clk % 60000) / 1000)) :
                    string.Format("{0}", ((Clk % 60000) / 1000));
            string playClockStr = string.Format("{0}", Pck / 1000);
            return "{\"id\":" + Id + ",\"Clk\":\"" + clockStr + "\",\"Pck\":\"" + playClockStr + "\",\"isRunning\":" + ((isRunning) ? "true" : "false") + "}";
        }
    }

    public class ClockDataDak
    {
        public ClockDataDak(ClockData data) {
            Id = data.Id;
            isRunning = data.isRunning;
            Clk = (data.Clk > 60000) ? string.Format("{0}:{1:00}", (int) data.Clk / 60000, ((data.Clk % 60000) / 1000)) :
                    string.Format("{0}", ((data.Clk % 60000) / 1000));
            Pck = string.Format("{0}", data.Pck / 1000);
        }
        public long Id { get; set; }
        public string Clk { get; }
        public string Pck { get; }
        public bool isRunning {get;}
    }
}