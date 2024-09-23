using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.IdGenerators;
using System.Runtime.CompilerServices;
using System.Text.Json;

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
        public IActionResult Create([FromBody] JsonElement data)
        {
            var email = data.GetProperty("email").GetString();
            var password = data.GetProperty("password").GetString();

            if (email.IsNullOrEmpty())
            {
                return BadRequest(new { error = "Invalid Input", message = "El email está vacío." , type = "Email"});
            }
            if (password.IsNullOrEmpty())
            {
                return BadRequest(new { error = "Invalid Input", message = "La contraseña está vacía.", type = "Password" });
            }

            try
            {
                string sessionId = sessionDb.Login(email, password);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true
                };

                Response.Cookies.Append("SessionId", sessionId, cookieOptions);

                var sessionData = UserSessionService.GetSession(sessionId);

                return Ok(sessionData);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = "Not Auth", message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Unauthorized(new { error = "Not Found", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string sessionId)
        {
            try
            {
                if (sessionId != null)
                {
                    if (sessionDb.VerifySession(sessionId))
                    {
                        return Ok();
                    }
                    else
                    {
                        return Unauthorized(new { error = "Not Auth", message = "La sesión actual no es válida o está expirada.", type = "Not valid" });
                    }
                }
                else
                {
                    return Unauthorized(new { error = "Not Auth", message = "La sesión actual no es válida.", type = "No session"});
                }

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = "Not Found", message = ex.Message });
            }

        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] string sessionId)
        {
            try
            {
                if (sessionId != null)
                {
                    sessionDb.Logout(sessionId);
                    return Ok();
                }
                else
                {
                    return Unauthorized(new { error = "Not Auth", message = "La sesión actual no es válida." });
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = "Not Found", message = ex.Message });
            }
        }
    }
}
