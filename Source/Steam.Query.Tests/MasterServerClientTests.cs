using System;
using System.Linq;
using NUnit.Framework;

namespace Steam.Query.Tests
{
    [TestFixture]
    public class MasterServerClientTests
    {
        [Test]
        public void BasicQuery()
        {
            var client = new MasterServer();
            var t = client.GetServers(MasterServerRegion.All, MasterServerFilter.Gamedir("arma2arrowpc"));
            t.Wait(TimeSpan.FromSeconds(10));

            Assert.IsTrue(t.Result.Any(), "No servers were returned");
        }
    }
}
