using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace ApiWeb.Models
{
    public class Commit
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        public string RepoName { get; set; }

        [Required]
        public string BranchName { get; set; }

        [Required]
        public int Version { get; set; }

        [Required]
        public string Message { get; set; }

        public Commit(string repositoryName, string branchName, int version, string message)
        {
            RepoName = repositoryName;
            BranchName = branchName;
            Version = version;
            Message = message;
        }

        //Aún falta lo de los archivos
    }
}
