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
        

        // POST api/<ValuesController>
        [HttpPost("createuser/{userid}")]
        public async Task<OkResult> CreateUser(string userid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("CREATE (u:User {userid: $userid})", new { userid }));

            return Ok();

        }

        [HttpDelete("deleteuser/{userid}")]
        public async Task<OkResult> DeleteUser(string userid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u:User) WHERE u.userid = $userid DETACH DELETE u", new { userid }));

            return Ok();
        }


        [HttpPost("createrepo/{userid},{repoid},{vis}")]
        public async Task<OkResult> CreateRepo(string userid, string repoid, string vis)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u:User) WHERE u.userid = $userid" +
                " MERGE (r:REPOSITORY {repoid: $repoid, visivility: $vis})" +
                " CREATE (u)-[:Owner_Of]->(r)", new { userid, repoid, vis }));

            return Ok();
        }

        [HttpDelete("deleterepo/{repoid}")]
        public async Task<OkResult> DeleteRepo(string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (r:REPOSITORY) WHERE r.repoid = $repoid DETACH DELETE r", new { repoid }));

            return Ok();
        }

        [HttpPost("subscribeto/{userid},{repoid}")]
        public async Task<OkResult> SubscribeTo(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User)" +
                " WHERE u.userid = $userid" +
                " MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = $repoid" +
                " CREATE (u)-[:Subscribe_To]->(r)", new { userid, repoid }));

            return Ok();
        }

        [HttpDelete("unsubscribeto/{userid},{repoid}")]
        public async Task<OkResult> UnSubscribeTo(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User {userid: $userid})-[s: Subscribe_To]->(r:REPOSITORY {repoid: $repoid})" +
                " DELETE s", new { userid, repoid }));

            return Ok();
        }

        [HttpPost("like/{userid},{repoid}")]
        public async Task<OkResult> SetLike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User)" +
                " WHERE u.userid = $userid" +
                " MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = $repoid" +
                " CREATE (u)-[:Like]->(r)", new { userid, repoid }));

            return Ok();
        }

        [HttpDelete("removelike/{userid},{repoid}")]
        public async Task<OkResult> RemoveLike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User {userid: $userid})-[l: Like]->(r:REPOSITORY {repoid: $repoid})" +
                " DELETE l", new { userid, repoid }));

            return Ok();
        }

        [HttpPost("dislike/{userid},{repoid}")]
        public async Task<OkResult> SetDislike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User)" +
                " WHERE u.userid = $userid" +
                " MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = $repoid" +
                " CREATE (u)-[:Dislike]->(r)", new { userid, repoid }));

            return Ok();
        }

        [HttpDelete("removedislike/{userid},{repoid}")]
        public async Task<OkResult> RemoveDislike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User {userid: $userid})-[d: Dislike]->(r:REPOSITORY {repoid: $repoid})" +
                " DELETE d", new { userid, repoid }));

            return Ok();
        }

        [HttpPost("puttag/{repoid},{tag}")]
        public async Task<OkResult> TaggedWith(string repoid, string tag)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = $repoid" +
                " MERGE (t:TAG {tag: $tag})" +
                " MERGE (r)-[:Tagged_With]->(t)", new { repoid, tag }));

            return Ok();
        }

        [HttpDelete("removetag/{repoid},{tag}")]
        public async Task<OkResult> Removetag(string repoid, string tag)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (r:REPOSITORY {repoid: $repoid})-[x:Tagged_With]->(t:TAG {tag: $tag})" +
                " DELETE x", new { repoid, tag }));

            return Ok();
        }

        [HttpGet("getlikedrepos/{userid}")]
        public async Task<List<string>> GetLikedRepos(string userid)
        {
            var records = new List<string>();
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            await using var session = Driver.AsyncSession();

            var reader = await session.RunAsync(
                "MATCH (u: User)-[:Like]->(r:REPOSITORY) WHERE u.userid = $userid RETURN r.repoid",
                new { userid }
            );

            while (await reader.FetchAsync())
            {
                records.Add(reader.Current[0].ToString());
            }

            return records;
        }

        [HttpGet("getdislikedrepos/{userid}")]
        public async Task<List<string>> GetDislikedRepos(string userid)
        {
            var records = new List<string>();
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            await using var session = Driver.AsyncSession();

            var reader = await session.RunAsync(
                "MATCH (u: User)-[:Dislike]->(r:REPOSITORY) WHERE u.userid = $userid RETURN r.repoid",
                new { userid }
            );

            while (await reader.FetchAsync())
            {
                records.Add(reader.Current[0].ToString());
            }

            return records;
        }

        [HttpGet("getSubscribedrepos/{userid}")]
        public async Task<List<string>> GetSubscribedRepos(string userid)
        {
            var records = new List<string>();
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            await using var session = Driver.AsyncSession();

            var reader = await session.RunAsync(
                "MATCH (u: User)-[:Subscribe_To]->(r:REPOSITORY) WHERE u.userid = $userid RETURN r.repoid",
                new { userid }
            );

            while (await reader.FetchAsync())
            {
                records.Add(reader.Current[0].ToString());
            }

            return records;
        }

        [HttpGet("getReposByTag/{tag}")]
        public async Task<List<string>> GetReposByTag(string tag)
        {
            var records = new List<string>();
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            await using var session = Driver.AsyncSession();

            var reader = await session.RunAsync(
                "MATCH (r:REPOSITORY)-[:Tagged_With]->(t:TAG) WHERE t.tag = $tag RETURN r.repoid",
                new { tag }
            );

            while (await reader.FetchAsync())
            {
                records.Add(reader.Current[0].ToString());
            }

            return records;
        }

        [HttpGet("getRecomByTags/{userid}")]
        public async Task<List<string>> GetRecomByTags(string userid)
        {
            var records = new List<string>();
            var vis = "public";
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            await using var session = Driver.AsyncSession();

            var reader = await session.RunAsync(
                "MATCH (u:User {userid: $userid})-[:Owner_Of|Subscribe_To]->(r:REPOSITORY)-[:Tagged_With]->(t:TAG)<-[:Tagged_With]-(o:REPOSITORY {visivility: $vis})" +
                " RETURN .repoid",
                new { userid, vis }
            );

            while (await reader.FetchAsync())
            {
                records.Add(reader.Current[0].ToString());
            }

            return records;
        }

    }
}
