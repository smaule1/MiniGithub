using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Serialization;


namespace ApiWeb.Models
{
    public class User
    {
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Id { get; set; }

        public User(string email, string password, string name, string id)
        {
            Email = email;
            Password = password;
            Name = name;
            Id = id;
        }
    }
}
