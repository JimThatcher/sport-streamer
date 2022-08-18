// Implement this interface to tell the app how to parse a specific sport data from
// the Daktronics All Sport 5000 scoreboard controller console.
namespace DakAccess
{
    interface ISportData
    {
        public string GetSportCode();
        // Return the console sport code for this sport (From Daktronics console template)
        public int MaxDataLen();
        // Return the zero-based offset of end of data for this sport
        public string GetDefaultData();
        // Return a default string for this sport to avoid uninitialized responses prior to console providing full update
        public string GetData(ref string data);
        // Return JSON object with all game-related data of interest
        public string GetClocks(ref string data);
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
    }
}