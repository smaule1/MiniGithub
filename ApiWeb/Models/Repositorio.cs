using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Cryptography;


namespace ApiWeb.Models
{
    public class Repositorio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [Required]
        public string UsuarioId { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]                
        public List<string> Tags { get; set; }        
        [Required]
        [AllowedValues(["Public","Private"])]        
        public string Visibilidad { get; set; }        
        public List<Branch>? Branches { get; set; }


        public void validateCreate()
        {
            Id = null;
            if (Branches.IsNullOrEmpty())
            {
                Branches = [new Branch("Master", null)];
            }            
        }
                
    }
}
