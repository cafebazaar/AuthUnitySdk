using System.Threading.Tasks;
using CafeBazaar.AuthAndStorage;

namespace CafeBazaar.Games.BasicApi.SavedGame
{
    public class AndroidSavedGameClient : ISavedGameClient
    {
        public bool IsSynced { get { return AuthAndStorageBridge.Instance.Storage_Is_Synced; } }
        public void Clear()
        {
            AuthAndStorageBridge.Instance.Storage_Clear();
        }
        public void DeleteKey(string key)
        {
            AuthAndStorageBridge.Instance.Storage_DeleteKey(key);
        }
        public bool HasKey(string key)
        {
            return AuthAndStorageBridge.Instance.Storage_HasKey(key);
        }
        public async Task<float> GetFloat(string key, float defaultValue)
        {
            if (float.TryParse(await GetString(key, defaultValue.ToString()), out float res))
                return res;
            else
                return defaultValue;
        }
        public async Task<float> GetFloat(string key)
        {
            return await GetFloat(key, 0);
        }
        public async Task<int> GetInt(string key, int defaultValue)
        {
            if (int.TryParse(await GetString(key, defaultValue.ToString()), out int res))
                return res;
            else
                return defaultValue;
        }
        public async Task<int> GetInt(string key)
        {
            return await GetInt(key, 0);
        }
        public async Task<string> GetString(string key, string defaultValue)
        {
            return await AuthAndStorageBridge.Instance.Storage_GetKey(key, defaultValue);
        }
        public async Task<string> GetString(string key)
        {
            return await GetString(key, null);
        }
        public async Task<bool> GetBool(string key, bool defaultValue)
        {
            if (bool.TryParse(await GetString(key, defaultValue.ToString()), out bool res))
                return res;
            else
                return defaultValue;
        }
        public async Task<bool> GetBool(string key)
        {
            return await GetBool(key, false);
        }

        public void SetFloat(string key, float value)
        {
            SetString(key, value.ToString());
        }
        public void SetInt(string key, int value)
        {
            SetString(key, value.ToString());
        }
        public void SetString(string key, string value)
        {
            AuthAndStorageBridge.Instance.Storage_SetKey(key, value);
        }
        public void SetBool(string key, bool value)
        {
            SetString(key, value.ToString());
        }

    }
}
