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

namespace WebAPI.Controllers
{
    public class ConsoleStatus
    {
        public bool IsConnected { get; set; }
    }

    [Route("rest/[controller]")]
    [ApiController]
    public class ConsoleController : ControllerBase
    {
        private readonly GameContext _context;
        // private readonly IServiceScopeFactory _scopeFactory;
        ConsoleData _console;


        // public ClockDataController(GameContext context, IServiceScopeFactory scopeFactory)
        public ConsoleController(GameContext context, ConsoleData console)
        {
            _context = context;
            _console = console;
        }

        // GET: rest/consoleversion
        [HttpGet("version")]
        public ActionResult<ConsoleInfo> GetConsoleVersion()
        {
            if (_context.ConsoleVersion != null && _context.ConsoleVersion.Count() > 0) {
                ConsoleInfo _info = _context.ConsoleVersion.First();
                return _info;
            } else {
                return NotFound();
            }
        }

        // GET: api/Console/connect
        // Calls the DakAccess.StartConsoleConnectTimer method to start the console connection timer.
        [HttpGet("connect")]
        public IActionResult Connect()
        {
            _console.StartConsoleConnectTimer();
            return Ok();
        }

        // GET: api/Console/disconnect
        // Calls the DakAccess.StopConsoleConnectTimer method to stop the console connection timer.
        [HttpGet("disconnect")]
        public IActionResult Disconnect()
        {
            _console.StopConsoleConnectTimer();
            return Ok();
        }

        // GET: api/Console/Status
        // Gets the status of the console connection.
        [HttpGet("status")]
        public IActionResult Status()
        {
            ConsoleStatus status = new ConsoleStatus();
            status.IsConnected = _console.IsConsoleConnected;
            return Ok(JsonSerializer.Serialize(status));
        }
    }
}
