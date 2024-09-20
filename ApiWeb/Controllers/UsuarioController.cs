using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Create([FromBody] UserUpdateDTO user)
        {
            try
            {
                
                userDb.CreateUser(user.Email, user.Password, user.Name);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] UserUpdateDTO user)
        {
            try
            {
                userDb.EditUser(id, user.Name, user.Password);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
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
