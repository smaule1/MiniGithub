﻿using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

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
                    return NotFound(new { error = "Not found", message = "No se encontró al usuario solicitado." });
                }

                return Ok(user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = "Invalid Input", message = ex.Message });
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

        [HttpPut]
        public IActionResult Put([FromBody] UserUpdateDTO user)
        {
            try
            {
                userDb.EditUser(user.Email, user.Name, user.Password, user.OldPassword);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = "Invalid Operation", message = ex.Message });
            }
            catch (ValidationException ex)
            {
                if (ex.Message.Split(' ')[1] == "contraseña")
                {
                    if (ex.Message.Split(' ')[2] == "introducida")
                    {
                        return BadRequest(new { error = "Invalid Input", message = ex.Message, type = "Wrong Current" });
                    }
                    return BadRequest(new { error = "Invalid Input", message = ex.Message, type = "Wrong New" });
                }
                return BadRequest(new { error = "Invalid Input", message = ex.Message, type = "Nombre" });
            }
        }


        [HttpDelete]
        public IActionResult Delete([FromQuery] string userId)
        {
            try
            {
                userDb.DeleteUser(userId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = "Invalid Operation", message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return NotFound(new { error = "Invalid Input", message = ex.Message });
            }
        }
    }
}
