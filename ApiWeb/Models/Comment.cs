using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiWeb.Models
{

    public class Comment
    {
        public Guid Id { get; set; }
        [Required]
        public string User {  get; set; }
        [Required]
        public string Message { get; set; }
        public DateTimeOffset LastDate { get; set; }
        public List<Subcomment>? Subcomments { get; set; }

        [JsonConstructor]
        public Comment(Guid id, string user, string message, DateTimeOffset lastDate, List<Subcomment> subcomments)
        {
            Id = id;
            User = user;
            Message = message;
            LastDate = lastDate;
            Subcomments = subcomments;
        }

        public Comment(string user, string message, DateTimeOffset lastDate, List<Subcomment> subcomments)
        {
            Id = Guid.NewGuid();
            User = user;
            Message = message;
            LastDate = lastDate;
            Subcomments = subcomments;
        }
    }

}
