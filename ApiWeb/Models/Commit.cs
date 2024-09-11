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
        [Required]
        public string RepositoryName { get; set; }

        [Required]
        public string BranchName { get; set; }

        [Required]
        public int Version { get; set; }

        public Commit(string repositoryName, string branchName, int version)
        {
            RepositoryName = repositoryName;
            BranchName = branchName;
            Version = version;
        }

        //Aún falta lo de los archivos
    }
}
