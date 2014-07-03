using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using NUnit.Framework;

namespace Steam.Query.Tests
{
    [TestFixture]
    public class ServerTests
    {
        private Server _server;

        [TestFixtureSetUp]
        public void Setup()
        {
            var client = new MasterServer();
            var t = client.GetServers(MasterServerRegion.All, MasterServerFilter.Gamedir("arma2arrowpc"));
            t.Wait(TimeSpan.FromSeconds(10));
            _server = t.Result.Skip(5).First();
        }

        [Test]
        [Ignore]
        public void RuleQuery()
        {
            var t = _server.GetServerRules();

            t.Wait(TimeSpan.FromSeconds(10));
            Assert.That(t.Result.Keys.Count, Is.GreaterThan(2));
        }
    }
}