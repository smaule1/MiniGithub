using ApiWeb.Models;
using ApiWeb.Services;
using CoreApp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
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
        public ActionResult RetrieveAll(string currentBranch)
        {
            return Ok(repositorioDB.getAllCommits(currentBranch)); 
        }

        [HttpGet]
        [Route("RetrieveById")]
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


        [HttpPost]
        [Route("Create")]
        public ActionResult Create(Commit commit)
        {
            try
            {
                repositorioDB.createCommit(commit);
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
