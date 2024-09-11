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

        public IEnumerable<Repositorio> getRepositorios()
        {
            return repositorioCollection.Find(a => true).ToList();
        }

        //Methods used to commit
        //Agregado aquí por solo una conexión a MongoDB
        public IMongoCollection<Commit> commitCollection =>
            db.GetCollection<Commit>("commits");

        public IEnumerable<Commit> getCommits()
        {
            return commitCollection.Find(a => true).ToList();
        }

        public void CreateCommit(Commit commit)
        {
            try
            {
                commitCollection.InsertOne(commit);
            }
            catch (MongoWriteException) { throw; }
        }
    }
}



