using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Models
{

    public class Comment
    {
        public Guid Id { get; set; }
        [Required]
        public string User {  get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime Last_date { get; set; }
        [Required]
        public List<Subcomment> Subcomments { get; set; }

        public Comment(string user, string message, DateTime date, List<Subcomment> subcomments)
        {
            Id = Guid.NewGuid();
            User = user;
            Message = message;
            Last_date = date;
            Subcomments = subcomments;
        }
    }

}
