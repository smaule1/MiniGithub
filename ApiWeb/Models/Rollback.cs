using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace ApiWeb.Models
{
    public class Rollback
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Commit { get; set; }

        [Required]
        public int Version { get; set; }

    }
}
