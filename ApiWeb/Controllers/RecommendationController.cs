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
        [HttpPut("creteuser/{userid}")]
        public async void CreateUser(string userid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("CREATE (u:User {userid: $userid})", new { userid }));
            
        }

        [HttpDelete("deleteuser/{userid}")]
        public async void DeleteUser(string userid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u:User) WHERE u.userid = $userid DETACH DELETE u", new { userid }));
        }


        [HttpPut("createrepo/{userid},{repoid},{vis}")]
        public async void CreateRepo(string userid, string repoid, string vis)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u:User) WHERE u.userid = $userid" +
                " MERGE (r:REPOSITORY {repoid: $repoid, visivility: $vis})" +
                " CREATE (u)-[:Owner_Of]->(r)", new { userid, repoid, vis }));
        }

        [HttpDelete("deleterepo/{repoid}")]
        public async void DeleteRepo(string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (r:REPOSITORY) WHERE r.repoid = $repoid DETACH DELETE r", new { repoid }));
        }

        [HttpPut("subscribeto/{userid},{repoid}")]
        public async void SubscribeTo(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User)" +
                " WHERE u.userid = $userid" +
                " MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = $repoid" +
                " CREATE (u)-[:Subscribe_To]->(r)", new { userid, repoid }));
        }

        [HttpDelete("unsubscribeto/{userid},{repoid}")]
        public async void UnSubscribeTo(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User {userid: $userid})-[s: Subscribe_To]->(r:REPOSITORY {repoid: $repoid})" +
                " DELETE s", new { userid, repoid }));
        }

    }
}
