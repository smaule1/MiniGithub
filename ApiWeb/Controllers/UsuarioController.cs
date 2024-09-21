using ApiWeb.Models;
using ApiWeb.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public IActionResult Get(string id)
        {
            try
            {
                var user = userDb.GetUser(id);

                if(user == null)
                {
                    return NotFound("Could not find requested user.");
                }

                return Ok(user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
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
                return BadRequest(new { error = "Invalid Operation", message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = "Invalid Input", message = ex.Message, type = ex.Message.Split(' ')[1] });
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
            catch (ValidationException ex)
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
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
