using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using DakAccess;
using System.Text.Json;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Logging;

// TODO: Update to send SSE updates every second while clock is running
namespace WebAPI.Controllers
{
    [Route("api/clock")]
    [ApiController]
    public class ClockDataController : ControllerBase
    {
        private readonly GameContext _context;
        // private readonly IServiceScopeFactory _scopeFactory;
        ConsoleData _console;
        private readonly IServerSentEventsService _sseService;
        private readonly ILogger<ClockDataController> _logger;



        // public ClockDataController(GameContext context, IServiceScopeFactory scopeFactory)
        public ClockDataController(GameContext context, ConsoleData console, IServerSentEventsService serverSentEventsService, ILogger<ClockDataController> logger)
        {
            _context = context;
            _console = console;
            _logger = logger;
            _sseService = serverSentEventsService;
            // _scopeFactory = scopeFactory;
            if (!ClockDataExists(1)) {
                ClockData clockData = new ClockData();
                clockData.Clk = 12 * 60000;
                clockData.Pck = 20000;
                clockData.isRunning = false;
                clockData.lastChange = DateTime.UtcNow;
                if (_context.Clocks != null)
                    _context.Clocks.Add(clockData);
                _context.SaveChanges();
            }
        }

        public async void UpdateBrowserClients(ClockData clockData)
        {
            ServerSentEvent sse = new ServerSentEvent();
            sse.Type = "clock";
            sse.Data = new List<string>(new string[] {clockData.asJson()} );
            await _sseService.SendEventAsync(sse);
        }

        // GET: api/ClockData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClockData>>> GetClocks()
        {
            if (_context.Clocks != null) {
                var clocks = await _context.Clocks.ToListAsync();
                if (clocks.Count > 0) {
                    return clocks;
                }
            }
            return NotFound();
        }

        // GET: api/ClockData/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClockDataDak>> GetClockData(long id)
        {
            if (_context.Clocks != null) {
                var clockData = await _context.Clocks.FindAsync(id);
                if (clockData != null) {
                    return new ClockDataDak(clockData);
                }
            }
            return NotFound();
        }

        // GET: api/ClockData/Start
        
        [HttpGet("Start")]
        public async Task<ActionResult<ClockDataDak>> StartClock()
        {
            if (_context.Clocks != null) {
                var clockData = await _context.Clocks.FindAsync((long) 1);

                if (clockData == null)
                {
                    clockData = new ClockData();
                    clockData.Clk = 12 * 60000;
                    clockData.Id = 1;
                    clockData.Pck = 20000;
                    _context.Clocks.Add(clockData);
                }
                clockData.isRunning = true;
                clockData.lastChange = DateTime.UtcNow;

                _context.Entry(clockData).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogError("Concurrency Error starting clock");
                }

                UpdateBrowserClients(clockData);
                _console.StartClockTimer();
                return new ClockDataDak(clockData);
            }
            return NotFound();
        }

