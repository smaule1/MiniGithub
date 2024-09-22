using Microsoft.Extensions.Options;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using NuGet.Protocol.Core.Types;
using System.Data;
using Neo4j.Driver;
using ApiWeb.Models;
using System;
//using System.Web.Http.Controllers;

namespace ApiWeb.Services
{
    public class RecommendationService
    {
        public void AddPerson(string userid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri ("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("CREATE (u:User {userid: $userid})", new { userid }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }


        public void AddRepo(string userid,string repoid, string vis)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: USER)" +
                " WHERE u.userid = &userid" +
                " CREATE (u)-[:Owner_Of]->(r:REPOSITORY {repoid: $repoid, visivility: $vis)", new { userid,repoid , vis}));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }

        public void RemovePerson(string userid) {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: USER)" +
                " WHERE u.userid = &userid" +
                " DETACH DELETE u", new { userid}));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }

        public void RemoveRepo(string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = &repoid" +
                " DETACH DELETE r", new { repoid }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }

        public void SubscribeTo(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: USER)" +
                " WHERE u.userid = &userid" + 
                " MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = &repoid" +
                " CREATE (u)-[:Subscribe_To]->(r)", new { userid, repoid }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }

        public void UnSubscribeTo(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: USER {userid: &userid})-[s: Subscribe_To]->(r:REPOSITORY {repoid: &repoid})" +
                " DELETE s", new { userid, repoid }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }

        public string GetSubscription(string userid, Task<IResultCursor> subscription)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            subscription = session.ExecuteWriteAsync(tx => {
                var result = tx.RunAsync("MATCH (r:REPOSITORY)<-[s: Subscribe_To]-(u: USER {id: &userid})" +
                " RETURN r.repoid AS subscriptions", new { userid });

                return result;
            });
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;

            return subscription.ToString();
        }


        public void SetLike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: USER)" +
                " WHERE u.userid = &userid" +
                " MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = &repoid" +
                " CREATE (u)-[:Like]->(r)", new { userid, repoid }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }

        public void SetDislike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: USER)" +
                " WHERE u.userid = &userid" +
                " MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = &repoid" +
                " CREATE (u)-[:Dislike]->(r)", new { userid, repoid }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }

        public void TaggedWith(string repoid, string tag)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (r: REPOSITORY)" +
                " WHERE r.repoid = &repoid" +
                " MERGE (t:TAG {tag: &tag})" +
                " MERGE (r)-[:Tagged_With]->(t)", new { repoid, tag }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;

        }

        public void RemoveTag(string tag)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (t: TAG)" +
                " WHERE t.tag = &tag" +
                " DETACH DELETE t", new { tag }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }

        public void RemoveLike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: USER {userid: &userid})-[l: Like]->(r:REPOSITORY {repoid: &repoid})" +
                " DELETE l", new { userid, repoid }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }

        public void RemoveDislike(string userid, string repoid)
        {
            IDriver Driver = GraphDatabase.Driver(new Uri("neo4j + s://57e8bb3a.databases.neo4j.io"), AuthTokens.Basic("neo4j", "dm7ul4qcPi1XK-_hWO6NXtbcACml6dqfWGmxrgaW7EA"));
            using var session = Driver.AsyncSession();
            session.ExecuteWriteAsync(tx => tx.RunAsync("MATCH (u: USER {userid: &userid})-[d: Dislike]->(r:REPOSITORY {repoid: &repoid})" +
                " DELETE d", new { userid, repoid }));
            ValueTask valueTask = session.DisposeAsync();
            _ = valueTask;
        }


    }
}




