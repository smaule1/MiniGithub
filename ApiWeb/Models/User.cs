using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace ApiWeb.Models
{
    public class User
    {
        public string Email { get;  }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Id { get; }

        public static string HashPassword(string password, string email)
        {
            string source = email + password;
            byte[] tmpSource = ASCIIEncoding.ASCII.GetBytes(source);
            byte[] hash = MD5.HashData(tmpSource);

            int i;
            StringBuilder output = new StringBuilder(hash.Length);

            for (i=0; i<hash.Length; i++)
            {
                output.Append(hash[i].ToString("X2"));
            }

            return output.ToString();
        }

        public User(string email, string password, string name)
        {
            // On user create
            Guid uuid = Guid.NewGuid();
            Email = email;
            Password = HashPassword(password, email);
            Name = name;
            Id = uuid.ToString();
        }

        [JsonConstructor]
        public User(string email, string password, string name, string id)
        {
            // On user update
            Email = email;
            Password = password;
            Name = name;
            Id = id;
        }

        public User(string email, string password, string name, string id, bool changed)
        {
            // On update but password changed
            if (changed) // This is just so it stops complaining about me not using the 'changed' parameter
            {
                Email = email;
                Password = HashPassword(password, email);
                Name = name;
                Id = id;
            } else
            {
                Email = email;
                Password = HashPassword(password, email);
                Name = name;
                Id = id;
            }
        }
    }
}
