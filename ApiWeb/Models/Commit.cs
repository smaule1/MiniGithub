using Microsoft.AspNetCore.Mvc;
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
        [BsonRepresentation(BsonType.ObjectId)]
        public string RepoId { get; set; }

        [Required]
        public string RepoName { get; set; }

        [Required]
        public string BranchName { get; set; }

        [Required]
        public int Version { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public List<ObjectId> FileId { get; set; }

        public List<string> FileName { get; set; }


    }
}
