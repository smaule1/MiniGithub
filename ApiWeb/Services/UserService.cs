using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Serialization;
using ApiWeb.Models;
using System.Text.Json;
using MongoDB.Bson.Serialization.IdGenerators;

namespace ApiWeb.Services
{
    public class UserService
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

        static UserService()
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect("localhost")
            );
        }

        public void CreateUser(User user)
        {
            var db = Connection.GetDatabase();

            if (db.KeyExists(user.Email))
            {
                throw new InvalidOperationException("An user with this email already exists.");
            }

            string userString = JsonSerializer.Serialize(user);

            db.StringSet(user.Email, userString);
        }

        public User? GetUser(string email)
        {
            var db = Connection.GetDatabase();

            string? userString = db.StringGet(email);

            if (userString == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<User>(userString);
        }

        public void DeleteUser(string email)
        {
            var db = Connection.GetDatabase();

            if (db.KeyExists(email))
            {
                db.KeyDelete(email);
            } else
            {
                throw new InvalidOperationException("Email does not belong to any user.");
            }
        }
    }
}
