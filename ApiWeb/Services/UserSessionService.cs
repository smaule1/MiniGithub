using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using ApiWeb.Models;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ApiWeb.Services
{
    public class UserSessionService
    {
        [Required]
        private static readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return _lazyConnection.Value;
            }
        }

        static UserSessionService()
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect("localhost")
            );
        }

        public string Login(string email, string password)
        {
            var db = Connection.GetDatabase();

            if(db.KeyExists("user:" + email))
            {
                var userString = db.StringGet("user:" + email);
                User? user = JsonSerializer.Deserialize<User>(userString);

                if(user != null)
                {
                    if(user.Password == User.HashPassword(password, email))
                    {
                        UserSession session = new UserSession(user.Email, user.Name, user.Id);
                        string sessionString = JsonSerializer.Serialize(session);

                        db.StringSet("session:" + session.SessionId, sessionString);
                        db.KeyExpire("session:" + session.SessionId, TimeSpan.FromHours(1));

                        return session.SessionId;
                    } else
                    {
                        throw new UnauthorizedAccessException("Password provided does not belong to requested email.");
                    }
                } else
                {
                    throw new KeyNotFoundException("Error retrieving requested user.");
                }
            } else
            {
                throw new KeyNotFoundException("Requested user does not exist.");
            }
        }

        public static UserSession? GetSession(string sessionId)
        {
            var db = Connection.GetDatabase();

            string? sessionString = db.StringGet("session:" + sessionId);

            if (sessionString == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<UserSession>(sessionString);
        }

        public bool VerifySession(string sessionId)
        {
            var db = Connection.GetDatabase();

            if(db.KeyExists("session:" + sessionId))
            {
                db.KeyExpire("session:" + sessionId, TimeSpan.FromHours(1));

                return true;
            } else
            {
                return false;
            }
        }

        public void Logout(string sessionId)
        {
            var db = Connection.GetDatabase();

            if(db.KeyExists("session:" + sessionId))
            {
                db.KeyDelete("session:" + sessionId);
            } else
            {
                throw new KeyNotFoundException("Could not find requested session.");
            }
        }
    }
}
