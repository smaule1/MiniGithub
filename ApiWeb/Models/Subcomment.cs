using Cassandra.Mapping.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Models
{
    public class Subcomment
    {
        [Required]
        public string User { get; set; } = string.Empty;
        [Required]
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset LastDate { get; set; }

        public Subcomment() { }
        public Subcomment(string user, string message, DateTimeOffset creationDate)
        {
            User = user;
            Message = message;
            CreationDate = creationDate;
        }
        public Subcomment(string user, string message, DateTimeOffset creationDate, DateTimeOffset lastDate)
        {
            User = user;
            Message = message;
            CreationDate = creationDate;
            LastDate = lastDate;
        }
    }
}
