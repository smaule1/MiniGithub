using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Cryptography;


namespace ApiWeb.Models
{
    public class Repository
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]                
        public List<string> Tags { get; set; }
        [Required]
        [AllowedValues("public", "private")]
        public string Visibility { get; set; }      //Replace for enum. maybe   
        public List<Branch>? Branches { get; set; }




        public void validateCreate()
        {
            Id = null;
            Branches = [new Branch("Master", null)];
        }        
                
        public bool IsBranchNameAvailable(string name)
        {
            if (Branches.IsNullOrEmpty()) return true;             
            foreach (var item in Branches)
            {
                if (item.Name == name) return false; 
            }
            return true;
        }
    }
}
