using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using System.Drawing;
using Lib.AspNetCore.ServerSentEvents;
using System.Text.Json;

namespace WebAPI.Controllers
{
    public class Player2
    {
      public Player2() {
        this.id = 0;
        this.jersey = 0;
        this.name = string.Empty;
        this.position = string.Empty;
        this.image = string.Empty;
        this.year = 0;
        this.height = "0'0\"";
        this.weight = 0;
        this.school = string.Empty;
      }
        public Player2(Player player) {
            this.id = player.id;
            this.jersey = player.jersey;
            this.name = player.name;
            this.position = player.position;
            this.image = player.image;
            this.year = player.year;
            this.height = string.Format("{0}'{1}\"", player.height / 12, player.height % 12);
            this.weight = player.weight;
            this.school = player.school;
        }
        public Player getPlayer() {
            Player player = new Player();
            player.id = this.id;
            player.jersey = this.jersey;
            player.name = this.name;
            player.position = this.position;
            player.image = this.image;
            player.year = this.year;
            player.weight = this.weight;
            player.school = this.school;
            int nFt = this.height.IndexOf("'");
            int nIn = this.height.IndexOf("\"");
            player.height = (12 * int.Parse(this.height.Substring(0, nFt))) + int.Parse(this.height.Substring(nFt + 1, nIn - nFt - 1));
            return player;
        }
        public long id { get; set; }
        public long jersey { get; set; }
        public string name { get; set; } = string.Empty;
        public string position { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
        public int year { get; set; }
        public string height { get; set; } = string.Empty;
        public int weight { get; set; }
        public string school {get; set;} = string.Empty;
    }


    [Route("rest/db")]
    [ApiController]
    public class OverlayManagerController : ControllerBase
    {
        private readonly GameMgrContext _context;
        private readonly GameContext _gameContext;
        private readonly IServerSentEventsService _sseService;
        private string? _webRoot;

        public OverlayManagerController(GameMgrContext context, GameContext gameContext, IServerSentEventsService serverSentEventsService)
        {
            _context = context;
            _gameContext = gameContext;
            _sseService = serverSentEventsService;
#pragma warning disable 8600
            _webRoot = (string) AppDomain.CurrentDomain.GetData("WebRootPath");
#pragma warning restore 8600
        }

        // Players table management. Basic CRUD operations

        // POST: rest/db/player
        // Adds Player sent in request to Player table in database. If the Player sent in the request
        // includes an id, that id is ignored because the database auto-assigns id as the primary key.
        // Valid Player entries should include at least the jersey, name, and image fields.
        [HttpPost("player")]
        public async Task<ActionResult<Player2>> PostPlayer(Player2 player)
        {
          if (_context.Players == null) {
            return NotFound();
          }
          _context.Players.Add(player.getPlayer());
          await _context.SaveChangesAsync();
          return CreatedAtAction(nameof(Player2), new { id = player.id }, player);
        }

        // POST: rest/db/player/{imageName}
        // Uploads the player's image to the server. The imageName is the name of the image file.
        // Save the image to the server in the /overlay/Images/ directory.
        [HttpPost("player/{imageName}")]
        public async Task<ActionResult> PostPlayerImage(string imageName, IFormFile image)
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
          var path = Path.Combine(_webRoot, "overlay\\Images", imageName);
          using (var stream = new FileStream(path, FileMode.Create))
          {
            await image.CopyToAsync(stream);
          }
          return Ok();
        }

        // GET: rest/db/players
        // Returns a list of all Players currently in the Players table.
        [HttpGet("players")]
        public async Task<ActionResult<IEnumerable<PlayerSorted>>> GetPlayers()
        {
          if (_context.Teams == null) {
            return NotFound();
          }
          return await _context.Teams.ToListAsync();
        }

        // GET: rest/db/player/5
        // Returns the Player with the specified id. We don't use jersey number here because
        // that may not be unique across all players on a large team.
        [HttpGet("player/{id}")]
        public async Task<ActionResult<Player2>> GetPlayer(long id)
        {
          if (_context.Players == null) {
            return NotFound();
          }
          var player = await _context.Players.FindAsync(id);
          if (player == null) {
            return NotFound();
          }
          return new Player2(player);
        }

