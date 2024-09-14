using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace ApiWeb.Models
{
    public class Repositorio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public List<string> Tags { get; set; } 
        [Required]    
        public string UsuarioId { get; set; }
        [Required]        
        public string Visibilidad { get; set; }         //TODO: the accepted values for visibilidad should be (public, private)
        public List<Branch> Branches { get; set; }

        // poste constructor
        public Repositorio(string? id, string nombre, List<string> tags, string usuarioId, string visibilidad, List<Branch> branches)
        {            
            // id is not used assigned because Mongo assigns the ObjectId automatically
            Nombre = nombre;
            Tags = tags;
            UsuarioId = usuarioId; //TODO: this has to be assigned according to the currently logged user.
            Visibilidad = visibilidad; 

            // repos must always have 1 branch available
            // if no branch is inserted then it is created
            if (branches.IsNullOrEmpty())
            {                
                Branches = [new Branch("Master", null)];
            }
            else
            {
                Branches = branches;
            }
        }                                
    }
}
