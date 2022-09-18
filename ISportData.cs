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
using WebAPI.Models;
// Implement this interface to tell the app how to parse a specific sport data from
// the Daktronics All Sport 5000 scoreboard controller console.
namespace DakAccess
{
    interface ISportData
    {
        public string GetSportCode();
        // Return the console sport code for this sport (From Daktronics console template)
        // public int MaxDataLen();
        // Return the zero-based offset of end of data for this sport
        public string GetDefaultData();
        // Return a default string for this sport to avoid uninitialized responses prior to console providing full update
        // public string GetClocks(ref string data);
        // Return JSON object with all game clocks (main period clock, play/shot clock, and time-out clock)
        public string GetTeams(ref string data);
        // Return JSON with team names and team abbreviations
        public void UpdateData(int dataLen, int offset, ref string data);
        // Takes the offset and length of current update to allow implementation to determine which fields were updated
        // Implementation should set values that will be returned in calls to ClockUpdated, DataUpdated, and TeamsUpdated
        public bool ClockUpdated();
        // Return true if any of the clocks have changes since last call to GetClocks
        public bool DataUpdated();
        // Return true if any elements returned in GetData have changed since last call to GetData
        public bool TeamsUpdated();
        // Return true if the team names have changes since last call to GetTeams
        public ClockData GetClockData();
        // Return a ClockData object representing the current score to be sent to the database
        public DbScoreData GetScoreData();
        // Return a ScoreData object representing the current score to be inserted into the database
        public ConsoleInfo GetConsoleInfo();
        // Return a ConsoleInfo object representing the current connection to the scoreboard console
    }
}