        // PUT: rest/db/player/5
        // Update the Player with the specified id.
        [HttpPut("player/{id}")]
        public async Task<IActionResult> PutPlayer(long id, Player2 player)
        {
            if (id != player.id) {
                return BadRequest();
            }
            Player _player = player.getPlayer();
            _context.Entry(_player).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (_context.Players == null || !_context.Players.Any(e => e.id == id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }
            // TODO: Should we return the updated player?
            return Ok(player);
        }

        // DELETE: rest/db/player/5
        // Delete the Player with the specified id.
        [HttpDelete("player/{id}")]
        public async Task<IActionResult> DeletePlayer(long id)
        {
            if (_context.Players == null) {
                return NotFound();
            }
            var player = await _context.Players.FindAsync(id);
            if (player == null) {
                return NotFound();
            }
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Hilight table management. Hilight is a runtime in-memory table that should only hold zero or 
        // one item at any time, representing a Player currently being higlighted on the on-screen
        // overlay. Consequently we only have operations to GET or PUT. The PUT operation acts to either
        // update the single entry to a Player matching the id sent, or if the id passed is zero then
        // the PUT operation clears the Hilight table, indicating that no player is currently hilighted.

        // GET: rest/db/hilight
        // Return the Player currently being hilighted (if any).
        [HttpGet("hilight")]
        public async Task<ActionResult<Player>> GetHilight()
        {
          if (_gameContext.Hilight == null) {
            return NotFound();
          }
          var hilight = await _gameContext.Hilight.FirstOrDefaultAsync();
          if (hilight == null) {
              return NotFound();
          }
          return hilight;
        }

        // PUT: rest/db/hilight
        // Update the single-entry Hilight table to contain the Player (from the Players table) matching
        // the id sent in the request. When the sent id=0 the single entry is deleted from the Hilight 
        // table, clearing the current hilight.
        [HttpPut("hilight")]
        public async Task<ActionResult<Player>> PutHilight(CurrentItem item)
        {
            if (_gameContext.Hilight != null && item.id == (long) 0) {
              // Clear the current entry.  
              var hilight = await _gameContext.Hilight.FindAsync((long) 1);
              if (hilight == null) {
                  return NoContent();
              }
              _gameContext.Hilight.Remove(hilight);
              await _gameContext.SaveChangesAsync();
              ServerSentEvent evt = new ServerSentEvent();
              evt.Type = "hilight";
              string jsonMsg = string.Format("{{\"id\":0,\"requester\":\"{0}\"}}", item.requester);
              evt.Data = new List<string>(new string[] { jsonMsg });
              await _sseService.SendEventAsync(evt);
              return NoContent();
            }
            // If there is a Player in the Players table matching the id in the request, 
            // copy that entry to the Hilight table as entry 1 and sent a notification to
            // all web pages currently subscribed to "hilight" events.
            if (_context.Players == null || _gameContext.Hilight == null) {
              return StatusCode(500);
            }
            var player = await _context.Players.FindAsync(item.id);
            if (player != null) {
              var count = await _gameContext.Hilight.LongCountAsync();
              player.id = 1;
              if (count < 1)
                _gameContext.Hilight.Add(player);
              else
                _gameContext.Hilight.Update(player);
              await _gameContext.SaveChangesAsync();
              ServerSentEvent evt = new ServerSentEvent();
              evt.Type = "hilight";
              string jsonMsg = string.Format("{{\"id\":{0},\"jersey\":{1}, \"name\":\"{2}\", \"image\":\"{3}\", \"requester\":\"{4}\"}}", 
                                              player.id, player.jersey, player.name, player.image, item.requester);
              evt.Data = new List<string>(new string[] {jsonMsg});
              await _sseService.SendEventAsync(evt);
              return Ok(player);
            } else
              return NotFound();
        }

        // Sponsors  table management. Basic CRUD operations

        // POST: rest/db/sponsor
        // Adds Sponsor sent in request to Sponsors table in database. If the Sponsor sent in the request
        // includes an id, that id is ignored because the database auto-assigns id as the primary key.
        // Valid Sponsor entries should include at least the name and image fields.
        [HttpPost("sponsor")]
        public async Task<ActionResult<Sponsor>> PostSponsor(Sponsor sponsor)
        {
            if (_context.Sponsors == null) {
              return NotFound();
            }
            sponsor.id = 0;
            _context.Sponsors.Add(sponsor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Sponsor), new { id = sponsor.id }, sponsor);
        }

        // POST: rest/db/sponsor/{imageName}
        // Uploads the sponsor logo image to the server. The imageName is the name of the image file.
        // Save the image to the server in the /overlay/Images/ directory.
        [HttpPost("sponsor/{imageName}")]
        public async Task<ActionResult> PostSponsorImage(string imageName, IFormFile image)
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
          var path = Path.Combine(_webRoot, "overlay\\Images", imageName);
          using (var stream = new FileStream(path, FileMode.Create))
          {
            await image.CopyToAsync(stream);
          }
          return Ok();
        }

        // GET: rest/db/sponsors
        // Returns a list of all Sponsor entries currently in the Sponsors table.
        [HttpGet("sponsors")]
        public async Task<ActionResult<IEnumerable<Sponsor>>> GetSponsors()
        {
          if (_context.Sponsors == null) {
            return NotFound();
          }
          return await _context.Sponsors.ToListAsync();
        }

