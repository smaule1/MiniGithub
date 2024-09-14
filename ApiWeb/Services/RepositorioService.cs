using Microsoft.Extensions.Options;
using ApiWeb.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using ApiWeb.Data;
using static MongoDB.Driver.WriteConcern;

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
        }


        public IMongoCollection<Repositorio> repositorioCollection =>
            db.GetCollection<Repositorio>("repositorios");

        public void Create(Repositorio repositorio)
        {            
            try
            {
                repositorioCollection.InsertOne(repositorio);
            }
            catch (MongoWriteException) { throw; }                      
        }

        public UpdateResult Update(Repositorio repositorio, string id)
        {
            try
            {
                var filter = Builders<Repositorio>.Filter.Eq(repositorio => repositorio.Id, id);
                var update = Builders<Repositorio>.Update.Set(repositorio => repositorio.Nombre, repositorio.Nombre).
                                                          Set(repositorio => repositorio.Tags, repositorio.Tags).
                                                          Set(repositorio => repositorio.Visibilidad, repositorio.Visibilidad);
                
                return repositorioCollection.UpdateOne(filter, update);
            }
            catch (MongoWriteException) { throw; }
        }



        public IEnumerable<Repositorio> GetAllRepositorios()
        {
            return repositorioCollection.Find(a => true).ToList();
        }
    }
}



