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

namespace WebAPI.Controllers
{
    // This controller is used to manage the game-time in-memory scoreboard data for a game.
    // It is currently focused on American Football, but can be expanded to other sports.
    // This is primarily intended for situations where you are not able to connect to a Daktronics
    // contrtoller to get the game-time data from the controller's serial RTD stream. Because
    // this controller shares the same in-memory data used by the DakAccess parser, if you have
    // a connection to a Daktronics controller, all updates from the controller will overwrite
    // changes you try to make through the APIs provided here.
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
                var scoreData = new DbScoreData();
                scoreData.id = 1;
                if (_context.ScoreboardData != null) {
                    _context.ScoreboardData.Add(scoreData);
                }
                _context.SaveChanges();
            }
        }

        public Task UpdateBrowserClients(DbScoreData scoreData)
        {
            ServerSentEvent sse = new ServerSentEvent();
            sse.Type = "score";
            // var scoreData = await _context.ScoreboardData.FindAsync((long) 1);
            sse.Data = new List<string>(new string[] {JsonSerializer.Serialize(scoreData)} );
            return _sseService.SendEventAsync(sse);
        }

        // GET: api/score
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DbScoreData>>> GetScoreboardData()
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            return await _context.ScoreboardData.ToListAsync();
        }

        // GET: api/score/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DbScoreData>> GetScoreData(long id)
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

        // GET: api/score/ChangePoss
        [HttpGet("ChangePoss/{side=Switch}")]
        public async Task<ActionResult<DbScoreData>> ChangePossession(string side)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/score/Flag
        // Sets the "Flag" parameter to true for 10 seconds, then clears it
        // TODO: Change to support penalties against home or guest team.
        [HttpGet("Flag")]
        public async Task<ActionResult<DbScoreData>> ThrowFlag()
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
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
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
        }

        // GET: api/score/TimeOut/Home
        // Sets the "HomeTimeOut" or "GuestTimeOut" parameter to true for 10 seconds, then clears it
        [HttpGet("Timeout/{team}")]
        public async Task<ActionResult<DbScoreData>> CallTimeOut(string team)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/score/Down/Up
        [HttpGet("Down/{upDown}")]
        public async Task<ActionResult<DbScoreData>> ChangeDown(string upDown)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1) as DbScoreData;

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            if (upDown == "Up" && scoreData.Dn < 4) {
                scoreData.Dn++;
            }
            else if (upDown == "Down") {
                if (scoreData.Dn >= 1)
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/score/ToGo/6
        [HttpGet("ToGo/{addTo}")]
        public async Task<ActionResult<ScoreData>> ChangeToGo(int addTo)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/score/HomeScore/6
        [HttpGet("HomeScore/{addTo}")]
        public async Task<ActionResult<DbScoreData>> ChangeHomeScore(int addTo)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/score/GuestScore/6
        [HttpGet("GuestScore/{addTo}")]
        public async Task<ActionResult<DbScoreData>> ChangeGuestScore(int addTo)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/score/Quarter/Up
        [HttpGet("Quarter/{upDown}")]
        public async Task<ActionResult<DbScoreData>> ChangeQuarter(string upDown)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/score/HomeTOL/Down
        [HttpGet("HomeTOL/{upDown}")]
        public async Task<ActionResult<DbScoreData>> ChangeHomeTOL(string upDown)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/score/GuestTOL/Down
        [HttpGet("GuestTOL/{upDown}")]
        public async Task<ActionResult<DbScoreData>> ChangeGuestTOL(string upDown)
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        // GET: api/score/Reset
        [HttpGet("Reset")]
        public async Task<ActionResult<DbScoreData>> Reset()
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            var scoreData = await _context.ScoreboardData.FindAsync((long) 1);

            if (scoreData == null)
            {
                scoreData = new DbScoreData();
                scoreData.id = 1;
                _context.ScoreboardData.Add(scoreData);
            }
            scoreData.Hs = 0;
            scoreData.Gs = 0;
            scoreData.Htol = 3;
            scoreData.Gtol = 3;
            scoreData.Dn = 0;
            scoreData.Dt = 0;
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
            await UpdateBrowserClients(scoreData);
            return scoreData;
        }

        [HttpGet("Setup")]
        public async Task<ActionResult<DbScoreData>> SetupScoreData()
        {
            if (_context.ScoreboardData == null) {
                return NotFound();
            }
            DbScoreData scoreData = new DbScoreData();
            _context.ScoreboardData.Add(scoreData);
            await _context.SaveChangesAsync();
            
            await UpdateBrowserClients(scoreData);

            return CreatedAtAction(nameof(GetScoreData), new { id = scoreData.id }, scoreData);
        }

        // POST: api/score
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DbScoreData>> PostScoreData(DbScoreData scoreData)
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
