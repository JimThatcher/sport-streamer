using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using System.Drawing;

namespace WebAPI.Controllers
{
    // Class to represent a Game from DB with date converted from long (DateTime.Ticks) to ISO date string
    public class GameLocal
    {
        public GameLocal()
        {
        }
        public GameLocal(Game _game) {
            id = _game.id;
            awayId = _game.awayId;
            homeId = _game.homeId;
            date = new DateTime(_game.date).ToString("yyyy-MM-dd");
        }
        public Game GetGame() {
            Game _game = new Game();
            _game.id = id;
            _game.awayId = awayId;
            _game.homeId = homeId;
            _game.date = DateTime.Parse(date).Ticks;
            return _game;
        }
        public long id { get; set; }
        public long awayId { get; set; }
        public long homeId { get; set; }
        public string date { get; set; } = DateTime.UtcNow.ToString("s");
    }
    // Class to represet a GameView from DB with date converted from long (DateTime.Ticks) to ISO date string
    public class GameViewLocal
    {
        public GameViewLocal()
        {
        }
        public GameViewLocal(GameView _game) {
            id = _game.id;
            gameDate = new DateTime(_game.gameDate).ToString("yyyy-MM-dd");
            HomeName = _game.HomeName;
            HomeColor = _game.HomeColor;
            HomeIcon = _game.HomeIcon;
            HomeRecord = _game.HomeRecord;
            GuestName = _game.GuestName;
            GuestColor = _game.GuestColor;
            GuestIcon = _game.GuestIcon;
            GuestRecord = _game.GuestRecord;
        }
        public GameView GetGameView() {
            GameView _game = new GameView();
            _game.id = id;
            _game.gameDate = DateTime.Parse(gameDate).Ticks;
            _game.HomeName = HomeName;
            _game.HomeColor = HomeColor;
            _game.HomeIcon = HomeIcon;
            _game.HomeRecord = HomeRecord;
            _game.GuestName = GuestName;
            _game.GuestColor = GuestColor;
            _game.GuestIcon = GuestIcon;
            _game.GuestRecord = GuestRecord;
            return _game;
        }
        public long id { get; set; }
        // public string gameDate { get; set; } = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd");
        public string gameDate { get; set; } = DateTime.UtcNow.ToString("s");
        public string HomeName { get; set; } = "Home";
        public string HomeColor { get; set; } = "#ff0000";
        public string HomeIcon { get; set; } = string.Empty;
        public string HomeRecord {get; set; } = "0-0";
        public string GuestName { get; set; } = "Guest";
        public string GuestColor { get; set; } = "#0000ff";
        public string GuestIcon { get; set; } = string.Empty;
        public string GuestRecord {get; set; } = "0-0";
    }


    [Route("rest/db")]
    [ApiController]
    public class GameDataController : ControllerBase
    {
        private readonly GameMgrContext _context;
        private string? _webRoot;

        private string AddAlpha(string color) {
            string baseColor = color;
            string newColor = color;
            if (baseColor.StartsWith('#')) {
                if (baseColor.Length == 9) {
                    return color;
                }
                newColor = baseColor + "80";
                while (newColor.Length < 9) {
                    newColor = "#0" + newColor.Substring(1);
                }
            }
            else if (baseColor.Length > 2) { // Let's assume this is a named color
                Color rgbaColor = Color.FromName(baseColor);
                newColor = String.Format("#{:X2}{:X2}{:X2}80", rgbaColor.R, rgbaColor.G, rgbaColor.B);
            }
            return newColor;
        }

        public GameDataController(GameMgrContext context)
        {
#pragma warning disable 8600
            _context = context;
            _webRoot = (string) AppDomain.CurrentDomain.GetData("WebRootPath");
#pragma warning restore 8600
        }

        /*
        // GET: rest/db/gamesraw
        [HttpGet("gamesraw")]
        public async Task<ActionResult<IEnumerable<GameConfig>>> GetGameSetup()
        {
            if (_context.GameSetup == null) {
                return NotFound();
            }
            return await _context.GameSetup.ToListAsync();
        }

        // GET: rest/db/gameraw/5
        [HttpGet("gameraw/{id}")]
        public async Task<ActionResult<GameConfig>> GetGameConfig(long id)
        {
            if (_context.GameSetup == null) {
                return NotFound();
            }
            var gameConfig = await _context.GameSetup.FindAsync(id);

            if (gameConfig == null)
            {
                return NotFound();
            }

            return gameConfig;
        }
        */

        // GET: rest/db/schools
        [HttpGet("schools")]
        public async Task<ActionResult<IEnumerable<School>>> GetSchools()
        {
            if (_context.Schools == null) {
                return NotFound();
            }
            return await _context.Schools.ToListAsync();
        }

