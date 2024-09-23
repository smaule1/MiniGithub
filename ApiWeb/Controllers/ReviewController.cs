using CoreApp;
using DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {

        [HttpGet]
        [Route("RetrieveByID")]
        public ActionResult RetrieveById(int id)
        {
            try
            {
                var review = new ReviewManager();
                var result = review.RetrieveById(id);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveByUserId")]
        public ActionResult RetrieveByUserId(int usuarioId)
        {
            try
            {
                var reviewManager = new ReviewManager();
                var result = reviewManager.RetrieveByUserId(usuarioId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("RetrieveByContentId")]
        public ActionResult RetrieveByContentId(int contenidoId)
        {
            try
            {
                var reviewManager = new ReviewManager();
                var result = reviewManager.RetrieveByContentId(contenidoId);
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
                var review = new ReviewManager();
                var result = review.RetrieveAll();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Create")]
        public ActionResult Create(Review review)
        {
            try
            {
                var rev = new ReviewManager();
                rev.Create(review);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("Update")]
        public ActionResult Update(Review review)
        {
            try
            {
                var rev = new ReviewManager();
                rev.Update(review);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public ActionResult Delete(Review review)
        {
            try
            {
                var rev = new ReviewManager();
                rev.Delete(review);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
