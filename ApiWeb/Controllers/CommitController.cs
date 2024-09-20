using ApiWeb.Models;
using ApiWeb.Services;
using CoreApp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Net;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitController : ControllerBase
    {
        private readonly RepositoryService repositorioDB;
        public CommitController(RepositoryService repositorioDB)
        {
            this.repositorioDB = repositorioDB;
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult GetAll(string currentBranch)
        {
            return Ok(repositorioDB.getAllCommits(currentBranch)); 
        }

        /*
        [HttpGet]
        [Route("RetrieveLastVersion")]
        public ActionResult GetLastVersion(string currentCommit)
        {
            return Ok(repositorioDB.GetLastVersion(currentCommit));
        }
        */
        
        [HttpGet]
        [Route("Download/{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            return repositorioDB.getFile(fileId);
        }
        

        [HttpPost]
        [Route("Create")]
        public ActionResult Create(CommitRequest commitRequest)
        {
            try
            {
                repositorioDB.createCommit(commitRequest);
                return Created();

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
