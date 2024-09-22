namespace ApiWeb.Data
{
    public class CassandraSettings
    {
        public string ContactPoint { get; set; } = null!;
        public int Port {  get; set; } = 0;
        public string Keyspace { get; set; } = null!;
    }
}
