using Cassandra.Mapping.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Models
{
    public class Subcomment
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTimeOffset LastDate { get; set; }

        public Subcomment() { }
        public Subcomment(string user, string message, DateTimeOffset lastDate)
        {
            User = user;
            Message = message;
            LastDate = lastDate;
        }
    }
}
