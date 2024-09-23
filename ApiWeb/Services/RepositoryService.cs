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
using Azure.Core;
using System.Text;
using Amazon.Runtime.Internal;
using Microsoft.Extensions.FileProviders;

namespace ApiWeb.Services
{
    public class RepositoryService
    {
        //Does database operations related with Repositorio model

        public readonly IMongoDatabase db;
        private readonly IDictionary<string, string> collectionNames;
        private readonly GridFSBucket _gridFS;
        private FilterDefinitionBuilder<Commit> builder = Builders<Commit>.Filter;

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
                 Expression(f => new Repository { Id = f.Id, UserId = f.UserId, Name = f.Name, Visibility = f.Visibility, Tags = f.Tags });
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
            var update = Builders<Repository>.Update.PullFilter(x => x.Branches, Builders<Branch>.Filter.Eq(x => x.Name, name) &
                                                                                 Builders<Branch>.Filter.Ne<string>(x => x.Name, "Master"));
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

            var filter = builder.Eq(x => x.BranchName, currentBranch);
            var projection = Builders<Commit>.Projection.Expression(f => new Commit { Id = f.Id, Version = f.Version, Message = f.Message, FileId = f.FileId });
            return commitCollection.Find(filter).Project(projection).ToList();
        }

        public Commit getCommitById(string commitId)
        {
            var filter = builder.Eq(x => x.Id, commitId);
            var projection = Builders<Commit>.Projection.Expression(f => new Commit { RepoName = f.RepoName, BranchName = f.BranchName, Version = f.Version, FileId = f.FileId, FileName = f.FileName });
            return commitCollection.Find(filter).Project(projection).FirstOrDefault();
        }

        public FileStreamResult getFiles(string commitId)
        {

            //Get the Ids of the files
            var objectId = getCommitById(commitId);

            var zipStream = new MemoryStream();
            //List<FileStreamResult> streams = new List<FileStreamResult>();

            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileId in objectId.FileId)
                {
                    var fileInfo = GridFS.Find(Builders<GridFSFileInfo>.Filter.Eq("_id", fileId)).FirstOrDefault();
                    var stream = GridFS.OpenDownloadStream(fileId);

                    var zipEntry = zipArchive.CreateEntry(fileInfo.Filename, CompressionLevel.Optimal);

                    using (var zipEntryStream = zipEntry.Open())
                    {
                        stream.CopyTo(zipEntryStream);
                    }
                }

            }

            zipStream.Position = 0; // Reiniciar la posición para que se lea desde el principio
            return new FileStreamResult(zipStream, "application/zip")
            {
                FileDownloadName = $"{objectId.RepoName}_{objectId.BranchName}_{objectId.Version}.zip"
            };
        }

        public Commit copyCommit(string commitId)
        {
            var filter = builder.Eq(x => x.Id, commitId);
            return commitCollection.Find(filter).FirstOrDefault();
        }
        public void rollback(string commitId, int lastVersion)
        {

            var commit = copyCommit(commitId);

            commit.Id = "";
            commit.Version = lastVersion;

            // Save the commit to Mongo
            commitCollection.InsertOne(commit);

        }

        private async Task<ObjectId> UploadFileAsync(IFormFile file)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument { { "ContentType", file.ContentType } }
            };

            return await GridFS.UploadFromStreamAsync(file.FileName, file.OpenReadStream(), options);
        }

        public async Task createCommit(CommitRequest request)
        {
            try
            {
                var uploadTasks = request.File.Select(file => UploadFileAsync(file)).ToList();
                var fileIds = await Task.WhenAll(uploadTasks);


                // Create a new Commit object
                var commit = new Commit
                {
                    RepoName = request.RepoName,
                    BranchName = request.BranchName,
                    Version = request.Version,
                    Message = request.Message,
                    FileId = fileIds.ToList(),
                    FileName = request.File.Select(f => f.FileName).ToList()
                };

                // Save the commit to Mongo
                await commitCollection.InsertOneAsync(commit);

            }
            catch (MongoWriteException ex)
            {
                throw new Exception("Error al guardar el commit en la base de datos", ex);
            }
        }

        public MyersClass GetDiff(string[] oldLines, string[] newLines)
        {
            var result = new MyersClass();

            var oldSet = new HashSet<string>(oldLines);
            var newSet = new HashSet<string>(newLines);

            foreach (var line in oldLines)
            {
                result.Unchanged.Add(line);
            }

            foreach (var line in newLines)
            {
                if (!oldSet.Contains(line))
                {
                    result.Added.Add($"//{line}");
                }
            }

            return result;
        }

        public string dividirPalabras(string content1, string content2)
        {
            var linesFile1 = content1.Split('\n');
            var linesFile2 = content2.Split('\n');

            var diff = GetDiff(linesFile1, linesFile2);
            var mergedLines = new List<string>();

            // Combine the results
            if (diff.Added.Count != 0)
                mergedLines.Add("// REVISAR MERGE: Lineas con conflicto al final del documento");

            mergedLines.AddRange(diff.Unchanged);

            if (diff.Added.Count != 0)
                mergedLines.Add("// Lineas añadidas en la branch:");

            mergedLines.AddRange(diff.Added);

            return string.Join('\n', mergedLines);

        }

        public void mergeBranches(string commitId1, string commitId2)
        {

            var commit1 = getCommitById(commitId1);
            var commit2 = getCommitById(commitId2);

            //final commit merged
            var commit = new Commit
            {
                Id = "",
                RepoName = commit1.RepoName,
                BranchName = commit1.BranchName,
                Version = commit1.Version + 1,
                Message = $"{commit2.BranchName}-Merged",
                FileId = new List<ObjectId>(),
                FileName = new List<string>()
            };


            //Inserts files from master
            for (int i = 0; i < commit1.FileId.Count; i++)
            {
                var index = commit2.FileName.IndexOf(commit1.FileName[i]);
                if (index >= 0)
                {
                    var file1Content = GridFS.DownloadAsBytes(commit1.FileId[i]);
                    var file2Content = GridFS.DownloadAsBytes(commit2.FileId[index]);

                    string content1 = Encoding.UTF8.GetString(file1Content);
                    string content2 = Encoding.UTF8.GetString(file2Content);

                    var binary = dividirPalabras(content1, content2);

                    var mergedId = GridFS.UploadFromBytes(commit1.FileName[i], Encoding.UTF8.GetBytes(binary));

                    commit.FileId.Add(mergedId);
                    commit.FileName.Add(commit1.FileName[i]);
                } else
                {
                    commit.FileId.Add(commit1.FileId[i]);
                    commit.FileName.Add(commit1.FileName[i]);
                }
                
            }

            //Inserts files from the merged branch
            for (int i = 0; i < commit2.FileId.Count; i++)
            {
                if (!commit1.FileName.Contains(commit2.FileName[i])) { 
                    commit.FileId.Add(commit2.FileId[i]);
                    commit.FileName.Add(commit2.FileName[i]);
                }
            }

            //Inserts the new commit
            commitCollection.InsertOne(commit);
        }
    }
}