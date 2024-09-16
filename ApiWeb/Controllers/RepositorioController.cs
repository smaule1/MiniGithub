using ApiWeb.Models;
using ApiWeb.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Net;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositorioController : ControllerBase
    {
        private readonly RepositorioService repositorioDB;
        public RepositorioController(RepositorioService repositorioDB)
        {
            this.repositorioDB = repositorioDB;
        }

        //Assumption: Methods can update any kind of repo (public or private), just by knowing the id.
        //            The api assumes that just the logged user can get their own repos ids.
        


        //============ Repository ====================================

        //POST
        [HttpPost]
        public IActionResult Post([FromBody] Repositorio repositorio) 
        {
            try
            {
                repositorio.validateCreate();                
                repositorioDB.Create(repositorio);

                //TODO: If is a public repo, it needs a node in the graph for suscribes and likes
                return Created();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return Conflict(new { error = "Duplicate Key Error", message = $"You alreday have a repository named '{repositorio.Nombre}'"});
                return Conflict(ex.WriteError);
            }
        }

        //PUT
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] Repositorio repositorio)  //TODO: Cambiar por atributos nombre y tags nada más
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            try
            {
                var result = repositorioDB.Update(repositorio, id);
                if (result.IsAcknowledged && result.MatchedCount == 0) { return NotFound("Repositorio not found"); }
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return Conflict(new { error = "Duplicate Key Error", message = $"You alreday have a repository named '{repositorio.Nombre}'"});
                return Conflict(ex.WriteError);
            }
        }

        // DELETE 
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");

            return Ok(repositorioDB.Delete(id));
        }


        //GET public repo by id, returns full repo
        [HttpGet("public/{id}")]
        public IActionResult GetPublicById(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            var repositorio = repositorioDB.GetRepositorioById(id, "public");

            return (repositorio == null) ? NotFound("Repositorio not found") : Ok(repositorio);
        }

        //GET public repo by name, returns simple repo      
        [HttpGet("public/name/{name}")]
        public IActionResult GetPublicByName(string name)
        {
            return Ok(repositorioDB.GetPublicRepositorioByName(name));
        }

        //GET all public repos by user_id, returns simple repo
        [HttpGet("public/all/{userId}")]
        public IActionResult GetAllPublic(string user_id)
        {
            return Ok(repositorioDB.GetAllRepositorios(user_id, "public"));
        }

        //GET all private repos by user_id, returns simple repo
        [HttpGet("Privado/All/{user_id}")]
        public IActionResult GetAllPrivate(string user_id)
        {
            return Ok(repositorioDB.GetAllRepositorios(user_id, "private"));
        }

        //GET private repo by id & user_id, returns full repo
        [HttpGet("private/{id}")]
        public IActionResult GetPrivateById(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            var repositorio = repositorioDB.GetRepositorioById(id, "private");

            return (repositorio == null) ? NotFound("Repositorio not found") : Ok(repositorio);
        }

        //TODO: Make a private repo -> public , i think it is not part of the requirements       

        // ====================== Branch ===================================


        //POST Branch
        [HttpPost("{id}/branch")]
        public IActionResult PostBranch(string id, [FromBody] Branch branch)
        {
            try
            {
                if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
                var result = repositorioDB.CreateBranch(id, branch);
                if (result.IsAcknowledged && result.ModifiedCount == 0) { return Conflict("Creation failed"); }
                return Created();
            }
            catch (BadHttpRequestException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return Conflict(ex.Message);}            
        }
        
        //PUT Branch commit
        [HttpPut("{id}/branch/{name}")]
        public IActionResult Put(string id, string name, [FromBody] string commit)  
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
                        
            var result = repositorioDB.UpdateBranchCommit(id, name, commit);
            if (result.IsAcknowledged && result.MatchedCount == 0) { return NotFound("Combination of Repository and Name not found"); }
            return Ok();                      
        }

        // DELETE Branch
        [HttpDelete("{id}/branch/{name}")]
        public IActionResult DeleteBranch(string id, string name)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");

            var result = repositorioDB.DeleteBranch(id, name);
            if (result.IsAcknowledged && result.ModifiedCount == 0) { return NotFound("Combination of Repository and Name not found"); }
            return Ok();
        }      


        //TODO: convert all routes to lowercase
        //TODO: convert all text to english




    }
}
