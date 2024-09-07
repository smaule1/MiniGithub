using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
            return repositorioDB.getRepositorios();
        }

        // GET api/<RepositorioController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RepositorioController>
        [HttpPost]
        public IActionResult Post([FromBody] Repositorio repositorio)
        {
            try
            {
                repositorioDB.Create(repositorio);
                return Ok();
            }
            catch (MongoWriteException ex)
            {                
                return BadRequest(ex.WriteError.Message);
            }                        
        }

        // PUT api/<RepositorioController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RepositorioController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
