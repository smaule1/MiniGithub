using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;

namespace ApiWeb.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class UsuarioController : ControllerBase
    {
        private readonly UserService userDb;

        public UsuarioController(UserService userDb)
        {
            this.userDb = userDb;
        }

        [HttpGet]
        public IEnumerable<User?> Get()
        {
            return userDb.GetUsers();
        }

        [HttpGet("{id}")]
        public User? Get(string id)
        {
            return userDb.GetUser(id);
        }

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            try
            {
                userDb.CreateUser(user);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public void Put(string id, [FromBody] string value)
        {
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                userDb.DeleteUser(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
