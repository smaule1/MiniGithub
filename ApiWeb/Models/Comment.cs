using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiWeb.Models
{

    public class Comment
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset LastDate { get; set; }
        [Required]
        public List<Subcomment> Subcomments { get; set; }
        [Required]
        public string RepoId { get; set; }

        [JsonConstructor]
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

        public Comment(string user, string message, DateTimeOffset creationDate, List<Subcomment> subcomments, string repoId)
        {
            Id = Guid.NewGuid();
            User = user;
            Message = message;
            RepoId = repoId;
            CreationDate = creationDate;
            LastDate = creationDate;
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
