using ApiWeb.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
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
        [HttpPut("{id}")]
        public async void CreateUser(string id="default")
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("CREATE (u:User {userid: $id})", new { id }));
            
        }

    }
}
