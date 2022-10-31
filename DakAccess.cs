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
using System.Text;
using System.IO.Ports;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using WebAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Lib.AspNetCore.ServerSentEvents;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace DakAccess
{
    public class Clocks {
        private string _main = "";
        private string _play = "";
        private string _to = "";
        public Clocks(){}
        public Clocks(string main, string play, string timeout) {
            _main = main;
            _play = play;
            _to = timeout;
        }
        public string Clk { get { return _main; } }
        public string Pck { get { return _play; } }
        public string ToC { get { return _to; } }
    }
    public class Teams {
        private string _home = "HOME";
        private string _guest = "GUEST";
        private string _homeShort = "H";
        private string _guestShort = "G";
        public Teams(){}
        public Teams(string home, string guest, string homeShort, string guestShort) {
            _home = home;
            _guest = guest;
            _homeShort = homeShort;
            _guestShort = guestShort;
        }
        public string htl { get { return _home; } }
        public string gtl { get { return _guest; } }
        public string hts { get { return _homeShort; } }
        public string gts { get { return _guestShort; } }
    }
    enum MessageSection {
        Prefix = 0,
        Header = 1,
        Update = 2,
        Checksum = 3,
        End = 4,
        Unknown = 5
    }
    public class ConsoleData : IHostedService {
        // This class is the parser for the Daktronics serial data stream. It is implemented
        // as a hosted service so that it can run independently of the web server.
        // It implements rest endpoints for the web server to use to get the data, and to 
        // start and stop the parser from attempting to connect to the serial port. By default
        // it will attempt to connect to the serial port every 10 seconds when the server is
        // started. APIs in the ScoreData controller will start and stop the timer used to 
        // attempt to connect to the serial port.
        // This class also implements a timer that can be used when a connection to the scoreboard
        // controller is not available. The timer is accessed through the ClockData controller.
        private readonly ILogger<ConsoleData> _logger;
        // This timer is initially used to find a serial port and attempt to connect through it to the console.
        // TODO: Once a console is connected, convert this to a watchdog timer that will tell us if no data is
        // received from the console. Daktronics consoles send data every second, so if we don't receive data
        // from the console for at least 10 seconds, we can assume it is disconnected.
        private Timer? _consoleConnectTimer = null;
        private bool _ConnectedToConsole = false;
        private DateTime _LastDataReceived = DateTime.Now;
        private CancellationToken _token;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServerSentEventsService _sseService;
        // private string _clockJson;
        // private string _scoreJson;
        // Timer for managing manual clock updates when no console is connected.
        private Timer? _clockTimer = null;
        private long _lastClockValue = 0;

        public ConsoleData(ILogger<ConsoleData> logger, IServiceScopeFactory scopeFactory, IServerSentEventsService serverSentEventsService) {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _sseService = serverSentEventsService;
            _sseService.ClientConnected += SseService_ClientConnected;
            _sseService.ClientDisconnected += SseService_ClientDisconnected;
            // _context = context;
            _ConnectedToConsole = false;
            // _clockJson = "";
            // _scoreJson = "";
        }
        private async void SseService_ClientConnected(object? sender, ServerSentEventsClientConnectedArgs e)
        {
            HttpRequest request = e.Request; 
            IServerSentEventsClient client = e.Client;
            _logger.LogInformation("SSE Client connected: " + request.Host + request.Path);
            ServerSentEvent evt = new ServerSentEvent();
            evt.Type = "clock";
            ClockData cd = GetClockFromDatabase();
            _lastClockValue = cd.Clk / 100; // Convert to tenths of seconds
            evt.Data = new List<string>(new string[] {cd.asJson()});
            _logger.LogInformation("Sending clock: " + evt.Data[0]);
            await client.SendEventAsync(evt);
            evt.Type = "score";
            evt.Data = new List<string>(new string[] {GetScoreFromDatabase()});
            _logger.LogInformation("Sending score: " + evt.Data[0]);
            await client.SendEventAsync(evt);
            // TODO: Send Teams to client also.
        }
        private void SseService_ClientDisconnected(object? sender, ServerSentEventsClientDisconnectedArgs e)
        {
            _logger.LogInformation("SSE Client disconnected: " + e.Request.Host + e.Request.Path);
            // Console.WriteLine("Client disconnected");
        }

        // Methods to manage the clock timer. These will be called from the ConsoleController.
        public void StartClockTimer() {
            if (_clockTimer == null) {
                _clockTimer = new Timer(ManualClockTimer_Tick, null, 0, 100);
            } else {
                _clockTimer.Change(0, 100);
            }
        }
        public void StopClockTimer() {
            if (_clockTimer != null) {
                _clockTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
        private async void ManualClockTimer_Tick(object? state) {
            // Check if we have a console connected. There is no reason to post manual clock SSE updates
            // if we have a console connected.
            if (!_ConnectedToConsole) {
                ClockData cd = GetClockFromDatabase();
                if (_lastClockValue == 0 && cd.Clk > 0) {
                    _lastClockValue = (cd.Clk / 100) + 10; // Convert to tenths of seconds
                }
                // Now check if the clock value has changed. Our clock is in tenths of seconds, but we only
                // send updates to the web client when the clock value changes by a second until we are 
                // inside the last minute of a quarter, then we send updates on each tick.
                if (cd.Clk > 0 && (cd.Clk < 60000 || (cd.Clk / 100) <= _lastClockValue - 10)) {
                    // We have a clock update. Send it to the client.
                    _lastClockValue = cd.Clk / 100;
                    ServerSentEvent evt = new ServerSentEvent();
                    evt.Type = "clock";
                    evt.Data = new List<string>(new string[] {cd.asJson()});
                    // _logger.LogInformation("Sending clock: " + evt.Data[0]);
                    await _sseService.SendEventAsync(evt);
                }
            }
        }

        private ClockData GetClockFromDatabase() {
            using (var scope = _scopeFactory.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<GameContext>();
                if (dbContext.Clocks == null) {
                    ClockData newClock = new ClockData();
                    newClock.Clk = 12 * 60000;
                    newClock.Pck = 20000;
                    newClock.isRunning = false;
                    newClock.lastChange = DateTime.UtcNow;
                    return newClock;
                }
                ClockData? clockData = dbContext.Clocks.Find((long) 1);

                if (clockData == null)
                {
                    clockData = new ClockData();
                    clockData.Clk = 12 * 60000;
                    clockData.Pck = 20000;
                    clockData.isRunning = false;
                    clockData.lastChange = DateTime.UtcNow;
                    dbContext.Clocks.Add(clockData);
                    dbContext.SaveChanges();
                }
                return clockData;
            }
        }

        private string GetScoreFromDatabase() {
            using (var scope = _scopeFactory.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<GameContext>();
                if (dbContext.ScoreboardData == null) {
                    DbScoreData newScore = new DbScoreData();
                    return JsonSerializer.Serialize(newScore);
                }
                DbScoreData? scoreData = dbContext.ScoreboardData.Find((long) 1);

                if (scoreData == null)
                {
                    scoreData = new DbScoreData();
                    scoreData.id = 1;
                    try {
                        dbContext.ScoreboardData.Add(scoreData);
                    } catch (DbUpdateConcurrencyException e) {
                        _logger.LogError(e.Message);
                    }
                }
                return JsonSerializer.Serialize(scoreData);
            }
        }
        /*
        public string ClocksJson { get { return _clockJson; } }
        public string ScoreJson { get { return _scoreJson; } }
        */

        public bool IsConsoleConnected { get {return _ConnectedToConsole; }}

        // StartAsync and StopAsync implement the IHostedService interface.
        public Task StartAsync(CancellationToken stoppingToken) {
            _logger.LogInformation("ConsoleData Service running.");
            _token = stoppingToken;
            // _timer = new Timer(ConnectToConsole, stoppingToken, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken stoppingToken) {
            _logger.LogInformation("Stopping DakAccess ConsoleData");
            if (_consoleConnectTimer != null) {
                _consoleConnectTimer.Dispose();
            }
            _token.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
        
        // This method is called from the Startup.Configure method. This starts the console connect timer, which
        // will attempt to connect to the console every 10 seconds.
        public Task ConnectConsole() {
            _logger.LogInformation("ConsoleData ConnectConsole called.");
            if (_consoleConnectTimer == null) {
                _consoleConnectTimer = new Timer(ConnectToConsole, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            }
            return Task.CompletedTask;
        }

        // These methods are called from the ConsoleController to start or stop the _consoleConnectTimer.
        // TODO: Stop needs to free the COM port and set _ConnectedToConsole to false
        public void StartConsoleConnectTimer() {
            _logger.LogInformation("ConsoleData StartConsoleConnectTimer called.");
            if (_consoleConnectTimer != null) {
                _consoleConnectTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(10));
            }
            // return Task.CompletedTask;
        }
        public void StopConsoleConnectTimer() {
            _logger.LogInformation("ConsoleData StopConsoleConnectTimer called.");
            if (_consoleConnectTimer != null) {
                _consoleConnectTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }
            if (_ConnectedToConsole) {
                // We overload meaning of the _ConnectedToConsole flag here, testing if it's "false" in the ReadDataFromSerial()
                // function and disconnecting from the serial port if _ConnectedToConsole is changed to false.
                _ConnectedToConsole = false;
            }
            // return Task.CompletedTask;
        }

        private void ConnectToConsole(object? state) {
            if (_ConnectedToConsole)
                return;
            if (SerialPort.GetPortNames().Length == 0) {
                return;
            }
            SerialPort? _port = null;
            try {
                bool bFoundConsole = false;
                for (int i = 0; i < SerialPort.GetPortNames().Length; i++) {
                    _token.ThrowIfCancellationRequested();
                    _port = new SerialPort
                    {
                        PortName = (SerialPort.GetPortNames()[i]),
                        BaudRate = 19200,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One
                    };
                    _port.ReadTimeout = 2000;
                    _port.Open();
                    try {
                        char bCurr = (char) _port.ReadByte();
                        for (int n = 0; n < 1000; n++) {
                            // Read up to 1000 characters looking for a SYN token
                            if (bCurr == 0x16) {
                                bFoundConsole = true;
                                break;
                            }
                        }
                    } catch (TimeoutException) {
                        _logger.LogInformation("Timeout reading from COM{0}", i);
                    }
                    _port.Close();
                    if (bFoundConsole) {
                        break;
                    }
                }
            } catch (Exception ex) {
                _logger.LogError("Exception opening serial port {0}", ex.ToString());
            }
            if (_port != null) {
                _ConnectedToConsole = true;
                _logger.LogInformation("SerialPort connected to {0}", _port.PortName);
                // _port.ReadTimeout = 500;
                var t = Task.Run(() => ReadDataFromSerial(_port) );
                t.Wait();
            }
        }
        private void Update(string dataStr, int offset, ref string allData) {
            if (offset + dataStr.Length > allData.Length) {
                allData = allData + new string(' ', (offset + dataStr.Length) - allData.Length);
            }
            string prefix = (offset == 0) ? "" : allData.Substring(0, offset);
            string suffix = (allData.Length > (offset + dataStr.Length)) ? allData.Substring(offset + dataStr.Length) : "";
            allData = prefix + dataStr + suffix;
        }
        private ISportData GetSportInterface(string code) {
            switch (code) {
                case "6601":
                case "6604":
                case "6611":
                case "6612":
                case "6103":
                case "6104":
                case "6105":
                case "6402":
                case "6501":
                case "    ": // If the console is not producing a sport code, we assume football because we are biased that way ;-)
                    return new DakFootball(code);
                case "1101":
                case "1102":
                case "1103":
                case "1104":
                case "1105":
                case "1301":
                case "1401":
                case "1402":
                    return new DakBasketball(code);
                case "7701":
                case "7711":
                case "7611":
                case "7601":
                case "7501":
                case "7604":
                    return new DakSoccer(code);
                case "4000":
                case "4401":
                case "4402":
                case "4102":
                case "4103":
                case "4104":
                case "4105":
                case "4601":
                case "4602":
                case "4701":
                case "4702":
                    return new DakHockey(code);
                default:
                    return new DakDefault(code);
            }

        }

        private async void ReadDataFromSerial(SerialPort port)
        {
            string allData = new string(' ', 511); // Will hold the full info from the console
            try {
                port.Open();
                string header = "";
                string updateText = "";
                string chkStr = "00";
                byte checksum = 0;
                bool checkMatches = false;
                int nOffset = 0;
                int nTextLen = 0;
                // string currentSportCode = "1000";
                char bCurr = (char) port.ReadByte();
                MessageSection msgLoc = MessageSection.Unknown;
                ISportData dakData = GetSportInterface("1000"); // Let's default to basic, then update based on controller setting
                allData = dakData.GetDefaultData();
                Update(dakData.GetDefaultData(), 0, ref allData); // Do I really need both this and the previous line?
                // _scoreJson = dakData.GetData(ref allData);
                // _clockJson = dakData.GetClocks(ref allData);

                while (true) {
                    try {
                        if (_token.IsCancellationRequested || _ConnectedToConsole == false) {
                            _logger.LogInformation("Cancellation was requested, exiting read from console");
                            break;
                        }
                        while (bCurr != 0x16) {
                            bCurr = (char) port.ReadByte(); // Read to a SYN token to sync to a data frame
                        }
                        // We are at the beginning of a data frame, read each section of the frame
                        msgLoc = MessageSection.Prefix;
                        header = "";
                        updateText = "";
                        chkStr = "";
                        checksum = 0;
                        nOffset = -1;
                        nTextLen = 0;
                        bCurr = (char) port.ReadByte();
                        while ((byte) bCurr != 0x04) { // Read through EOT token to calculate checksum
                            checksum = (byte) (checksum + (byte) bCurr);
                            switch ((byte) bCurr) {
                                case 0x01: // SOH token
                                    msgLoc = MessageSection.Header;
                                    break;
                                case 0x02: // STX token
                                    msgLoc = MessageSection.Update;
                                    break;
                                case 0x04: // EOT token
                                    msgLoc = MessageSection.Checksum;
                                    break;
                                case 0x17: // ETB token
                                    msgLoc = MessageSection.End;
                                    break;
                                default:
                                    break;
                            }
                            if ((byte)bCurr > 0x17) // Only run this section if not a control token
                            {
                                switch (msgLoc) {
                                    case MessageSection.Header:
                                        header = string.Format("{0}{1}", header, bCurr);
                                        break;
                                    case MessageSection.Update:
                                        updateText = string.Format("{0}{1}", updateText, bCurr);
                                        nTextLen++;
                                        break;
                                    case MessageSection.Prefix: // just skip the byte, and move on
                                    default:
                                        break;
                                }
                            }
                            bCurr = (char) port.ReadByte();
                        }
                        checksum = (byte) (checksum + (byte) bCurr);
                        msgLoc = MessageSection.Checksum;
                        chkStr = "";
                        bCurr = (char) port.ReadByte();
                        while ((byte) bCurr != 0x17) {
                            chkStr = string.Format("{0}{1}", chkStr, bCurr);
                            bCurr = (char) port.ReadByte();
                        }
                        if (header.StartsWith("00421")) {
                            // This is a scoreboard update frame
                            try {
                                if (header.Length == 10) {
                                    nOffset = Int16.Parse(header.Substring(6));
                                    if (nTextLen != updateText.Length) {
                                        _logger.LogWarning(String.Format("Length not equal. {0}-{1}", nTextLen, updateText.Length));
                                    }
                                    if (chkStr == checksum.ToString("X2")) {
                                        checkMatches = true;
                                    }
                                    else {
                                        checkMatches = false;
                                        _logger.LogWarning(String.Format("Checksum failure. {0}-{1}", chkStr, checksum.ToString("X2")));
                                    }
                                    if (checkMatches) {
                                        _LastDataReceived = DateTime.Now;
                                        // Temporarily blank out sport code and console data for testing
                                        // TODO: Remove this when we are ready to go live
                                        if (updateText.Length > 187) {
                                            if (updateText.Length > 193) {
                                                updateText = updateText.Remove(183, 11);
                                                updateText = updateText.Insert(183, "           ");
                                            } else {
                                                updateText = updateText.Remove(183, 4);
                                                updateText = updateText.Insert(183, "    ");
                                            }
                                        }
                                        if (updateText.Length > 187 && dakData.GetSportCode() != updateText.Substring(183, 4)) {
                                            // The console is set to a different sport than previous setting
                                            // This allows the parser to automatically adapt to changes in the
                                            dakData = GetSportInterface(updateText.Substring(183, 4));
                                            // currentSportCode = updateText.Substring(183, 4);
                                            Update(updateText, nOffset, ref allData);
                                            dakData.UpdateData(updateText.Length, nOffset, ref allData);
                                            using (var scope = _scopeFactory.CreateScope()) {
                                                var dbContext = scope.ServiceProvider.GetRequiredService<GameContext>();
                                                if (dbContext != null) {
                                                    if (dbContext.ConsoleVersion != null) {
                                                        if (dbContext.ConsoleVersion.Count() == 0)
                                                            dbContext.ConsoleVersion.Add(dakData.GetConsoleInfo());
                                                        else
                                                            dbContext.ConsoleVersion.Update(dakData.GetConsoleInfo());
                                                        dbContext.SaveChanges();
                                                    }
                                                }
                                            }
                                        }
                                        Update(updateText, nOffset, ref allData);
                                        dakData.UpdateData(updateText.Length, nOffset, ref allData);
                                        using (var scope = _scopeFactory.CreateScope()) {
                                            var dbContext = scope.ServiceProvider.GetRequiredService<GameContext>();
                                            if (nOffset > 0 || updateText.Length > 32)
                                                _logger.LogInformation(String.Format("Dak Update: {0}, Clock updated: {1}, Score updated: {2}", updateText, dakData.ClockUpdated(), dakData.DataUpdated() ));
                                            if (dakData.ClockUpdated()) {
                                                ClockData clock = dakData.GetClockData();
                                                string _clock = JsonSerializer.Serialize(clock);
                                                if (dbContext != null && dbContext.Clocks != null) {
                                                    if (dbContext.Clocks.Count() == 0) {
                                                        dbContext.Clocks.Add(clock);
                                                    } else {
                                                        dbContext.Clocks.Update(clock);
                                                    }
                                                    dbContext.SaveChanges();
                                                }
                                                ServerSentEvent evt = new ServerSentEvent();
                                                evt.Type = "clock";
                                                evt.Data = new List<string>(new string[] {clock.asJson()});
                                                await _sseService.SendEventAsync(evt);
                                                _logger.LogTrace(_clock);
                                            }
                                            if (dakData.DataUpdated()) {
                                                string _score = JsonSerializer.Serialize(dakData.GetScoreData());
                                                if (dbContext != null && dbContext.ScoreboardData != null) {
                                                    if (dbContext.ScoreboardData.Count() == 0) {
                                                        dbContext.ScoreboardData.Add(dakData.GetScoreData());
                                                    } else {
                                                        dbContext.ScoreboardData.Update(dakData.GetScoreData());
                                                    }
                                                    dbContext.SaveChanges();
                                                }
                                                ServerSentEvent evt = new ServerSentEvent();
                                                evt.Type = "score";
                                                evt.Data = new List<string>(new string[] {_score});
                                                await _sseService.SendEventAsync(evt);
                                                _logger.LogTrace(_score);
                                            }
                                        }
                                    }
                                }
                                else {
                                    _logger.LogWarning("Header length was wrong. {0}", header);
                                }
                            }
                            catch (Exception ex) {
                                _logger.LogWarning(ex.ToString());
                                _logger.LogWarning(String.Format("Header={0}, Data={1}", header, dakData));
                            }
                        }
                        msgLoc = MessageSection.End;
                    }
                    catch (TimeoutException) {
                        _logger.LogInformation("Read from serial port {0} timed out.", port.PortName);
                        DateTime now = DateTime.Now;
                        if (now.Subtract(_LastDataReceived).TotalSeconds > 10) {
                            _logger.LogInformation("No data received for 10 seconds, restarting search for serial port");
                            port.Close();
                            _ConnectedToConsole = false;
                            break;
                        }
                    }
                }
            } catch(Exception ex) {
                _logger.LogWarning(ex.ToString());
            }
            port.Close(); 
            _ConnectedToConsole = false;
        }
    }
}
