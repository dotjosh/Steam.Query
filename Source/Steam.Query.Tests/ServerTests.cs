using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        public void GetServerRulesAsync()
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

        [Test]
        public void GetServerRulesSync()
        {
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var result = _servers[i].GetServerRulesSync(new Server.GetServerInfoSettings());
                    Assert.That(result.Keys.Count, Is.GreaterThan(2));
                    return;
                }
                catch (SocketException){}
            }
            Assert.Fail("Tried 10 servers and nothing came back....");
        }

        [Test]
        public void GetServerInfoAsync()
        {
            for (var i = 0; i < 10; i++)
            {
                var t = _servers[i].GetServerInfo();

                if (t.Wait(TimeSpan.FromSeconds(3)))
                {
                    Assert.That(t.Result.Name.Length, Is.GreaterThan(2));
                    return;
                }
            }
            Assert.Fail("Tried 10 servers and nothing came back....");
        }

        [Test]
        public void GetServerInfoSync()
        {
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var result = _servers[i].GetServerInfoSync(new Server.GetServerInfoSettings());
                    Assert.That(result.Name.Length, Is.GreaterThan(2));
                    return;
                }
                catch (SocketException) { }
            }
            Assert.Fail("Tried 10 servers and nothing came back....");
        }

    }
}