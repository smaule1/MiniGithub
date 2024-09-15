using ApiWeb.Models;
using ApiWeb.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

        //============== Public ====================

        //POST
        [HttpPost("Publico")]
        public IActionResult PostPublic([FromBody] Repositorio repositorio) 
        {

            try
            {
                repositorio.validateCreate();
                repositorio.validatePublic();
                repositorioDB.Create(repositorio);
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return BadRequest(new { error = "Duplicate Key Error", message = $"There is already a public repository named '{repositorio.Nombre}'" });
                return BadRequest(ex.WriteError);
            }
        }

        //PUT
        [HttpPut("Publico/{id}")]
        public IActionResult PutPublic(string id, [FromBody] Repositorio repositorio)
        {
            try
            {
                repositorioDB.Update(repositorio, id);
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return BadRequest(new { error = "Duplicate Key Error", message = $"There is already a public repository named '{repositorio.Nombre}'" });
                return BadRequest(ex.WriteError);
            }
        }

        //GET public repo by id
        [HttpGet("Publico/byId/{id}")]
        public IActionResult GetPublicById(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            var repositorio = repositorioDB.GetPublicRepositorioById(id);

            return (repositorio == null) ? NotFound("Repositorio not found") : Ok(repositorio);
        }

        //GET public repo by name        
        [HttpGet("Publico/byName/{name}")]
        public IEnumerable<Repositorio> GetPublicByName(string name)
        {
            return repositorioDB.GetPublicRepositorioByName(name);
        }

        //TODO GET all public repos the user is suscribed, needs request to the graph database, Authentication// by user_id

        // DELETE 
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");

            return Ok(repositorioDB.Delete(id));
        }


        //===============  Private  =======================


        //POST
        [HttpPost("Private")]
        public IActionResult PostPrivate([FromBody] Repositorio repositorio)
        {

            try
            {
                repositorio.validateCreate();
                repositorio.validatePrivate();
                repositorioDB.Create(repositorio);
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return BadRequest(new { error = "Duplicate Key Error", message = $"You alreday have a repository named '{repositorio.Nombre}'" });
                return BadRequest(ex.WriteError);
            }catch (Exception ex) { return BadRequest(ex.Message); }

        }

        //PUT
        [HttpPut("Private/{id}")]
        public IActionResult PutPrivate(string id, [FromBody] Repositorio repositorio)
        {
            try
            {
                repositorioDB.Update(repositorio, id);
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return BadRequest(new { error = "Duplicate Key Error", message = $"You alreday have a repository named '{repositorio.Nombre}'" });
                return BadRequest(ex.WriteError);
            }
        }

        //GET all private repos by user_id
        [HttpGet("Private/{user_id}")]
        public IEnumerable<Repositorio> GetAllPrivate(string user_id)
        {
            return repositorioDB.GetAllPrivateRepositorio(user_id);
        }

        //GET private repo by id & user_id
        [HttpGet("Private/{user_id}/byId/{id}")]
        public IActionResult GetPrivateById(string user_id, string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            var repositorio = repositorioDB.GetPrivateRepositorioById(user_id, id);

            return (repositorio == null) ? NotFound("Repositorio not found") : Ok(repositorio);
        }

     

        //TODO: CRUD BRANCH



    }
}
