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



        //GET public repo by id
        [HttpGet("getPublic/byId/{id}")]
        public IActionResult GetPublicById(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            var repositorio = repositorioDB.GetPublicRepositorioById(id);

            return (repositorio == null) ? NotFound("Repositorio not found") : Ok(repositorio);
        }

        //GET public repo by name        
        [HttpGet("getPublic/byName/{name}")]
        public IEnumerable<Repositorio> GetPublicByName(string name)
        {
            return repositorioDB.GetPublicRepositorioByName(name);
        }


        //TODO GET all public repos the user is suscribed, needs request to the graph database, Authentication

        
        //GET private repo by id
        [HttpGet("getPrivate/{user_id}")]
        public IEnumerable<Repositorio> GetAllPrivate(string user_id)
        {
            return repositorioDB.GetAllPrivateRepositorio(user_id);
        }
        
        //GET private repos by id & user_id, Authentication         
        [HttpGet("getPrivate/{user_id}/byId/{id}")]
        public IActionResult GetPrivateById(string user_id, string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");
            var repositorio = repositorioDB.GetPrivateRepositorioById(user_id, id);
            
            return (repositorio == null) ? NotFound("Repositorio not found") : Ok(repositorio);                   
        }





        // POST api/<RepositorioController>
        [HttpPost]
        public IActionResult Post([FromBody] Repositorio repositorio)  //maybe change for attributes
        {
            //TODO: Authentication
            try
            {
                repositorio.validateCreate();
                repositorioDB.Create(repositorio);
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return BadRequest("Duplicate Key Error: Combination of Nombre and UsuarioId already exists.");
                return BadRequest(ex.WriteError);
            }                        
        }

        // PUT api/<RepositorioController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] Repositorio repositorio)
        {
            //TODO: Authentication
            try
            {
                repositorioDB.Update(repositorio, id);
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return BadRequest("Duplicate Key Error: Combination of Nombre and UsuarioId already exists.");
                return BadRequest(ex.WriteError);
            }
        }

        // DELETE api/<RepositorioController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return BadRequest($"'{id}' is not a valid id.");

            return Ok(repositorioDB.Delete(id));
        }


  
        //TODO: CRUD BRANCH



    }
}
