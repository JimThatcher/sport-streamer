using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using DakAccess;
using System.Text.Json;
using Lib.AspNetCore.ServerSentEvents;

// TODO: Update this to use SSE. Remove old outputs and add a send of events to each change.
namespace WebAPI.Controllers
{
    [Route("api/score")]
    [ApiController]
    public class ScoreDataController : ControllerBase
    {
        private GameContext _context;
        ConsoleData _console;
        private readonly IServerSentEventsService _sseService;

        public ScoreDataController(GameContext context, ConsoleData console, IServerSentEventsService serverSentEventsService)
        {
            _context = context;
            _console = console;
            _sseService = serverSentEventsService;
            if (!ScoreDataExists(1)) {
                var scoreData = new ScoreData();
                scoreData.id = 1;
                if (_context.ScoreboardData != null) {
                    _context.ScoreboardData.Add(scoreData);
                }
                _context.SaveChanges();
            }
        }

        public async void UpdateBrowserClients(ScoreData scoreData)
        {
            ServerSentEvent sse = new ServerSentEvent();
            sse.Type = "score";
            // var scoreData = await _context.ScoreboardData.FindAsync((long) 1);
            sse.Data = new List<string>(new string[] {JsonSerializer.Serialize(scoreData)} );
            await _sseService.SendEventAsync(sse);
        }

        // GET: api/ScoreData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScoreData>>> GetScoreboardData()
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            return await _context.ScoreboardData.ToListAsync();
        }

        // GET: api/ScoreData/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScoreData>> GetScoreData(long id)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync(id);

            if (scoreData == null)
            {
                return NotFound();
            }

