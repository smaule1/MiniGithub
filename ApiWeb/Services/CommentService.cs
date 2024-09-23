using ApiWeb.Data;
using ApiWeb.Models;
using Cassandra;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

        const string colId = "id", colUser = "user", colMessage = "message", colCreationDate = "creation_date",
            colLastDate = "last_date", colSubcomments = "subcomments", colRepoId = "repo_id";
        
        public CommentService (IOptions<CassandraSettings> options)
        {
            _cluster = Cluster.Builder()
                              .AddContactPoint(options.Value.ContactPoint)
                              .WithPort(options.Value.Port)
                              .Build();
            _session = _cluster.Connect(options.Value.Keyspace);

            _session.UserDefinedTypes.Define(
                UdtMap.For<Subcomment>("subcomment")
                    .Map(c => c.User, colUser)
                    .Map(c => c.Message, colMessage)
                    .Map(c => c.CreationDate, colCreationDate)
                    .Map(c => c.LastDate, colLastDate)
            );
        }

        private static List<Subcomment> GetSubcomments(Subcomment[] array)
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

        public List<Comment> GetComments()
        {
            List<Comment> list = [];
            var comments = _session.Execute("SELECT * FROM comments;");
            
            foreach (var row in comments)
            {
                Guid id = (Guid) row[colId];
                string user = (string) row[colUser];
                string message = (string) row[colMessage];
                DateTimeOffset creationDate = (DateTimeOffset)row[colCreationDate];
                DateTimeOffset lastDate = (DateTimeOffset) row[colLastDate];
                List<Subcomment> subcomments = GetSubcomments((Subcomment[]) row[colSubcomments]);

                if (!subcomments.IsNullOrEmpty()) { subcomments.Sort((s1, s2) => s1.CreationDate.CompareTo(s2.CreationDate)); }

                string repoId = (string)row[colRepoId];
                Comment comment = new(id, user, message, creationDate, lastDate, subcomments, repoId);

                list.Add(comment);
            }

            if (!list.IsNullOrEmpty()) { list.Sort((s1, s2) => s1.CreationDate.CompareTo(s2.CreationDate));  }

            return list;
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

        public void CreateComment(Comment comment)
        {
            Guid id = comment.Id;
            string user = comment.User;
            string message = comment.Message;
            string repoId = comment.RepoId;
            DateTimeOffset creationDate = comment.CreationDate;
            List<Subcomment> subcomments = comment.Subcomments;

            if (GetComment(id) != null)
            {
                throw new Exception("Primary Key Duplicated");
            }

            try
            {
                string query = $"INSERT INTO comments ({colId}, {colUser}, {colMessage}, {colCreationDate}, {colLastDate}, {colSubcomments}, {colRepoId}) VALUES (?, ?, ?, ?, ?, ?, ?);";
                var statement = _session.Prepare(query);
                var boundStatement = statement.Bind(id, user, message, creationDate, creationDate, subcomments, repoId);
                _session.Execute(boundStatement);
            }
            catch (Exception e)
            {
                throw new Exception("Error in CommentService.CreateComment\n" + e.Message);
            }
        }

        public void UpdateComment(Comment comment)
        {
            Guid id = comment.Id;
            string user = comment.User;
            string message = comment.Message;
            string repoId = comment.RepoId;
            DateTimeOffset lastDate = comment.LastDate;
            List<Subcomment> subcomments = comment.Subcomments;
            if (!subcomments.IsNullOrEmpty()) { subcomments.Sort((s1, s2) => s1.CreationDate.CompareTo(s2.CreationDate)); }

            if (GetComment(id) == null)
            {
                throw new Exception("Object does not exist");
            }

            try
            {
                string query = $"UPDATE comments SET {colUser} = ?, {colMessage} = ?, {colLastDate} = ?, {colSubcomments} = ?, {colRepoId} = ? WHERE {colId} = ?;";
                var statement = _session.Prepare(query);
                var boundStatement = statement.Bind(user, message, lastDate, subcomments, repoId, id);
                _session.Execute(boundStatement);
            }
            catch (Exception e)
            {
                throw new Exception("Error in CommentService.UpdateComment\n" + e.Message);
            }
        }

        public void DeleteComment(Guid id)
        {
            if (GetComment(id) == null)
            {
                throw new Exception("Object does not exist");
            }

            try
            {
                string query = $"DELETE FROM comments WHERE {colId} = ?;";
                var statement = _session.Prepare(query);
                var boundStatement = statement.Bind(id);
                _session.Execute(boundStatement);
            }
            catch (Exception e)
            {
                throw new Exception("Error in CommentService.DeleteComment\n" + e.Message);
            }
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
