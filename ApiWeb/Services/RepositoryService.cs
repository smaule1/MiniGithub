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
using System.Data;
using MongoDB.Driver.Linq;
using Repository = ApiWeb.Models.Repository;

namespace ApiWeb.Services
{
    public class RepositoryService
    {
        //Does database operations related with Repositorio model

        public readonly IMongoDatabase db;
        private readonly IDictionary<string, string> collectionNames;

        public RepositoryService(IOptions<MongoDBSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            db = client.GetDatabase(options.Value.DatabaseName);
            collectionNames = options.Value.Collections;
        }


        public IMongoCollection<Repository> repositoryCollection =>
            db.GetCollection<Repository>(collectionNames["Collection1"]);

        public void Create(Repository repository)
        {            
            try
            {
                repositoryCollection.InsertOne(repository);
            }
            catch (MongoWriteException) { throw; }                      
        }

        public UpdateResult Update(Repository repository, string id)
        {
            try
            {
                var filter = Builders<Repository>.Filter.Eq<string>(x => x.Id, id);
                var update = Builders<Repository>.Update.Set(x => x.Name, repository.Name).
                                                          Set(x => x.Tags, repository.Tags);
                
                return repositoryCollection.UpdateOne(filter, update);
            }
            catch (MongoWriteException) { throw; }
        }        

        public DeleteResult Delete(string id)
        {
            var filter = Builders<Repository>.Filter.Eq<string>(x => x.Id, id);
            
            return repositoryCollection.DeleteOne(filter);                        
        }

        public Repository GetRepositorioById(string id, string visibilidad)
        {
            var builder = Builders<Repository>.Filter;
            var filter = builder.Eq(x => x.Visibility, visibilidad) &  
                         builder.Eq(x => x.Id, id);            
            return repositoryCollection.Find(filter).FirstOrDefault();            
        }

        public List<Repository> GetPublicRepositorioByName(string name)
        {
            var builder = Builders<Repository>.Filter;
            var filter = builder.Regex(x => x.Name, $"/.*{name}.*/") &   //TODO: use Mongo's search index
                         builder.Eq(x => x.Visibility, "public");
            var simpleRepo = Builders<Repository>.Projection.
                 Expression(f => new Repository { Id= f.Id, UserId = f.UserId, Name = f.Name, Visibility= f.Visibility, Tags= f.Tags});
            return repositoryCollection.Find(filter).Project(simpleRepo).ToList();
        }

        public List<Repository> GetAllRepositorios(string user_id, string visibilidad)
        {
            var builder = Builders<Repository>.Filter;
            var filter = builder.Eq(x => x.Visibility, visibilidad) &
                         builder.Eq(x => x.UserId, user_id);
            var simpleRepo = Builders<Repository>.Projection.
                Expression(f => new Repository { Id = f.Id, UserId = f.UserId, Name = f.Name, Visibility = f.Visibility, Tags = f.Tags });
            return repositoryCollection.Find(filter).Project(simpleRepo).ToList();
        }                

        public UpdateResult CreateBranch(string id, Branch branch)
        {
            var repositorio = GetRepositorioById(id) ?? throw new BadHttpRequestException("Repositorio not found");
            if (!repositorio.IsBranchNameAvailable(branch.Name)) throw new DuplicateNameException($"There is already a branch named '{branch.Name}' in this repository");
            
            var filter = Builders<Repository>.Filter.Eq<string>(x => x.Id, id);
            var update = Builders<Repository>.Update.Push(x => x.Branches, branch);
            
            return repositoryCollection.UpdateOne(filter, update);

        }

        private Repository GetRepositorioById(string id)
        {
            var filter = Builders<Repository>.Filter.Eq<string>(x => x.Id, id);
            return repositoryCollection.Find(filter).FirstOrDefault();
        }

        public UpdateResult UpdateBranchCommit(string id, string name, string commit)
        {            
            var builder = Builders<Repository>.Filter;
            var filter = builder.Eq(x => x.Id, id) &
                         builder.Where(x => x.Branches.Any(i => i.Name == name));
            var update = Builders<Repository>.Update.Set<string>(x => x.Branches.FirstMatchingElement().LatestCommit, commit);
            
            return repositoryCollection.UpdateOne(filter, update);
        }

        public UpdateResult DeleteBranch(string id, string name)
        {            
            var filter = Builders<Repository>.Filter.Eq<string>(x => x.Id, id);                         
            var update = Builders<Repository>.Update.PullFilter(x => x.Branches, Builders<Branch>.Filter.Eq(x => x.Name, name));
            return repositoryCollection.UpdateOne(filter, update);
        }

        //Methods used to commit
        //Methods were added in this same file, so
        //another MongoClient isn't needed

        public IMongoCollection<Commit> commitCollection =>
             db.GetCollection<Commit>(collectionNames["Collection2"]);


        public IEnumerable<Commit> getAllCommits(string currentBranch)
        {
            var builder = Builders<Commit>.Filter;
            var filter = builder.Eq(x => x.BranchName, currentBranch);
            return commitCollection.Find(filter).ToList();

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



