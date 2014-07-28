using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Steam.Query
{
    public partial class Server
    {
        public Server(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        public IPEndPoint EndPoint { get; private set; }

#if NET45
        public async Task<ServerRulesResult> GetServerRules()
        {
            using (var client = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                client.Connect(EndPoint);
                var requestPacket = new List<byte>();
                requestPacket.AddRange(new Byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0x56});
                requestPacket.AddRange(BitConverter.GetBytes(-1));
                await client.SendAsync(requestPacket.ToArray(), requestPacket.ToArray().Length);
                UdpReceiveResult response = await client.ReceiveAsync();
                List<byte> responseData = response.Buffer.ToList();
                requestPacket.Clear();
                requestPacket.AddRange(new Byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0x56});
                requestPacket.AddRange(responseData.GetRange(5, 4));
                await client.SendAsync(requestPacket.ToArray(), requestPacket.ToArray().Length);
                response = await client.ReceiveAsync();
                return ServerRulesResult.Parse(response.Buffer);
            }
        }
#endif

        public ServerRulesResult GetServerRulesSync(GetServerInfoSettings settings)
        {
            var localEndpoint = new IPEndPoint(IPAddress.Any, 0);
            using (var client = new UdpClient(localEndpoint))
            {
                client.Client.ReceiveTimeout = settings.ReceiveTimeout;
                client.Client.SendTimeout = settings.SendTimeout;

                client.Connect(EndPoint);
                var requestPacket = new List<byte>();
                requestPacket.AddRange(new Byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0x56});
                requestPacket.AddRange(BitConverter.GetBytes(-1));
                client.Send(requestPacket.ToArray(), requestPacket.ToArray().Length);
                byte[] responseData = client.Receive(ref localEndpoint);
                requestPacket.Clear();
                requestPacket.AddRange(new Byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x56 });
                requestPacket.AddRange(responseData.Skip(6).Take(4));
                client.Send(requestPacket.ToArray(), requestPacket.ToArray().Length);
                responseData = client.Receive(ref localEndpoint);
                return ServerRulesResult.Parse(responseData);
            }
        }

#if NET45
        public async Task<ServerInfoResult> GetServerInfo()
        {
            using (var client = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                client.Connect(EndPoint);
                var requestPacket = new List<byte>();
                requestPacket.AddRange(new Byte[] { 0xFF, 0xFF, 0xFF, 0xFF});
                requestPacket.Add(0x54);
                requestPacket.AddRange(Encoding.ASCII.GetBytes("Source Engine Query"));
                requestPacket.Add(0x00);
                await client.SendAsync(requestPacket.ToArray(), requestPacket.ToArray().Length);
                UdpReceiveResult response = await client.ReceiveAsync();
                return ServerInfoResult.Parse(response.Buffer);
            }
        }
#endif

        public ServerInfoResult GetServerInfoSync(GetServerInfoSettings settings)
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, 0);
            using (var client = new UdpClient(localEndPoint))
            {
                client.Client.ReceiveTimeout = settings.ReceiveTimeout;
                client.Client.SendTimeout = settings.SendTimeout;

                client.Connect(EndPoint);
                var requestPacket = new List<byte>();
                requestPacket.AddRange(new Byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                requestPacket.Add(0x54);
                requestPacket.AddRange(Encoding.ASCII.GetBytes("Source Engine Query"));
                requestPacket.Add(0x00);
                var requestData = requestPacket.ToArray();
                client.Send(requestData, requestData.Length);
                byte[] data = client.Receive(ref localEndPoint);
                return ServerInfoResult.Parse(data);
            }
        }

        public class GetServerInfoSettings
        {
            public int SendTimeout = 2000;
            public int ReceiveTimeout = 2000;
        }
    }
}