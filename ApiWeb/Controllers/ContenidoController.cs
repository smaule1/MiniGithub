using CoreApp;
using DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContenidoController : ControllerBase
    {

        [HttpGet]
        [Route("ReatriveByID")]
        public ActionResult RetrieveById(int id)
        {
            try
            {
                var contenido = new ContenidoManager();
                var result = contenido.RetrieveById(id);
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
                var contenido = new ContenidoManager();
                var result = contenido.RetrieveAll();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("Create")]
        public ActionResult Create(Contenido contenido)
        {
            try
            {
                var cont = new ContenidoManager();
                cont.Create(contenido);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("Update")]
        public ActionResult Update(Contenido contenido)
        {
            try
            {
                var cont = new ContenidoManager();
                cont.Update(contenido);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public ActionResult Delete(Contenido contenido)
        {
            try
            {
                var cont = new ContenidoManager();
                cont.Delete(contenido);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
