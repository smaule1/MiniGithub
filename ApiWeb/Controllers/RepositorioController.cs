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
        



        // GET: api/<RepositorioController>
        [HttpGet]
        public IEnumerable<Repositorio> Get()
        {
            return repositorioDB.GetAllRepositorios();     //TODO: this request is just for debugging purposes, should not be used in final version
        }

        // GET api/<RepositorioController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        //TODO: GET all public repos

        //TODO: GET all private repos by user_id, Authentication

        //TODO GET all public repos the user is suscribed, needs request to the graph database, Authentication

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
            //TODO: Authentication, how public repos are deleted? when no one is suscribed?
            try
            {
                repositorioDB.Delete(id);
                return Ok();
            }
            catch (System.FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }



        //TODO: CRUD BRANCH



    }
}
