﻿using CafeBazaar.AuthAndStorage;

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
        public float GetFloat(string key, float defaultValue)
        {
            if (float.TryParse(GetString(key, defaultValue.ToString()), out float res))
                return res;
            else
                return defaultValue;
        }
        public float GetFloat(string key)
        {
            return GetFloat(key, 0);
        }
        public int GetInt(string key, int defaultValue)
        {
            if (int.TryParse(GetString(key, defaultValue.ToString()), out int res))
                return res;
            else
                return defaultValue;
        }
        public int GetInt(string key)
        {
            return GetInt(key, 0);
        }
        public string GetString(string key, string defaultValue)
        {
            return AuthAndStorageBridge.Instance.Storage_GetKey(key, defaultValue);
        }
        public string GetString(string key)
        {
            return GetString(key, null);
        }
        public bool GetBool(string key, bool defaultValue)
        {
            if (bool.TryParse(GetString(key, defaultValue.ToString()), out bool res))
                return res;
            else
                return defaultValue;
        }
        public bool GetBool(string key)
        {
            return false;
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
