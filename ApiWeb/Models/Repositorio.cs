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
        public string Visibilidad { get; set; }        
        public List<Branch> Branches { get; set; }

        public Repositorio(string? id, string nombre, List<string> tags, string usuarioId, string visibilidad, List<Branch> branches)
        {            
            Nombre = nombre;
            Tags = tags;
            UsuarioId = usuarioId;
            Visibilidad = visibilidad;            
            if (branches.IsNullOrEmpty())
            {                
                Branches = [new Branch("Master", null)];
            }
            else
            {
                Branches = branches;
            }
        }        


        //faltan branches
        //tags tiene que ser array
        //en general todo esta mal y hay que cambiarlo despues
    }
}
