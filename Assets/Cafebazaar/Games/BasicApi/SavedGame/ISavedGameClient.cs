using System.Threading.Tasks;

namespace CafeBazaar.Games.BasicApi.SavedGame
{
    public interface ISavedGameClient
    {
        void Clear();
        void DeleteKey(string key);
        bool HasKey(string key);

        Task<float> GetFloat(string key, float defaultValue);
        Task<float> GetFloat(string key);
        Task<int> GetInt(string key, int defaultValue);
        Task<int> GetInt(string key);
        Task<string> GetString(string key, string defaultValue);
        Task<string> GetString(string key);
        Task<bool> GetBool(string key, bool defaultValue);
        Task<bool> GetBool(string key);

        void SetFloat(string key, float value);
        void SetInt(string key, int value);
        void SetString(string key, string value);
        void SetBool(string key, bool value);

        bool IsSynced { get; }
    }
}