using Microsoft.Extensions.Options;
using ApiWeb.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ApiWeb.Data;
using System.Data;
using MongoDB.Driver.Linq;
using Repository = ApiWeb.Models.Repository;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Core.WireProtocol.Messages;
using System.IO.Compression;
using Microsoft.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Rewrite;

namespace ApiWeb.Services
{
    public class RepositoryService
    {
        //Does database operations related with Repositorio model

        public readonly IMongoDatabase db;
        private readonly IDictionary<string, string> collectionNames;
        private readonly GridFSBucket _gridFS;

        public RepositoryService(IOptions<MongoDBSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            db = client.GetDatabase(options.Value.DatabaseName);
            collectionNames = options.Value.Collections;
            _gridFS = new GridFSBucket(db);
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

        public GridFSBucket GridFS => _gridFS;
        public IMongoCollection<Commit> commitCollection =>
             db.GetCollection<Commit>(collectionNames["Collection2"]);


        public IEnumerable<Commit> getAllCommits(string currentBranch)
        {

            var builder = Builders<Commit>.Filter;
            var filter = builder.Eq(x => x.BranchName, currentBranch);
            var projection = Builders<Commit>.Projection.Expression(f => new Commit { Id = f.Id, Version = f.Version, Message = f.Message, FileId = f.FileId });
            return commitCollection.Find(filter).Project(projection).ToList();
        }

        public Commit getCommitById(string commitId)
        {
            var builder = Builders<Commit>.Filter;
            var filter = builder.Eq(x => x.Id, commitId);
            var projection = Builders<Commit>.Projection.Expression(f => new Commit { RepoName = f.RepoName, BranchName = f.BranchName, Version = f.Version, FileId = f.FileId });
            return commitCollection.Find(filter).Project(projection).FirstOrDefault();
        }

        /*
        public int GetLastVersion(string commitId)
        {

            var builder = Builders<Commit>.Filter;
            var filter = builder.Eq(x => x.Id, commitId);
            var project = Builders<Commit>.Projection.Include(f => f.Version).Exclude(f => f.Id);

            var result = commitCollection.Find(filter).Project(project).FirstOrDefault();

            return result != null ? result["Version"].AsInt32 : 0;

        }
        */

        public FileStreamResult getStreams(ObjectId fileId)
        {

            var stream = GridFS.OpenDownloadStream(fileId);

            var builder = Builders<GridFSFileInfo>.Filter;
            var fileInfo = GridFS.Find(builder.Eq("_id", fileId)).FirstOrDefault();

            var fileMetadata = fileInfo.Metadata;
            var fileName = fileInfo.Filename;

            var contentType = fileMetadata["ContentType"].AsString;

            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = fileName
            };

        }

        public FileStreamResult getFiles(string commitId) {

            //Get the Ids of the files
            var objectId = getCommitById(commitId);

            var zipStream = new MemoryStream();
            //List<FileStreamResult> streams = new List<FileStreamResult>();

            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var ele in objectId.FileId)
                {
                    var fileResult = getStreams(ele);
                    var stream = GridFS.OpenDownloadStreamAsync(ele);

                    
                    var zipEntry = zipArchive.CreateEntry(fileResult.FileDownloadName, CompressionLevel.Optimal);
                    using (var fileEntry = zipEntry.Open())
                    {
                        fileResult.FileStream.CopyTo(fileEntry);
                    }
                    
                }
            }

            zipStream.Position = 0; // Reiniciar la posición para que se lea desde el principio
            return new FileStreamResult(zipStream, "application/zip")
            {
                FileDownloadName = $"{objectId.RepoName}_{objectId.BranchName}_{objectId.Version}.zip"
            };
        }

        public void createCommit(CommitRequest request)
        {
            try
            {
                List<ObjectId> fileId = new List<ObjectId>();
                List<string> filename = new List<string>();

                foreach (var ele in request.File) {

                    var options = new GridFSUploadOptions
                    {
                        Metadata = new BsonDocument {{ "ContentType", ele.ContentType
                        }}
                    };

                    // Upload the file to GridFS
                    fileId.Add(GridFS.UploadFromStream(ele.FileName, ele.OpenReadStream(), options));
                    filename.Add(ele.FileName);
                }


                // Create a new Commit object
                var commit = new Commit
                {
                    RepoName = request.RepoName,
                    BranchName = request.BranchName,
                    Version = request.Version,
                    Message = request.Message,
                    FileId = fileId,
                    FileName = filename
                };

                // Save the commit to Mongo
                commitCollection.InsertOne(commit);

            }
            catch (MongoWriteException) { throw; }
        }
    }
}



