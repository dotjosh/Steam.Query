using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Steam.Query
{
    public class Server
    {
        public Server(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        public IPEndPoint EndPoint { get; private set; }

        public async Task<Dictionary<string, string>> GetServerRules()
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
                var parser = new ResponseParser(response.Buffer);
                parser.CurrentPosition += 7;
                var items = new Dictionary<string, string>();
                while (parser.BytesLeft)
                {
                    items.Add(parser.GetStringToTermination(), parser.GetStringToTermination());
                }
                return items;
            }
        }

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
                var parser = new ResponseParser(response.Buffer);
                parser.CurrentPosition += 5;//Header
                var result = new ServerInfoResult();
                result.Protocol = parser.GetByte();
                result.Name = parser.GetStringToTermination();
                result.Map = parser.GetStringToTermination();
                result.Folder = parser.GetStringToTermination();
                result.Game = parser.GetStringToTermination();
                result.ID = parser.GetShort();
                result.Players = parser.GetByte();
                result.MaxPlayers = parser.GetByte();
                result.Bots = parser.GetByte();
                result.ServerType = parser.GetStringOfByte();
                result.Environment = parser.GetStringOfByte();
                result.Visibility = parser.GetByte();
                result.VAC = parser.GetByte();
                return result;
            }
        }
    }
}