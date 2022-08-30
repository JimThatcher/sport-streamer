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

            return CreatedAtAction(nameof(GetGameConfig), new { id = school.id }, school);
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
                if (!GameConfigExists(id))
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

        // GET: rest/db/games
        [HttpGet("games")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            if (_context.Games == null) {
                return NotFound();
            }
            return await _context.Games.ToListAsync();
        }

        // GET: rest/db/gamelist
        [HttpGet("gamelist")]
        public async Task<ActionResult<IEnumerable<GameView>>> GetGameList()
        {
            if (_context.GameView == null) {
                return NotFound();
            }
            return await _context.GameView.ToListAsync();
        }

        // GET: rest/db/game/5
        [HttpGet("game/{id}")]
        public async Task<ActionResult<GameView>> GetGameView(long id)
        {
            if (_context.GameView == null) {
                return NotFound();
            }
            var gameConfig = await _context.GameView.FindAsync(id);

            if (gameConfig == null)
            {
                return NotFound();
            }
            if (gameConfig.HomeColor.Length < 9)
                gameConfig.HomeColor = AddAlpha(gameConfig.HomeColor);
            if (gameConfig.GuestColor.Length < 9)
                gameConfig.GuestColor = AddAlpha(gameConfig.GuestColor);
            return gameConfig;
        }

        // GET: rest/db/game/5
        [HttpGet("game/next")]
        public async Task<ActionResult<NextGame>> GetNextGame()
        {
            if (_context.NextGame == null) {
                return NotFound();
            }
            var game = await _context.NextGame.FirstOrDefaultAsync();

            if (game == null)
            {
                return NotFound();
            }
            if (game.HomeColor.Length < 9)
                game.HomeColor = AddAlpha(game.HomeColor);
            if (game.GuestColor.Length < 9)
                game.GuestColor = AddAlpha(game.GuestColor);
            return game;
        }

        // GET: rest/db/game/5/1
        [HttpGet("game/{homeId}/{guestId}")]
        public async Task<ActionResult<TempGame>> GetTempGame(long homeId, long guestId)
        {
            if (_context.TempGame == null) {
                return NotFound();
            }
            var gameConfig = await _context.TempGame.Where(h => h.homeId == homeId).Where(a => a.awayId == guestId).FirstOrDefaultAsync();

            if (gameConfig == null)
            {
                return NotFound();
            }
            if (gameConfig.HomeColor.Length < 9)
                gameConfig.HomeColor = AddAlpha(gameConfig.HomeColor);
            if (gameConfig.GuestColor.Length < 9)
                gameConfig.GuestColor = AddAlpha(gameConfig.GuestColor);
            return gameConfig;
        }

        // PUT: rest/db/game/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("game/{id}")]
        public async Task<IActionResult> PutGame(long id, Game game)
        {
            if (id != game.id || game.awayId == (long) 0 || game.homeId == (long) 0)
            {
                return BadRequest();
            }
            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameConfigExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(game);
        }

        // POST: rest/db/game
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("game")]
        public async Task<ActionResult<Game>> PostGame(Game game)
        {
            if (_context.Games == null) {
                return NotFound();
            }
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGameConfig), new { id = game.id }, game);
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

        private bool GameConfigExists(long id)
        {
            if (_context.GameSetup == null) {
                return false;
            }
            return _context.GameSetup.Any(e => e.Id == id);
        }
    }
}
