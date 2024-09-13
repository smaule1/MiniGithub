using Cassandra;
using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Services
{
    public class CommentsService
    {
        [Required]
        private readonly Cluster _cluster;
        [Required]
        private readonly Cassandra.ISession _session;
        private static CommentsService? Instance {  get; set; }

        public CommentsService getInstance()
        {
            if (Instance == null)
            {
                Instance = new CommentsService();
            }
            return Instance;
        }
        private CommentsService ()
        {
            _cluster = Cluster.Builder()
                              .AddContactPoint("")
                              .Build();
            _session = _cluster.Connect("");
        }
    }
}
