using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Mvc;

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
        public void Post([FromBody] Repositorio repositorio)
        {
            repositorioDB.Create(repositorio);
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
