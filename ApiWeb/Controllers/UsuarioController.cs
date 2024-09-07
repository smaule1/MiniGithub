using CoreApp;
using DataAcces.CRUD;
using DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApiWeb.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class UsuarioController : ControllerBase
    {

        [HttpGet]
        [Route("RetrieveByID")]
        public ActionResult RetrieveById(int id)
        {
            try
            {
                var usuario = new UsuarioManager();
                var result = usuario.RetrieveById(id);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

		[HttpGet]
		[Route("RetrieveAll")]
		public ActionResult RetrieveAll()
		{
			try
			{
				var usuario = new UsuarioManager();
				var result = usuario.RetrieveAll();
				return Ok(result);

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
        [Route("Create")]
        public ActionResult Create(Usuario usuario)
        {
            try
            {
                var user = new UsuarioManager();
                user.Create(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("Update")]
        public ActionResult Update(Usuario usuario)
        {
            try
            {
                var user = new UsuarioManager();
                user.Update(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public ActionResult Delete(Usuario usuario)
        {
            try
            {
                var user = new UsuarioManager();
                user.Delete(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult Login(string Email, string Password)
        {
            try
            {

                var um = new UsuarioManager();

                var user = um.Login(Email, Password);

                if (user == null)
                {
                    return BadRequest("Usuario invalido");
                }
                else
                {

                    return Ok(user);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }
}
