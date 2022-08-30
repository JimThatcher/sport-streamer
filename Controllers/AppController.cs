using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using System.Drawing;
using AuthenticationService.Models;
using AuthenticationService.Managers;
using DakAccess;

namespace WebAPI.Controllers
{
    [Route("rest")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly GameMgrContext _context;
        private readonly ConsoleData _console;

        public AppController(GameMgrContext context, ConsoleData console)
        {
            _context = context;
            _console = console;
        }

        //GET: rest/version
        [HttpGet("version")]
        public ActionResult<ServiceInfo> GetVersion()
        {
            // TODO: Read version info from assembly properties
            ServiceInfo _version = new ServiceInfo();
            _version.IsConsoleConnected = _console.IsConsoleConnected;
            return _version;
        }

        // GET: rest/features
        [HttpGet("features")]
        public ActionResult<Features> GetFeatures()
        {
          Features _features = new Features();
          _features.device = false;
          _features.project = true;
          _features.security = true;
          _features.mqtt = false;
          _features.ntp = false;
          _features.ota = false;
          _features.upload_firmware = false;
            return  _features;
        }

        // POST: rest/signIn takes json body {"username":"string", "password":"string"} and returns a JWT 
        [HttpPost("signIn")]
        public async Task<ActionResult<AuthResult>> PostSignIn(SignInRequest request)
        {
            // Validate that the username/password pair represent an authorized user
            // then generate a JWT representing the authorization
            if (_context.Users == null || _context.jwt_seed == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "No users found");
            }
            var user = _context.Users.FirstOrDefault(u => u.username == request.username);
            if (user == null || user.password != request.password)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Invalid username/password");
            }
            var jwt_seed = await _context.jwt_seed.FindAsync((long)1);
            if (jwt_seed == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "No Security Kay has been set");
            }
            IAuthContainerModel model = new JWTContainerModel() { 
                Claims = new System.Security.Claims.Claim[] 
                {
                    new System.Security.Claims.Claim("username", user.username)
                }, 
                SecretKey = jwt_seed.seed
            };
            if (user.admin)
            {
                model.Claims = model.Claims.Concat(new System.Security.Claims.Claim[] 
                {
                    new System.Security.Claims.Claim("admin", "true")
                }).ToArray();
            }
            int highWord = (int)(model.SecretKey >> 32);
            int lowWord = (int)(model.SecretKey & 0xFFFFFFFF);
            string signingKey = string.Format("{0:x8}-{1:x8}", highWord, lowWord);
            IAuthService authService = new JWTService(signingKey);

            AuthResult result = new AuthResult();
            result.access_token = authService.GenerateToken(model);
            return Ok(result);
        }

        // GET: rest/verifyAuthorization
        [HttpGet("verifyAuthorization")]
        public ActionResult GetAuth()
        {
            // Check the authorization header. If it is Bearer auth with a valid JWT token issued 
            // by this server, return 200, otherwise return 401.
            var authHeader = Request.Headers["Authorization"];
            if (authHeader.Count == 0)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "No authorization header");
            }
            var authHeaderValue = authHeader.FirstOrDefault();
            if (authHeaderValue != null && authHeaderValue.StartsWith("Bearer ") && _context.jwt_seed != null)
            {
                var token = authHeaderValue.Substring("Bearer ".Length).Trim();
                if (string.IsNullOrEmpty(token))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "No token");
                }
                var jwt_seed = _context.jwt_seed.Find((long)1);
                if (jwt_seed == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "No Security Kay has been set");
                }
                int highWord = (int)(jwt_seed.seed >> 32);
                int lowWord = (int)(jwt_seed.seed & 0xFFFFFFFF);
                string signingKey = string.Format("{0:x8}-{1:x8}", highWord, lowWord);
                IAuthService authService = new JWTService(signingKey);
                if (authService.IsTokenValid(token))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Invalid token");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Invalid authorization header");
            }
        }

        // GET: rest/securitySettings
        [HttpGet("securitySettings")]
        public async Task<ActionResult<getSecuritySettings>> GetSecuritySettings()
        {
            // Return the security settings for the server.
            // Get the jwt_seed from the JWT_SEED table.
            // Get the users from the Users table.
            // Return the security settings.
            if (_context.jwt_seed == null || _context.Users == null)
            {
                return NotFound();
            }
            var jwt_seed = await _context.jwt_seed.FindAsync((long)1);
            getSecuritySettings settings = new getSecuritySettings(jwt_seed != null ? jwt_seed.seed : 0x0BADF00DCAFE1234);
            settings.users = await _context.Users.ToArrayAsync();
            return settings;
        }

        // POST: rest/securitySettings
        [HttpPost("securitySettings")]
        public async Task<ActionResult<setSecuritySettings>> PostSecuritySettings(setSecuritySettings settings)
        {
            // Update the security settings for the server.
            // Update the jwt_seed in the JWT_SEED table.
            // Update the users in the Users table.
            // Return the security settings.
            // If settings is null or does not contain a jwt_seed, return a bad request.
            if (settings == null || settings.jwt_secret == null || settings.jwt_secret.Length < 10)
            {
                return BadRequest();
            }
            if (_context.jwt_seed == null || _context.Users == null)
            {
                return StatusCode(500);
            }
            // Convert string-based jwt_secret to 64-bit integer
            string hexSecret = settings.jwt_secret.Replace("-", "");
            UInt64 secret = UInt64.Parse(hexSecret, System.Globalization.NumberStyles.HexNumber);
            var jwt_seed = await _context.jwt_seed.FindAsync((long)1);
            if (jwt_seed == null)
            {
                jwt_seed = new jwt_seed() { id = 1, seed = 0 };
                _context.jwt_seed.Add(jwt_seed);
            }
            if (jwt_seed.seed != secret)
            {
                jwt_seed.seed = secret;
                _context.Entry(jwt_seed).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            _context.Users.RemoveRange(_context.Users.ToArray());
            await _context.Users.AddRangeAsync(settings.users);
            await _context.SaveChangesAsync();
            return settings;
        }
    }
}
