using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Models
{
    public class UserSession
    {
        public string SessionId { get; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UserId { get; set; }

        public UserSession(string email, string name, string userId)
        {
            SessionId = Guid.NewGuid().ToString();
            Email = email;
            Name = name;
            UserId = userId;
        }
    }
}
