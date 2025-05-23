﻿using StackExchange.Redis;
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
            var options = ConfigurationOptions.Parse("54.197.24.238:6379"); 
            options.Password = "d3jenDeHacke4rmed$&TEsMKncxLaCKklLdAazV6hdR5&CR4YEB52xJw#vLTqJr&U72^wVJHxqpw2e";
            var HOST_NAME = "54.197.24.238";
            var PORT_NUMBER = "6379";
            var PASSWORD = "d3jenDeHacke4rmed$&TEsMKncxLaCKklLdAazV6hdR5&CR4YEB52xJw#vLTqJr&U72^wVJHxqpw2e";
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect($"{HOST_NAME}:{PORT_NUMBER},password={PASSWORD}")
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
                        throw new UnauthorizedAccessException("Contraseña equivocada.");
                    }
                } else
                {
                    throw new KeyNotFoundException("Error al obtener el usuario solicitado.");
                }
            } else
            {
                throw new KeyNotFoundException("El usuario solicitado no existe.");
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
                throw new KeyNotFoundException("No se pudo encontrar la sesión solicitada.");
            }
        }
    }
}
