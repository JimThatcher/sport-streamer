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
 7:53 7:53    7:53 7:53    s           12:00:00HOME                GUEST               HOME      GUEST        0   0 3 2 0 5 3 2 0 5           11ST 1ST Quarter  cc                     11015000427 4545  0:45                               0 0                                                                                                              0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0                                                                      0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0                                                        
*/
    public class DakBasketball : DakDefault
    {
        public override string GetDefaultData() {
            return "12:0012:00   12:0012:00    s           12:00:00HOME                GUEST               HOME      GUEST        0   0 3 2 0 5 3 2 0 5           11ST 1ST Quarter  cc                     11015000427 4545  0:45                               0 0                                                                                                              0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0                                                                      0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0    0 0                                                        ";
        }

        public DakBasketball(string code) : base(code) {
            _code = code;
            _sport = "Basketball";
            _data = GetDefaultData();
        }
        public override ClockData GetClockData() {
            ClockData _clock = base.GetClockData();
            if (_clock.Pck == 0) {
                try {
                    _clock.Pck = (long) TimeSpan.Parse("0:00:" + PlayClock).TotalMilliseconds;
                } catch (Exception) {} 
            }
            return _clock;
        }
    }
}
