using System;

namespace WebAPI.Models
{
    public class ClockData
    {
        /*
        public ClockData(int clock = 12, int playClock = 20) {
            Id = 1;
            Clock = clock * 60000; 
            PlayClock = playClock * 1000;
        } */
        private long clockMillis;
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