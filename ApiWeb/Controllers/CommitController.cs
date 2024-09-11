using ApiWeb.Models;
using ApiWeb.Services;
using CoreApp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitController : ControllerBase
    {
        private readonly RepositorioService repositorioDB;
        public CommitController(RepositorioService repositorioDB)
        {
            this.repositorioDB = repositorioDB;
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public IEnumerable<Commit> Get()
        {
            return repositorioDB.getCommits();
        }

        [HttpPost]
        [Route("Create")]
        public ActionResult Create(Commit commit)
        {
            try
            {
                repositorioDB.CreateCommit(commit);
                return Ok();
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return BadRequest("Duplicate Key Error.");
                return BadRequest(ex.WriteError);
            }
        }
    }
}
