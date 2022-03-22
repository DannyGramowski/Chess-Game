namespace SaveSystem
{
    public interface ISaveRaw
    {
        void SaveRawString(string key, string value);
        string LoadRawString(string key);

        void SaveRawBytes(string key, byte[] value);
        byte[] LoadRawBytes(string key);
    }
}
