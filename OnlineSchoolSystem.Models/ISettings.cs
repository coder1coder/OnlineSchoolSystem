namespace OnlineSchoolSystem.Models
{
    public interface ISettings
    {
        string Get(string key);
        void Set(string key, string value);
    }
}