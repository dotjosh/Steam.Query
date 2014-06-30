namespace Steam.Query
{
    public class MasterServerFilter
    {
        public MasterServerFilter(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }

        /// <summary>
        /// Servers using anti-cheat technology (VAC, but potentially others as well)
        /// </summary>
        public static MasterServerFilter Secure()
        {
            return new MasterServerFilter("secure", "1");
        }

        /// <summary>
        /// Servers running dedicated
        /// </summary>
        public static MasterServerFilter Dedicated()
        {
            return new MasterServerFilter("type", "d");
        }

        /// <summary>
        /// Servers running the specified modification (ex. cstrike)
        /// </summary>
        public static MasterServerFilter Gamedir(string mod)
        {
            return new MasterServerFilter("gamedir", mod);
        }

        /// <summary>
        /// Servers running the specified map (ex. cs_italy)
        /// </summary>
        public static MasterServerFilter Map(string map)
        {
            return new MasterServerFilter("map", map);
        }

        /// <summary>
        /// Servers running on a Linux platform
        /// </summary>
        public static MasterServerFilter Linux()
        {
            return new MasterServerFilter("linux", "1");
        }

        /// <summary>
        /// Servers that are not empty
        /// </summary>
        public static MasterServerFilter Empty()
        {
            return new MasterServerFilter("empty", "1");
        }

        /// <summary>
        /// Servers that are not full
        /// </summary>
        public static MasterServerFilter Full()
        {
            return new MasterServerFilter("full", "1");
        }

        /// <summary>
        /// Servers that are spectator proxies
        /// </summary>
        public static MasterServerFilter Proxy()
        {
            return new MasterServerFilter("proxy", "1");
        }

        /// <summary>
        /// Servers that are NOT running game [appid] (This was introduced to block Left 4 Dead games from the Steam Server Browser)
        /// </summary>
        public static MasterServerFilter NotApp(string appId)
        {
            return new MasterServerFilter("napp", appId);
        }

        /// <summary>
        /// Servers that are empty
        /// </summary>
        public static MasterServerFilter NoPlayers()
        {
            return new MasterServerFilter("noplayers", "1");
        }

        /// <summary>
        /// Servers that are whitelisted
        /// </summary>
        public static MasterServerFilter White()
        {
            return new MasterServerFilter("white", "1");
        }
    }
}