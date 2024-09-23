using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace ApiWeb.Models
{
    public class MyersClass
    {
        public List<string> Added { get; set; } = new List<string>();
        public List<string> Unchanged { get; set; } = new List<string>();
    }
}
