using Microsoft.Extensions.Options;
using ApiWeb.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using ApiWeb.Data;
using static MongoDB.Driver.WriteConcern;
using NuGet.Protocol.Core.Types;

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
                var filter = Builders<Repositorio>.Filter.Eq(x => x.Id, id);
                var update = Builders<Repositorio>.Update.Set(x => x.Nombre, repositorio.Nombre).
                                                          Set(x => x.Tags, repositorio.Tags);
                
                return repositorioCollection.UpdateOne(filter, update);
            }
            catch (MongoWriteException) { throw; }
        }        

        public DeleteResult Delete(string id)
        {
            var filter = Builders<Repositorio>.Filter.Eq(x => x.Id, id);
            
            return repositorioCollection.DeleteOne(filter);                        
        }

        public Repositorio GetPublicRepositorioById(string id)
        {
            var builder = Builders<Repositorio>.Filter;
            var filter = builder.Eq(x => x.Visibilidad, "public") &  
                         builder.Eq(x => x.Id, id);            
            return repositorioCollection.Find(filter).FirstOrDefault();            
        }

        public List<Repositorio> GetPublicRepositorioByName(string name)
        {
            var builder = Builders<Repositorio>.Filter;
            var filter = builder.Regex(x => x.Nombre, $"/.*{name}.*/") &   //TODO: use Mongo's search index
                         builder.Eq(x => x.Visibilidad, "public");
            var simpleRepo = Builders<Repositorio>.Projection.
                 Expression(f => new Repositorio { Id=f.Id, UsuarioId = f.UsuarioId, Nombre = f.Nombre, Visibilidad=f.Visibilidad, Tags=f.Tags});
            return repositorioCollection.Find(filter).Project(simpleRepo).ToList();
        }

        public List<Repositorio> GetAllRepositorios(string user_id, string visibilidad)
        {
            var builder = Builders<Repositorio>.Filter;
            var filter = builder.Eq(x => x.Visibilidad, visibilidad) &
                         builder.Eq(x => x.UsuarioId, user_id);
            var simpleRepo = Builders<Repositorio>.Projection.
                Expression(f => new Repositorio {Id = f.Id, UsuarioId = f.UsuarioId, Nombre = f.Nombre, Visibilidad = f.Visibilidad, Tags = f.Tags });
            return repositorioCollection.Find(filter).Project(simpleRepo).ToList();
        }        

        public Repositorio GetPrivateRepositorioById(string id)
        {
            var builder = Builders<Repositorio>.Filter;
            var filter = builder.Eq(x => x.Visibilidad, "private") &
                         builder.Eq(x => x.Id, id);            
            return repositorioCollection.Find(filter).FirstOrDefault();
        }
    }
}