        // GET: rest/db/school/5
        [HttpGet("school/{id}")]
        public async Task<ActionResult<School>> GetSchool(long id)
        {
            if (_context.Schools == null) {
                return NotFound();
            }
            var schoolInfo = await _context.Schools.FindAsync(id);

            if (schoolInfo == null)
            {
                return NotFound();
            }
            return schoolInfo;
        }

        // POST: rest/db/school
        [HttpPost("school")]
        public async Task<ActionResult<School>> PostSchool(School school)
        {
            if (_context.Schools == null) {
                return NotFound();
            }
            _context.Schools.Add(school);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSchool), new { id = school.id }, school);
        }

        // POST: rest/db/logo/{imageName}
        // Uploads the school's logo to the server. The imageName is the name of the image file.
        // Save the image to the server in the /Images/ directory.
        [HttpPost("logo/{imageName}")]
        public async Task<ActionResult> PostLogoImage(string imageName, IFormFile image)
        {
          if (image == null || !imageName.EndsWith(".png"))
          {
            return BadRequest("Invalid image type");
          }
          // var webRootPath = (string) AppDomain.CurrentDomain.GetData("WebRootPath");
          if (_webRoot == null)
          {
            return StatusCode(StatusCodes.Status500InternalServerError, "No WebRootPath found");
          }
          // TODO: Allow app configuration of image directory.
          // TODO: Allow upload of multiple images.
          var path = Path.Combine(_webRoot, "Images", imageName);
          using (var stream = new FileStream(path, FileMode.Create))
          {
            await image.CopyToAsync(stream);
          }
          return Ok();
        }

        // PUT: rest/db/school/5
        [HttpPut("school/{id}")]
        public async Task<IActionResult> PutSchool(long id, School school)
        {
            if (id != school.id)
            {
                return BadRequest();
            }
            _context.Entry(school).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Schools == null || !_context.Schools.Any(e => e.id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(school);
        }

        // DELETE: rest/db/school/5
        [HttpDelete("school/{id}")]
        public async Task<IActionResult> DeleteSchool(long id)
        {
            if (_context.Schools == null) {
                return NotFound();
            }
            var school = await _context.Schools.FindAsync(id);
            if (school == null)
            {
                return NotFound();
            }

            _context.Schools.Remove(school);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /*
        // GET: rest/db/TempSchool/5
        [HttpGet("TempSchool")]
        public async Task<ActionResult<TempSchool>> GetTempSchool()
        {
            if (_context.TempSchool == null) {
                return NotFound();
            }
            var schoolInfo = await _context.TempSchool.FindAsync((long) 1);

            if (schoolInfo == null)
            {
                return NotFound();
            }
            if (schoolInfo.color.Length < 9)
                schoolInfo.color = AddAlpha(schoolInfo.color);
            return schoolInfo;
        }
        */
        // GET: rest/db/games
        [HttpGet("games")]
        public async Task<ActionResult<IEnumerable<GameLocal>>> GetGames()
        {
            if (_context.Games == null) {
                return NotFound();
            }
            var _games = await _context.Games.ToListAsync();
            List<GameLocal> games = new List<GameLocal>();
            foreach (var game in _games)
            {
                games.Add(new GameLocal(game));
            }
            return games;
        }

        // GET: rest/db/gamelist
        [HttpGet("gamelist")]
        public async Task<ActionResult<IEnumerable<GameViewLocal>>> GetGameList()
        {
            if (_context.Games == null || _context.Schools == null) {
                return NotFound();
            }
            var results = (from a in _context.Schools join g in _context.Games on a.id equals g.awayId join h in _context.Schools on g.homeId equals h.id orderby g.date
                            select new {g.id, gameDate=g.date, GuestName=a.mascot, GuestColor=a.color, GuestIcon=a.logo, GuestRecord=(a.win + "-" + a.loss),
									HomeName=h.mascot, HomeColor=h.color, HomeIcon=h.logo, HomeRecord=(h.win + "-" + h.loss)});
            List<GameViewLocal> games = new List<GameViewLocal>();
            await results.ForEachAsync((_this) => {
                GameViewLocal gameView = new GameViewLocal();
                gameView.id = (long) _this.id;
                gameView.gameDate = new DateTime(_this.gameDate).ToString("yyyy-MM-dd");
                gameView.GuestName = _this.GuestName;
                gameView.GuestColor = _this.GuestColor;
                gameView.GuestIcon = _this.GuestIcon;
                gameView.GuestRecord = _this.GuestRecord;
                gameView.HomeName = _this.HomeName;
                gameView.HomeColor = _this.HomeColor;
                gameView.HomeIcon = _this.HomeIcon;
                gameView.HomeRecord = _this.HomeRecord;
                games.Add(gameView);
            });
            return games;
        }

        // GET: rest/db/game/5
        [HttpGet("game/{id}")]
        public async Task<ActionResult<GameViewLocal>> GetGameView(long id)
        {
            if (_context.Games == null || _context.Schools == null) {
                return NotFound();
            }
            var result = (from a in _context.Schools join g in _context.Games on a.id equals g.awayId 
                        join h in _context.Schools on g.homeId equals h.id orderby g.date
                        select new {g.id, gameDate=g.date, GuestName=a.mascot, GuestColor=a.color, GuestIcon=a.logo, GuestRecord=(a.win + "-" + a.loss),
                        HomeName=h.mascot, HomeColor=h.color, HomeIcon=h.logo, HomeRecord=(h.win + "-" + h.loss)}).Where(g => g.id == id).Take(1);
            if (result == null)
            {
                return NotFound();
            }
            var _first = await result.FirstAsync();
            GameViewLocal gameView = new GameViewLocal();
            gameView.id = _first.id;
            gameView.gameDate = new DateTime(_first.gameDate).ToString("yyyy-MM-dd");
            gameView.GuestName = _first.GuestName;
            gameView.GuestColor = _first.GuestColor;
            gameView.GuestIcon = _first.GuestIcon;
            gameView.GuestRecord = _first.GuestRecord;
            gameView.HomeName = _first.HomeName;
            gameView.HomeColor = _first.HomeColor;
            gameView.HomeIcon = _first.HomeIcon;
            gameView.HomeRecord = _first.HomeRecord;
            if (gameView.HomeColor.Length < 9)
                gameView.HomeColor = AddAlpha(gameView.HomeColor);
            if (gameView.GuestColor.Length < 9)
                gameView.GuestColor = AddAlpha(gameView.GuestColor);
            return gameView;
        }

        // GET: rest/db/game/5
        [HttpGet("game/next")]
        public async Task<ActionResult<GameViewLocal>> GetNextGame()
        {
            if (_context.Games == null || _context.Schools == null) {
                return NotFound();
            }
            var results = (from a in _context.Schools join g in _context.Games on a.id equals g.awayId 
                        join h in _context.Schools on g.homeId equals h.id orderby g.date
                        select new {g.id, gameDate=g.date, GuestName=a.mascot, GuestColor=a.color, GuestIcon=a.logo, GuestRecord=(a.win + "-" + a.loss),
                        HomeName=h.mascot, HomeColor=h.color, HomeIcon=h.logo, HomeRecord=(h.win + "-" + h.loss)}).Where(_g => _g.gameDate >= DateTime.Now.AddHours(-3).Ticks).Take(1);
            if (results == null)
            {
                return NotFound();
            }
            GameViewLocal game = new GameViewLocal();
            var _first = await results.FirstAsync();
            game.id = _first.id;
            game.gameDate = new DateTime(_first.gameDate).ToString("yyyy-MM-dd");;
            game.GuestName = _first.GuestName;
            game.GuestColor = _first.GuestColor;
            game.GuestIcon = _first.GuestIcon;
            game.GuestRecord = _first.GuestRecord;
            game.HomeName = _first.HomeName;
            game.HomeColor = _first.HomeColor;
            game.HomeIcon = _first.HomeIcon;
            game.HomeRecord = _first.HomeRecord;
            if (game.HomeColor.Length < 9)
                game.HomeColor = AddAlpha(game.HomeColor);
            if (game.GuestColor.Length < 9)
                game.GuestColor = AddAlpha(game.GuestColor);
            return game;
        }
        /*
        // GET: rest/db/game/5/1
        [HttpGet("game/{homeId}/{guestId}")]
        public async Task<ActionResult<TempGame>> GetTempGame(long homeId, long guestId)
        {
            if (_context.TempGame == null) {
                return NotFound();
            }
            var tempGame = await _context.TempGame.Where(h => h.homeId == homeId).Where(a => a.awayId == guestId).FirstOrDefaultAsync();

            if (tempGame == null)
            {
                return NotFound();
            }
            if (tempGame.HomeColor.Length < 9)
                tempGame.HomeColor = AddAlpha(tempGame.HomeColor);
            if (tempGame.GuestColor.Length < 9)
                tempGame.GuestColor = AddAlpha(tempGame.GuestColor);
            return tempGame;
        }
        */
        // PUT: rest/db/game/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("game/{id}")]
        public async Task<IActionResult> PutGame(long id, GameLocal game)
        {
            if (id != game.id || game.awayId == (long) 0 || game.homeId == (long) 0)
            {
                return BadRequest();
            }
            Game _game = game.GetGame();
            _context.Entry(_game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Games == null || !_context.Games.Any(e => e.id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(_game);
        }

        // POST: rest/db/game
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("game")]
        public async Task<ActionResult<Game>> PostGame(GameLocal game)
        {
            if (_context.Games == null) {
                return NotFound();
            }
            Game _game = game.GetGame();
            _context.Games.Add(_game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGameView), new { id = game.id }, _game);
        }

        // DELETE: rest/db/game/5
        [HttpDelete("game/{id}")]
        public async Task<IActionResult> DeleteGame(long id)
        {
            if (_context.Games == null) {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
