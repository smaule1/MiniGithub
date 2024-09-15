using ApiWeb.Models;
using ApiWeb.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NuGet.Protocol.Core.Types;
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
        


        //============ Repositories ====================================

        //POST
        [HttpPost]
        public IActionResult PostPublic([FromBody] Repositorio repositorio) 
        {
            try
            {
                repositorio.validateCreate();                
                repositorioDB.Create(repositorio);

                //TODO: If is a public repo, it needs a node in the graph for suscribes and likes
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return BadRequest(new { error = "Duplicate Key Error", message = $"You alreday have a repository named '{repositorio.Nombre}'"});
                return BadRequest(ex.WriteError);
            }
        }

        //PUT
        [HttpPut("{id}")]
        public IActionResult PutPublic(string id, [FromBody] Repositorio repositorio)  //TODO: Cambiar por atributos nombre y tags nada más
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            try
            {
                var result = repositorioDB.Update(repositorio, id);
                if(result.IsAcknowledged && result.MatchedCount == 0) { return NotFound("Repositorio not found"); }
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return BadRequest(new { error = "Duplicate Key Error", message = $"You alreday have a repository named '{repositorio.Nombre}'"});
                return BadRequest(ex.WriteError);
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
        [HttpGet("Publico/{id}")]
        public IActionResult GetPublicById(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            var repositorio = repositorioDB.GetPublicRepositorioById(id);

            return (repositorio == null) ? NotFound("Repositorio not found") : Ok(repositorio);
        }

        //GET public repo by name, returns simple repo      
        [HttpGet("Publico/byName/{name}")]
        public IActionResult GetPublicByName(string name)
        {
            return Ok(repositorioDB.GetPublicRepositorioByName(name));
        }

        //GET all public repos by user_id, returns simple repo
        [HttpGet("Publico/All/{user_id}")]
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
        [HttpGet("Privado/{id}")]
        public IActionResult GetPrivateById(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            var repositorio = repositorioDB.GetPrivateRepositorioById(id);

            return (repositorio == null) ? NotFound("Repositorio not found") : Ok(repositorio);
        }




        //TODO: POST Branch - name must be unique, param(id_repo)
        //TODO: PUT Branch name - name must be unique, param(id_repo, name)
        //TODO: PUT Branch commit - param(id_repo, commit)
        //TODO: DELETE Branch - param(id)


        //TODO: Make a private repo -> public 









    }
}
