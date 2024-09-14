using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController (CommentService commentService)
        {
            _commentService = commentService;
        }


        // GET: api/<CommentController>
        [HttpGet]
        public IEnumerable<Comment> GetComments()
        {
            return _commentService.GetComments();
        }

        // GET api/<CommentController>/5
        [HttpGet("{id}")]
        public Comment GetComment(Guid id)
        {
            return _commentService.GetComment(id);
        }

        // POST api/<CommentController>
        [HttpPost]
        public IActionResult InsertComment([FromBody] Comment comment)
        {
            try
            {
                _commentService.CreateComment(comment);
                return Ok();
            }
            catch (Exception e)
            {
                string message = _commentService.GetExceptionMessage(e);
                return BadRequest(message);
            }
        }

        // PUT api/<CommentController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateComment(Guid id, [FromBody] Comment comment)
        {
            try
            {
                if (comment.Id == id)
                {
                    _commentService.UpdateComment(comment);
                }
                return Ok();
            }
            catch (Exception e)
            {
                string message = _commentService.GetExceptionMessage(e);
                return BadRequest(message);
            }
        }

        // DELETE api/<CommentController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteComment(Guid id)
        {
            try
            {
                _commentService.DeleteComment(id);
                return Ok();
            }
            catch (Exception e)
            {
                string message = _commentService.GetExceptionMessage(e);
                return BadRequest(message);
            }
        }
    }
}
