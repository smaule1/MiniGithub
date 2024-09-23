using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiWeb.Models
{

    public class Comment
    {
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string User { get; set; } = string.Empty;
        [Required]
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset LastDate { get; set; } = DateTimeOffset.Now;
        [Required]
        public List<Subcomment> Subcomments { get; set; } = [];
        [Required]
        public string RepoId { get; set; } = string.Empty;

        public Comment() { }

        public Comment(Guid id, string user, string message, DateTimeOffset creationDate, DateTimeOffset lastDate, List<Subcomment> subcomments, string repoId)
        {
            Id = id;
            User = user;
            Message = message;
            RepoId = repoId;
            CreationDate = creationDate;
            LastDate = lastDate;
            Subcomments = GetSubcomments(subcomments);
        }

        private static List<Subcomment> GetSubcomments(List<Subcomment> subcomments)
        {
            if (subcomments != null)
            {
                return subcomments;
            }
            return [];
        }
    }

}
