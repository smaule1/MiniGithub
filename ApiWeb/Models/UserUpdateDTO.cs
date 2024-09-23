using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Models
{
    public class UserUpdateDTO
    {
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
    }
}
