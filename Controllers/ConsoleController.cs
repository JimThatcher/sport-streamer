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
        // Provides /rest/console APIs to control serial connection to Daktronics console
        // and read version and status information from the console.
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
