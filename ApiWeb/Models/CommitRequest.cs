using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace ApiWeb.Models
{
    public class CommitRequest
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

        public string? Message { get; set; }

        [Required]
        public IFormFile File { get; set; }


    }
}