        // GET: rest/db/sponsor/5
        // Returns the Sponsor with the specified id.
        [HttpGet("sponsor/{id}")]
        public async Task<ActionResult<Sponsor>> GetSponsor(long id)
        {
          if (_context.Sponsors == null) {
            return NotFound();
          }
          var sponsor = await _context.Sponsors.FindAsync(id);
          if (sponsor == null) {
            return NotFound();
          }
          return sponsor;
        }

        // PUT: rest/db/sponsor/5
        // Update the Sponsor with the specified id.
        [HttpPut("sponsor/{id}")]
        public async Task<IActionResult> PutSponsor(long id, Sponsor sponsor)
        {
            if (id != sponsor.id) {
                return BadRequest();
            }
            _context.Entry(sponsor).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (_context.Sponsors == null || !_context.Sponsors.Any(e => e.id == id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }
            return Ok(sponsor);;
        }

        // DELETE: rest/db/sponsor/5
        // Delete the Sponsor with the specified id.
        [HttpDelete("sponsor/{id}")]
        public async Task<IActionResult> DeleteSponsor(long id)
        {
            if (_context.Sponsors == null) {
              return NotFound();
            }
            var sponsor = await _context.Sponsors.FindAsync(id);
            if (sponsor == null) {
                return NotFound();
            }
            _context.Sponsors.Remove(sponsor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Ad table management. Ad is a runtime in-memory table that should only hold zero or 
        // one item at any time, representing a Sponsor currently being displayed on the on-screen
        // overlay. Consequently we only have operations to GET or PUT. The PUT operation acts to either
        // update the single entry to a Sponsor matching the id sent, or if the id passed is zero then
        // the PUT operation clears the Ad table, indicating that no sponsor is currently displayed.

        // GET: rest/db/ad
        [HttpGet("ad")]
        public async Task<ActionResult<Sponsor>> GetAd()
        {
          if (_gameContext.Ad == null) {
            return NotFound();
          }
          var ad = await _gameContext.Ad.FirstOrDefaultAsync();
          if (ad == null) {
              return NotFound();
          }
          return ad;
        }

        // PUT: rest/db/ad
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ad")]
        public async Task<ActionResult<Sponsor>> PostAd(CurrentItem item)
        {
            if (item.id == (long) 0 && _gameContext.Ad != null) {
              // Clear the current entry, then notify all subscribed pages.
              var ad = await _gameContext.Ad.FindAsync((long) 1);
              if (ad == null)
              {
                  return NoContent();
              }
              _gameContext.Ad.Remove(ad);
              await _gameContext.SaveChangesAsync();
              ServerSentEvent evt = new ServerSentEvent();
              evt.Type = "ad";
              string jsonMsg = string.Format("{{\"id\":0,\"requester\":\"{0}\"}}", item.requester);
              evt.Data = new List<string>(new string[] { jsonMsg });
              // evt.Data = new List<string>(new string[] {"{}"});
              await _sseService.SendEventAsync(evt);
              return NoContent();
            }
            // If there is a Sponsor in the Sponsors table matching the id in the request, 
            // copy that entry to the Ad table as entry 1 and sent a notification to
            // all web pages currently subscribed to "ad" events.
            if (_context.Sponsors == null || _gameContext.Ad == null) {
              return StatusCode(500);
            }
            var sponsor = await _context.Sponsors.FindAsync(item.id);
            if (sponsor != null) {
              var count = await _gameContext.Ad.LongCountAsync();
              sponsor.id = 1;
              if (count < 1)
                _gameContext.Ad.Add(sponsor);
              else
                _gameContext.Ad.Update(sponsor);
              await _gameContext.SaveChangesAsync();
              ServerSentEvent evt = new ServerSentEvent();
              evt.Type = "ad";
              string jsonMsg = string.Format("{{\"id\":{0},\"name\":\"{1}\",\"image\":\"{2}\", \"requester\":\"{3}\"}}", 
                                              sponsor.id, sponsor.name, sponsor.image, item.requester);
              evt.Data = new List<string>(new string[] {jsonMsg});
              await _sseService.SendEventAsync(evt);
              return Ok(sponsor);
            } else
              return NotFound();
        }

        // PUT: rest/db/scoreboard
        // Show or hide the scoreboard overlay
        [HttpPut("scoreboard")]
        public async Task<ActionResult> PutScoreboard(CurrentItem item)
        {
          // Clear the current entry.  
          ServerSentEvent evt = new ServerSentEvent();
          /*
          if (item.id == (long) 0) {
            evt.Type = "hideScoreboard";
          } else {
            evt.Type = "showScoreboard";
          }
          */
          evt.Type = "scoreboard";
          // string jsonMsg = string.Format("{{\"id\":{0},\"requester\":\"{1}\"}}", item.id, item.requester);
          evt.Data = new List<string>(new string[] {JsonSerializer.Serialize(item)} );
          // evt.Data = new List<string>(new string[] {"{}"});
          await _sseService.SendEventAsync(evt);
          return NoContent();
        }
    }
}
