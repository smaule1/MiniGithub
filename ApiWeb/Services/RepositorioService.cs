using Microsoft.Extensions.Options;
using ApiWeb.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using ApiWeb.Data;

namespace ApiWeb.Services
{
    public class RepositorioService
    {
        //Does database operations related with Repositorio model

        public readonly IMongoDatabase db;

        public RepositorioService(IOptions<MongoDBSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            db = client.GetDatabase(options.Value.DatabaseName);

            //Test connectiom
            try
            {
                var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                System.Diagnostics.Debug.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
        }


        public IMongoCollection<Repositorio> repositorioCollection =>
            db.GetCollection<Repositorio>("repositorios");

        public void Create(Repositorio repositorio)
        {
            repositorioCollection.InsertOne(repositorio);
            //TODO: catch duplicate key exception
        }

        public IEnumerable<Repositorio> getRepositorios()
        {
            return repositorioCollection.Find(a => true).ToList();
        }
    }
}



