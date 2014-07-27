namespace Steam.Query
{
    public class ServerInfoResult
    {
        public int ID;
        public byte VAC;
        public int Protocol { get; set; }
        public string Name { get; set; }
        public string Map { get; set; }
        public string Folder { get; set; }
        public string Game { get; set; }
        public byte Players { get; set; }
        public byte MaxPlayers { get; set; }
        public byte Bots { get; set; }
        public string ServerType { get; set; }
        public string Environment { get; set; }
        public byte Visibility { get; set; }
    }
}