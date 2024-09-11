namespace ApiWeb.Data
{
    public class MongoDBSettings
    {
        //class that retrieves the database properties stored in appsettings.json

        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public IDictionary<string, string> Collections { get; set; } = null!;
    }
}
