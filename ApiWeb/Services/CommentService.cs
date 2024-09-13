using ApiWeb.Data;
using Cassandra;
using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Services
{
    public class CommentService
    {
        [Required]
        private readonly Cluster _cluster;
        [Required]
        private readonly Cassandra.ISession _session;
        
        public CommentService ()
        {
            var settings = new CassandraSettings();

            _cluster = Cluster.Builder()
                              .AddContactPoint("")
                              .Build();
            _session = _cluster.Connect("");
        }
    }
}
