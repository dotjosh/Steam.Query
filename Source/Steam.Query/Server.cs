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
        private readonly IPEndPoint _server;

        public Server(IPEndPoint server)
        {
            _server = server;
        }

        public async Task<Dictionary<string, string>> GetServerRules()
        {
            using (var client = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                client.Connect(_server);
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

                var items = new Dictionary<string, string>();
                for (int i = 7; i < response.Buffer.Length; i++)
                {
                    byte? c = null;
                    var nameBytes = new List<byte>();
                    var valueBytes = new List<byte>();
                    while (true)
                    {
                        c = response.Buffer[i];
                        if (c != 0x00)
                        {
                            nameBytes.Add((byte) c);
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i++;

                    var name = Encoding.ASCII.GetString(nameBytes.ToArray());
                    while (true)
                    {
                        c = response.Buffer[i];
                        if (c != 0x00)
                        {
                            valueBytes.Add((byte) c);
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    var value = Encoding.ASCII.GetString(valueBytes.ToArray());
                    items.Add(name, value);
                }
                return items;
            }
        }
    }
}