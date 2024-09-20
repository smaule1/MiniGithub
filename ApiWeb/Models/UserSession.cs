using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Models
{
    public class UserSession
    {
        public string SessionId { get; }
        [Required]
        public string Name {  get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string UserId { get; set; }

        public UserSession(string name, string email, string password, string userId)
        {
            SessionId = Guid.NewGuid().ToString();
            Name = name;
            Email = email;
            Password = password;
            UserId = userId;
        }
    }
}
