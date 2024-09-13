using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Models
{
    public class Subcomment
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime Last_date { get; set; }

        public Subcomment(string user, string message, DateTime date)
        {
            User = user;
            Message = message;
            Last_date = date;
        }
    }
}
