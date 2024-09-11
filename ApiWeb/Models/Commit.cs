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
        public string Id { get; set; }
        [Required]
        public int Version { get; set; }

        public Commit(string id, int version)
        {
            Id = id;
            Version = version;
        }

        //Aún falta lo de los archivos en sí
    }
}
