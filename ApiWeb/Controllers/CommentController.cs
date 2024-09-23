using ApiWeb.Models;
using ApiWeb.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<Comment> GetAllComments()
        {
            return _commentService.GetComments();
        }

        // GET api/<CommentController>/GetByID
        [HttpGet]
        [Route("GetByID/{id}")]
        public Comment GetCommentById(Guid id)
        {
            return _commentService.GetComment(id);
        }

        [HttpGet]
        [Route("GetByUser/{user}")]
        public IEnumerable<Comment> GetCommentsByUser(string user)
        {
            List<Comment> comments = _commentService.GetComments();
            List<Comment> userComments = [];

            foreach (Comment comment in comments)
            {
                if (user.Equals(comment.User))
                {
                    userComments.Add(comment);
                }


                    List<Subcomment> userSubcomments = [];

                    foreach (Subcomment subcomment in comment.Subcomments)
                    {
                        if (user.Equals(subcomment.User))
                        {
                            userSubcomments.Add(subcomment);
                        }
                    }


                        comment.Subcomments = userSubcomments;
                        userComments.Add(comment);

            }

            return userComments;
        }

        // GET: api/<CommentController>/GetByRepoId
        [HttpGet]
        [Route("GetByRepoId/{repoId}")]
        public IEnumerable<Comment> GetCommentsByRepoId(string repoId)
        {
            List<Comment> comments = _commentService.GetComments();
            List<Comment> repoComments = [];

            foreach (Comment comment in comments)
            {
                if (repoId.Equals(comment.RepoId))
                {
                    repoComments.Add(comment);
                }
            }

            return repoComments;
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
        [Route("UpdateComment/{commentId}/{message}")]
        public IActionResult UpdateComment(Guid commentId, string message)
        {
            try
            {
                Comment comment = _commentService.GetComment(commentId);
                comment.LastDate = DateTime.Now;
                comment.Message = message;

                _commentService.UpdateComment(comment);
                return Ok();
            }
            catch (Exception e)
            {
                string eMessage = _commentService.GetExceptionMessage(e);
                return BadRequest(eMessage);
            }
        }

        // DELETE api/<CommentController>/DeleteComment/5
        [HttpDelete]
        [Route("DeleteComment/{id}")]
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
        [Route("{idComment}/InsertSubcomment")]
        public IActionResult InsertSubcomment(Guid idComment, [FromBody] Subcomment subcomment)
        {
            try
            {
                Comment comment = _commentService.GetComment(idComment);
                comment.Subcomments.Add(subcomment);
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
        [Route("{idComment}/UpdateSubcomment/${message}")]
        public IActionResult Subcomment(Guid idComment, string message, [FromBody] Subcomment antSubcomment)
        {
            try
            {
                Comment comment = _commentService.GetComment(idComment);
                Subcomment? subcomment = comment.Subcomments!.Find(s => s.Equals(antSubcomment));
                if (subcomment != null)
                {
                    subcomment.Message = message;
                    subcomment.LastDate = DateTimeOffset.Now;
                }
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
        [Route("{idComment}/DeleteSubcomment")]
        public IActionResult DeleteSubcomment(Guid idComment, [FromBody] Subcomment antSubcomment)
        {
            try
            {
                Comment comment = _commentService.GetComment(idComment);
                foreach (Subcomment subcomment in comment.Subcomments!)
                {
                    if (subcomment.User.Equals(antSubcomment.User) && subcomment.Message.Equals(antSubcomment.Message))
                    {
                        comment.Subcomments.Remove(subcomment);
                        break;
                    }
                }

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
