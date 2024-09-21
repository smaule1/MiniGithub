using ApiWeb.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Net;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService recommendationDB;

        public RecommendationController(RecommendationService recommendationDB)
        {
            this.recommendationDB = recommendationDB;
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<IActionResult> Create(string id)
        {
             recommendationDB.AddPerson(id);

            return Ok();
        }

    }
}
