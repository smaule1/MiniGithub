using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController(CommentService commentService) : ControllerBase
    {
        private readonly CommentService _commentService = commentService;


        // GET: api/<CommentController>/GetAll
        [HttpGet]
        [Route("GetAll")]
        public IEnumerable<Comment> GetComments()
        {
            return _commentService.GetComments();
        }

        // GET api/<CommentController>/GetByID/5
        [HttpGet]
        [Route("GetByID")]
        public Comment GetCommentById(Guid id)
        {
            return _commentService.GetComment(id);
        }

        [HttpGet]
        [Route("GetByUser")]
        public IEnumerable<Comment> GetCommentByUser(string user)
        {
            List<Comment> comments = _commentService.GetComments();
            List<Comment> userComments = [];

            foreach (Comment comment in comments)
            {
                if (user.Equals(comment.User))
                {
                    userComments.Add(comment);
                }
                else if (!comment.Subcomments.IsNullOrEmpty())
                {
                    List<Subcomment> userSubcomments = [];

                    foreach (Subcomment subcomment in comment.Subcomments)
                    {
                        if (user.Equals(subcomment.User))
                        {
                            userSubcomments.Add(subcomment);
                        }
                    }

                    if (!userSubcomments.IsNullOrEmpty())
                    {
                        comment.Subcomments = userSubcomments;
                        userComments.Add(comment);
                    }
                }
            }

            return userComments;
        }

        // POST api/<CommentController>/InsertComment
        [HttpPost]
        [Route("InsertComment")]
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

        // PUT api/<CommentController>/UpdateComment/5
        [HttpPut]
        [Route("UpdateComment")]
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

        // DELETE api/<CommentController>/DeleteComment/5
        [HttpDelete]
        [Route("DeleteComment")]
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

        // POST api/<CommentController>/InsertSubcomment/5
        [HttpPost]
        [Route("InsertSubcomment")]
        public IActionResult InsertSubcomment(Guid idComment, [FromBody] Subcomment subcomment)
        {
            try
            {
                Comment comment = _commentService.GetComment(idComment);
                List<Subcomment> subcomments = comment.Subcomments;
                if (subcomments != null)
                {
                    subcomments.Add(subcomment);
                }
                else
                {
                    subcomments = new List<Subcomment>([subcomment]);
                }
                comment.Subcomments = subcomments;
                _commentService.UpdateComment(comment);
                return Ok();
            }
            catch (Exception e)
            {
                string message = _commentService.GetExceptionMessage(e);
                return BadRequest(message);
            }
        }

        // DELETE api/<CommentController>/UpdateSubcomment/5
        [HttpPut]
        [Route("UpdateSubcomment")]
        public IActionResult Subcomment(Guid idComment, string message, DateTimeOffset date, [FromBody] Subcomment antSubcomment)
        {
            try
            {
                Comment comment = _commentService.GetComment(idComment);
                Subcomment subcomment = new(antSubcomment.User, message, date);
                comment.Subcomments.Remove(antSubcomment);
                comment.Subcomments.Add(subcomment);
                _commentService.UpdateComment(comment);
                return Ok();
            }
            catch (Exception e)
            {
                string eMessage = _commentService.GetExceptionMessage(e);
                return BadRequest(eMessage);
            }
        }

        // DELETE api/<CommentController>/DeleteSubcomment/5
        [HttpDelete]
        [Route("DeleteSubcomment")]
        public IActionResult DeleteSubcomment(Guid idComment, [FromBody] Subcomment antSubcomment)
        {
            try
            {
                Comment comment = _commentService.GetComment(idComment);
                comment.Subcomments.Remove(antSubcomment);
                _commentService.UpdateComment(comment);
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
