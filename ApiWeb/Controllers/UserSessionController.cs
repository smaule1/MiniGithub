using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.IdGenerators;
using System.Runtime.CompilerServices;

namespace ApiWeb.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class UserSessionController : ControllerBase
    {
        private readonly UserSessionService sessionDb;

        public UserSessionController(UserSessionService sessionDb)
        {
            this.sessionDb = sessionDb;
        }

        [HttpPost]
        public IActionResult Create(string email, string password)
        {
            try
            {
                string sessionId = sessionDb.Login(email, password);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true
                };

                Response.Cookies.Append("SessionId", sessionId, cookieOptions);

                return Ok("Logged in successfully!");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string? sessionId = Request.Cookies["SessionId"];

                if (sessionId != null)
                {
                    if (sessionDb.VerifySession(sessionId))
                    {
                        return Ok();
                    }
                    else
                    {
                        return Unauthorized("Current session is not valid or is expired.");
                    }
                }
                else
                {
                    return Unauthorized("Current session is invalid.");
                }

            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete]
        public IActionResult Delete()
        {
            try
            {
                string? sessionId = Request.Cookies["SessionId"];

                if (sessionId != null)
                {
                    sessionDb.Logout(sessionId);
                    return Ok("Logged out successfully!");
                }
                else
                {
                    return Unauthorized("Current session is invalid.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
