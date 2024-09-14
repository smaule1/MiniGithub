using ApiWeb.Data;
using ApiWeb.Models;
using Cassandra;
using DTOs;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ApiWeb.Services
{
    public class CommentService
    {
        [Required]
        private readonly Cluster _cluster;
        [Required]
        private readonly Cassandra.ISession _session;
        
        public CommentService (IOptions<CassandraSettings> options)
        {
            _cluster = Cluster.Builder()
                              .AddContactPoint(options.Value.ContactPoint)
                              .Build();
            _session = _cluster.Connect(options.Value.Keyspace);

            _session.UserDefinedTypes.Define(
                UdtMap.For<Subcomment>("subcomment")
                    .Map(c => c.User, "user")
                    .Map(c => c.Message, "message")
                    .Map(c => c.LastDate, "last_date")
            );
        }

        public void CreateComment(Comment comment)
        {
            Guid id = comment.Id;
            string user = comment.User;
            string message = comment.Message;
            DateTimeOffset date = comment.LastDate;
            List<Subcomment> subcomments = comment.Subcomments;

            if (GetComment(id) != null)
            {
                throw new Exception("Primary Key Duplicated");
            }

            string query = "INSERT INTO comments (id, user, message, last_date, subcomments) VALUES (?, ?, ?, ?, ?);";
            var statement = _session.Prepare(query);
            var boundStatement = statement.Bind(id, user, message, date, subcomments);
            _session.Execute(boundStatement);
        }

        public List<Comment> GetComments()
        {
            List<Comment> list = new List<Comment>();
            var comments = _session.Execute("SELECT * FROM comments;");
            
            foreach (var row in comments)
            {
                Guid id = (Guid) row["id"];
                string user = (string) row["user"];
                string message = (string) row["message"];
                DateTimeOffset date = (DateTimeOffset) row["last_date"];
                List<Subcomment> subcomments = GetSubcomments((Subcomment[]) row["subcomments"]);
                Comment comment = new Comment(id, user, message, date, subcomments);
                list.Add(comment);
            }

            return list;
        }

        private List<Subcomment> GetSubcomments(Subcomment[] array)
        {
            if (array == null || array.Length == 0)
            {
                return null!;
            }
            else
            {
                return [.. array];
            }
        }

        public Comment GetComment(Guid id)
        {
            List<Comment> list = GetComments();
            foreach (Comment comment in list)
            {
                if (comment.Id == id)
                {
                    return comment;
                }
            }
            return null!;
        }

        public void UpdateComment(Comment comment)
        {
            Guid id = comment.Id;
            string user = comment.User;
            string message = comment.Message;
            DateTimeOffset date = comment.LastDate;
            List<Subcomment> subcomments = comment.Subcomments;

            if (GetComment(id) == null)
            {
                throw new Exception("Object does not exist");
            }

            string query = "UPDATE comments SET user = ?, message = ?, last_date = ?, subcomments = ? WHERE id = ?;";
            var statement = _session.Prepare(query);
            var boundStatement = statement.Bind(user, message, date, subcomments, id);
            _session.Execute(boundStatement);
        }

        public void DeleteComment(Guid id)
        {
            if (GetComment(id) == null)
            {
                throw new Exception("Object does not exist");
            }

            string query = "DELETE FROM comments WHERE id = ?;";
            var statement = _session.Prepare(query);
            var boundStatement = statement.Bind(id);
            _session.Execute(boundStatement);
        }

        public string GetExceptionMessage(Exception exception)
        {
            Type type = exception.GetType();

            if (type == typeof(NoHostAvailableException))
            {
                return "Problems on connecting to nodes";
            }
            else if (type == typeof(QueryExecutionException))
            {
                return "Error with the query execution";
            }
            else if (type == typeof(InvalidQueryException))
            {
                return "Invalid query";
            }
            else if (type == typeof(ReadTimeoutException))
            {
                return "Waiting time has been exceeded when reading";
            }
            else if (type == typeof(WriteTimeoutException))
            {
                return "Waiting time has been exceeded when writing";
            }
            else if (type == typeof(TimeoutException))
            {
                return "The operation has exceeded the waiting time";
            }
            else if (type == typeof(UnavailableException))
            {
                return "Cluster has not enough replicas";
            }
            else
            {
                return "An error has ocurred: " + exception.Message;
            }
        }
    }
}
