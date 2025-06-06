﻿using ApiWeb.Services;
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

        [HttpPut("like/{userid},{repoid}")]
        public async void SetLike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User)" +
                " WHERE u.userid = $userid" +
                " MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = $repoid" +
                " CREATE (u)-[:Like]->(r)", new { userid, repoid }));
        }

        [HttpDelete("removelike/{userid},{repoid}")]
        public async void RemoveLike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User {userid: $userid})-[l: Like]->(r:REPOSITORY {repoid: $repoid})" +
                " DELETE l", new { userid, repoid }));
        }

        [HttpPut("dislike/{userid},{repoid}")]
        public async void SetDislike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User)" +
                " WHERE u.userid = $userid" +
                " MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = $repoid" +
                " CREATE (u)-[:Dislike]->(r)", new { userid, repoid }));
        }

        [HttpDelete("removedislike/{userid},{repoid}")]
        public async void RemoveDislike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: User {userid: $userid})-[d: Dislike]->(r:REPOSITORY {repoid: $repoid})" +
                " DELETE d", new { userid, repoid }));
        }

        [HttpPut("puttag/{repoid},{tag}")]
        public async void TaggedWith(string repoid, string tag)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = $repoid" +
                " MERGE (t:TAG {tag: $tag})" +
                " MERGE (r)-[:Tagged_With]->(t)", new { repoid, tag }));
        }

        [HttpDelete("removetag/{repoid},{tag}")]
        public async void Removetag(string repoid, string tag)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j+s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "CW9aqx5BQXhFnyWt-hXoyG_ywsdp9r1TFEcUolala_c"));
            using var session = Driver.AsyncSession();
            await session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (r:REPOSITORY {repoid: $repoid})-[x:Tagged_With]->(t:TAG {tag: $tag})" +
                " DELETE x", new { repoid, tag }));
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


    }
}