        // GET: api/ClockData/Stop
        [HttpGet("Stop")]
        public async Task<ActionResult<ClockDataDak>> StopClock()
        {
            if (_context.Clocks != null) {
                var clockData = await _context.Clocks.FindAsync((long) 1);

                if (clockData == null)
                {
                    clockData = new ClockData();
                    clockData.Clk = 12 * 60000;
                    clockData.Id = 1;
                    clockData.Pck = 20000;
                    clockData.isRunning = true; // Force fall-through to standard path
                    _context.Clocks.Add(clockData);
                }
                if (clockData.isRunning) {
                    clockData.isRunning = false;
                    DateTime now = DateTime.UtcNow;
                    TimeSpan elapsed = now - clockData.lastChange;
                    clockData.Clk = (long) ((elapsed.TotalMilliseconds > clockData.Clk) ? 0 : (clockData.Clk - elapsed.TotalMilliseconds));
                    clockData.lastChange = now;

                    _context.Entry(clockData).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogError("Concurrentcy Error stopping clock");
                    }
                }

                UpdateBrowserClients(clockData);
                _console.StopClockTimer();
                return new ClockDataDak(clockData);
            }
            return NotFound();
        }

        // GET: api/ClockData/Round/5
        // Round the current clock up or down to the nearest number of seconds
        [HttpGet("Round/{seconds=10}")]
        public async Task<ActionResult<ClockData>> RoundSeconds(int seconds)
        {
            if (_context.Clocks != null) {
                var clockData = await _context.Clocks.FindAsync((long) 1);

                if (clockData == null)
                {
                    return NotFound();
                }
                if (clockData.isRunning) {
                    DateTime now = DateTime.UtcNow;
                    TimeSpan elapsed = now - clockData.lastChange;
                    clockData.isRunning = false;
                    clockData.Clk = (long) ((elapsed.TotalMilliseconds > clockData.Clk) ? 0 : (clockData.Clk - elapsed.TotalMilliseconds));
                    clockData.isRunning = true;
                    clockData.lastChange = now;
                }
                clockData.Clk = (long)((long) Math.Round((decimal) clockData.Clk / (seconds * 1000))) * (seconds * 1000);

                _context.Entry(clockData).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                }

                UpdateBrowserClients(clockData);
                return clockData;
            }
            return NotFound();
        }

        // GET: api/ClockData/Adjust/5
        // Adjust the current clock up or down the number of seconds
        [HttpGet("Adjust/{seconds=1}")]
        public async Task<ActionResult<ClockData>> AdjustSeconds(int seconds)
        {
            if (_context.Clocks != null) {
                var clockData = await _context.Clocks.FindAsync((long) 1);

                if (clockData == null)
                {
                    return NotFound();
                }
                if (clockData.isRunning) {
                    DateTime now = DateTime.UtcNow;
                    TimeSpan elapsed = now - clockData.lastChange;
                    clockData.isRunning = false;
                    clockData.Clk = (long) ((elapsed.TotalMilliseconds > clockData.Clk) ? 0 : (clockData.Clk - elapsed.TotalMilliseconds));
                    clockData.isRunning = true;
                    clockData.lastChange = now;
                    // clockData.lastChange = clockData.lastChange.AddSeconds(seconds);
                }
                clockData.Clk = clockData.Clk + (seconds * 1000);

                _context.Entry(clockData).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                }

                UpdateBrowserClients(clockData);
                return clockData;
            }
            return NotFound();
        }

        // GET: api/ClockData/dak/5
        [HttpGet("dak/{id=1}")]
        public async Task<ActionResult<ClockDataDak>> GetClockDataDak(long id)
        {
            if (_context.Clocks != null) {
                var clockData = await _context.Clocks.FindAsync(id);
                if (clockData != null) {
                    return new ClockDataDak(clockData);
                }
            }
            return NotFound();
        }

        // GET: api/ClockData/Setup/12
        [HttpGet("Setup/{minutes}")]
        public async Task<ActionResult<ClockData>> SetupClockData(int minutes)
        {
            _logger.LogTrace("SetupClockData called");
            /*
            var clockData = await _context.Clocks.FindAsync((long) 1);

            if (clockData == null)
            {
                clockData = new ClockData();
                clockData.Id = 1;
                _context.Clocks.Add(clockData);
            }
            clockData.Clock = minutes * 60000;
            clockData.PlayClock = 20000;
            clockData.isRunning = false;
            clockData.lastChange = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            UpdateBrowserClients(clockData);
            return CreatedAtAction(nameof(GetClockData), new { id = clockData.Id }, clockData);
            */
            return await ResetClockData(minutes, 0);
        }

        // GET: api/ClockData/Reset/12
        [HttpGet("Reset/{minutes}/{seconds=0}")]
        public async Task<ActionResult<ClockData>> ResetClockData(int minutes, int seconds)
        {
            if (_context.Clocks != null) {
                var clockData = await _context.Clocks.FindAsync((long) 1);

                if (clockData == null)
                {
                    clockData = new ClockData();
                    clockData.Id = 1;
                    _context.Clocks.Add(clockData);
                }
                clockData.Clk = minutes * 60000 + seconds * 1000;
                clockData.Pck = 20000;
                clockData.isRunning = false;
                clockData.lastChange = DateTime.UtcNow;

                _context.Entry(clockData).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClockDataExists(1))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                UpdateBrowserClients(clockData);
                return clockData;
            }
            return NotFound();
        }

        // PUT: api/ClockData/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClockData(long id, ClockData clockData)
        {
            if (id != clockData.Id)
            {
                return BadRequest();
            }

            _context.Entry(clockData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClockDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ClockData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClockData>> PostClockData(ClockData clockData)
        {
            if (_context.Clocks != null) {
                _context.Clocks.Add(clockData);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetClockData), new { id = clockData.Id }, clockData);
            }
            return StatusCode(500);
        }

        // DELETE: api/ClockData/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClockData(long id)
        {
            if (_context.Clocks != null) {
                var clockData = await _context.Clocks.FindAsync(id);
                if (clockData == null)
                {
                    return NotFound();
                }

                _context.Clocks.Remove(clockData);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            return StatusCode(500);
        }

        private bool ClockDataExists(long id)
        {
            if (_context.Clocks != null) {
                return _context.Clocks.Any(e => e.Id == id);
            }
            return false;
        }
    }
}
