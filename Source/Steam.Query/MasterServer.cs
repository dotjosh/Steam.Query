using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Steam.Query
{
    public class MasterServer
    {
        private const string FIRST_AND_LAST_SERVER = "0.0.0.0:0";
        private const int HEADER_BYTES_LENGTH = 6;
        private readonly IPAddress _steamSteamIpAddress;
        private readonly int _steamSteamPort;

        public MasterServer()
            : this("hl2master.steampowered.com", 27011)
        {
        }

        public MasterServer(string hostname, int steamPort)
        {
            _steamSteamIpAddress = Dns.GetHostEntry(hostname).AddressList[0];
            _steamSteamPort = steamPort;
        }

        public async Task<IEnumerable<Server>> GetServers(
            MasterServerRegion region = MasterServerRegion.All,
            params MasterServerFilter[] masterServerFilters)
        {
            var servers = new List<Server>();

            using (var client = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                client.Connect(_steamSteamIpAddress, _steamSteamPort);

                string thisServer = null;
                while (thisServer != FIRST_AND_LAST_SERVER)
                {
                    var requestPacket = CreateRequestPacket(thisServer ?? FIRST_AND_LAST_SERVER, region, masterServerFilters);
                    await client.SendAsync(requestPacket, requestPacket.Length);
                    var response = await client.ReceiveAsync();
                    var responseData = response.Buffer.ToList();
                    for (int i = HEADER_BYTES_LENGTH; i < responseData.Count; i++)
                    {
                        var ip = string.Join(".", responseData.GetRange(i, 4).ToArray());
                        int port = responseData[i + 4] << 8 | responseData[i + 5];
                        thisServer = string.Format("{0}:{1}", ip, port);
                        if (thisServer != FIRST_AND_LAST_SERVER)
                        {
                            servers.Add(new Server(new IPEndPoint(IPAddress.Parse(ip), port)));
                        }
                        i += 5;
                    }
                }
            }

            return servers;
        }

        private static byte[] CreateRequestPacket(string ipAddress, MasterServerRegion region, IEnumerable<MasterServerFilter> filters)
        {
            var buffer = new List<byte> { 0x31, (byte)region };
            buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(ipAddress));
            buffer.Add(0x00);
            var filtersString = string.Join("", filters.Select(x => x.Key + "\\" + x.Value));
            buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(filtersString));
            return buffer.ToArray();
        }
    }
}