            return scoreData;
        }

        // GET: api/ScoreData/ChangePoss
        [HttpGet("ChangePoss/{side=Switch}")]
        public async Task<ActionResult<ScoreData>> ChangePossession(string side)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            if (side == "Switch") {
                if (scoreData.Gpo) {
                    scoreData.Gpo = false;
                    scoreData.Hpo = true;
                }
                else {
                    scoreData.Gpo = true;
                    scoreData.Hpo = false;
                }
            }
            else if (side == "Home") {
                scoreData.Hpo = true;
                scoreData.Gpo = false;
            }
            else if (side == "Guest") {
                scoreData.Hpo = false;
                scoreData.Gpo = true;
            } else if (side == "Clear") {
                scoreData.Hpo = false;
                scoreData.Gpo = false;
            }

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/ScoreData/Flag
        // Sets the "Flag" parameter to true for 10 seconds, then clears it
        // TODO: Change to support penalties against home or guest team.
        [HttpGet("Flag")]
        public async Task<ActionResult<ScoreData>> ThrowFlag()
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            scoreData.Fl = true;

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var tasks = new[] {
                Task.Run(() => WaitAndResetFlag())
            };
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        public async void WaitAndResetFlag() {
            System.Threading.Thread.Sleep(10000);

            GameContext dbContext = new GameContext();
            if (dbContext.ScoreboardData == null) {
                return;
            }
            var scoreData = await dbContext.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                dbContext.ScoreboardData.Add(scoreData);
            }

            scoreData.Fl = false;
            scoreData.Hto = false;
            scoreData.Gto = false;

            dbContext.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
            }
            UpdateBrowserClients(scoreData);
        }

        // GET: api/ScoreData/TimeOut/Home
        // Sets the "HomeTimeOut" or "GuestTimeOut" parameter to true for 10 seconds, then clears it
        [HttpGet("Timeout/{team}")]
        public async Task<ActionResult<ScoreData>> CallTimeOut(string team)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            if (team == "Home") {
                scoreData.Hto = true;
            }
            else if (team == "Guest") {
                scoreData.Gto = true;
            }

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var tasks = new[] {
                Task.Run(() => WaitAndResetFlag())
            };
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/ScoreData/Down/Up
        [HttpGet("Down/{upDown}")]
        public async Task<ActionResult<ScoreData>> ChangeDown(string upDown)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            if (upDown == "Up" && scoreData.Dn < 4) {
                scoreData.Dn++;
            }
            else if (upDown == "Down") {
                if (scoreData.Dn > 1)
                    scoreData.Dn--;
            }
            else if (upDown == "Reset") {
                scoreData.Dn = 1;
                scoreData.Dt = 10;
            }

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/ScoreData/ToGo/6
        [HttpGet("ToGo/{addTo}")]
        public async Task<ActionResult<ScoreData>> ChangeToGo(int addTo)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            scoreData.Dt = (addTo == 0) ? 10 : scoreData.Dt + addTo;

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/ScoreData/HomeScore/6
        [HttpGet("HomeScore/{addTo}")]
        public async Task<ActionResult<ScoreData>> ChangeHomeScore(int addTo)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            scoreData.Hs = (addTo == 0) ? 0 : scoreData.Hs + addTo;
            if (scoreData.Hs < 0)
                scoreData.Hs = 0;

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/ScoreData/GuestScore/6
        [HttpGet("GuestScore/{addTo}")]
        public async Task<ActionResult<ScoreData>> ChangeGuestScore(int addTo)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            scoreData.Gs = (addTo == 0) ? 0 : scoreData.Gs + addTo;
            if (scoreData.Gs < 0)
                scoreData.Gs = 0;

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/ScoreData/Quarter/Up
        [HttpGet("Quarter/{upDown}")]
        public async Task<ActionResult<ScoreData>> ChangeQuarter(string upDown)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            if (upDown == "Up") {
                scoreData.Pd++;
            }
            else if (upDown == "Down") {
                if (scoreData.Pd > 1)
                    scoreData.Pd--;
            }
            else if (upDown == "Reset") {
                scoreData.Pd = 1;
            }

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/ScoreData/HomeTOL/Down
        [HttpGet("HomeTOL/{upDown}")]
        public async Task<ActionResult<ScoreData>> ChangeHomeTOL(string upDown)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            if (upDown == "Up") {
                if (scoreData.Htol < 3)
                    scoreData.Htol++;
            }
            else if (upDown == "Down") {
                if (scoreData.Htol > 0)
                    scoreData.Htol--;
            }
            else if (upDown == "Reset") {
                scoreData.Htol = 3;
            }

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/ScoreData/GuestTOL/Down
        [HttpGet("GuestTOL/{upDown}")]
        public async Task<ActionResult<ScoreData>> ChangeGuestTOL(string upDown)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            if (upDown == "Up") {
                if (scoreData.Gtol < 3)
                    scoreData.Gtol++;
            }
            else if (upDown == "Down") {
                if (scoreData.Gtol > 0)
                    scoreData.Gtol--;
            }
            else if (upDown == "Reset") {
                scoreData.Gtol = 3;
            }

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/ScoreData/Reset
        [HttpGet("Reset")]
        public async Task<ActionResult<ScoreData>> Reset()
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new ScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            scoreData.Hs = 0;
            scoreData.Gs = 0;
            scoreData.Htol = 3;
            scoreData.Gtol = 3;
            scoreData.Dn = 1;
            scoreData.Dt = 10;
            scoreData.Pd = 1;
            scoreData.Hpo = false;
            scoreData.Gpo = false;
            scoreData.Hto = false;
            scoreData.Gto = false;
            scoreData.BO = 0;

            _context.Entry(scoreData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreDataExists(1))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            UpdateBrowserClients(scoreData);
            return scoreData;
        }

        [HttpGet("Setup")]
        public async Task<ActionResult<ScoreData>> SetupScoreData()
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            ScoreData scoreData = new ScoreData();
            _context.ScoreboardData.Add(scoreData);
            await _context.SaveChangesAsync();
            
            UpdateBrowserClients(scoreData);

            return CreatedAtAction(nameof(GetScoreData), new { id = scoreData.id }, scoreData);
        }

        // POST: api/ScoreData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ScoreData>> PostScoreData(ScoreData scoreData)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            _context.ScoreboardData.Add(scoreData);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetScoreData), new { id = scoreData.id }, scoreData);
        }

        private bool ScoreDataExists(long id)
        {
            if (_context.ScoreboardData == null) {
                return false;
            }
            return _context.ScoreboardData.Any(e => e.id == id);
        }
    }
}
