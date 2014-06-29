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

        public static MasterServerFilter Gamedir(string value)
        {
            return new MasterServerFilter("gamedir", value);
        }
    }
}