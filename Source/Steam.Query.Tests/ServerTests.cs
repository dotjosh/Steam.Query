using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using NUnit.Framework;

namespace Steam.Query.Tests
{
    [TestFixture]
    public class ServerTests
    {
        private List<Server> _servers;

        [TestFixtureSetUp]
        public void Setup()
        {
            var client = new MasterServer();
            var t = client.GetServers(MasterServerRegion.All, MasterServerFilter.Gamedir("arma2arrowpc"));
            t.Wait(TimeSpan.FromSeconds(10));
            _servers = t.Result.ToList();
        }

        [Test]
        [Ignore]
        public void RuleQuery()
        {
            for (var i = 0; i < 10; i++)
            {
                var t = _servers[i].GetServerRules();

                if (t.Wait(TimeSpan.FromSeconds(3)))
                {
                    Assert.That(t.Result.Keys.Count, Is.GreaterThan(2));
                    return;
                }
            }
            Assert.Fail("Tried 10 servers and nothing came back....");
        }
    }
}