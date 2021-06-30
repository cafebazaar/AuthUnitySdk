using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CafeBazaar.Storage;
using CafeBazaar.Core;
using CafeBazaar.AuthAndStorage;
using CafeBazaar.Core.UI;
using System;

namespace CafeBazaar.AuthAndStorage
{
    public class AuthAndStorageBridge : MonoBehaviour
    {
        #region Singleton
        private static AuthAndStorageBridge pointer;
        public static AuthAndStorageBridge Instance
        {
            get
            {
                if (pointer == null)
                    pointer = FindObjectOfType<AuthAndStorageBridge>();

                if (pointer == null)
                {
                    GameObject cafebazaarObject = new GameObject("_CAFEBAZAAR_", typeof(AuthAndStorageBridge));
                    cafebazaarObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(cafebazaarObject);
                    pointer = cafebazaarObject.GetComponent<AuthAndStorageBridge>();
                }

                return pointer;
            }
        }
        #endregion

        #region Private Variables
        private AndroidJavaObject bazaarBridgePlugin;
        #endregion

        #region Unity Functions
        public void Awake()
        {
            BaseInit();
        }
        public void Start()
        {
#if !UNITY_EDITOR
            if (!init_storageLoop)
                StartCoroutine(IEProcess_SotrageLoop());
#endif
        }
        public void Update()
        {
#if UNITY_EDITOR
            if (!init_storageLoop)
                StartCoroutine(IEProcess_SotrageLoop());
#endif
        }
        #endregion

        #region Core functions
        private void BaseInit()
        {
            if (!Application.isEditor)
            {
#if UNITY_ANDROID
                using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.farsitel.bazaar.BazaarBridge"))
                    bazaarBridgePlugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
#endif
            }
        }
        
        #endregion
        #region LOGIN System
        public bool IsSignIn { get; private set; }
        private string AccountId;
        public void LOGIN_SignIn(bool SilentMode, Action<SignInResult> OnResult)
        {
            if (!Application.isEditor)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    CallbackHolder.Instance.RegisterCallBack("OnLoginSucceed", (x) => { OnResult((SignInResult)x); });
                    CallbackHolder.Instance.RegisterCallBack("OnLoginFailed", (x) => { OnResult((SignInResult)x); });

                    if (SilentMode)
                        bazaarBridgePlugin.Call("SilentSignIn");
                    else
                        bazaarBridgePlugin.Call("SignIn");
#endif
                }
            }
            else
            {
                if (!IsSignIn)
                {
                    AccountId = "TEST_MODE";

                    IsSignIn = true;

                }
                else
                {

                    if (OnResult != null)
                        OnResult(new SignInResult() { Status = CoreSignInStatus.Success, AccountId = AccountId });
                }
            }

        }

        public void OnLoginSucceed(string AccountId)
        {
            SignInResult result = new SignInResult();
            result.Status = CoreSignInStatus.Success;
            result.AccountId = AccountId;
            IsSignIn = true;
            CallbackHolder.Instance.RemoveCallback("OnLoginFailed");
            CallbackHolder.Instance.ExecuteCallBack("OnLoginSucceed", result);
        }
        public void OnLoginFailed()
        {
            SignInResult result = new SignInResult();
            result.Status = CoreSignInStatus.Failed;
            CallbackHolder.Instance.RemoveCallback("OnLoginSucceed");
            CallbackHolder.Instance.ExecuteCallBack("OnLoginFailed", result);
        }
        #endregion
        #region STORAGE System
        public bool StorageIsInit { get; private set; }
        public bool Storage_Is_Synced { get; private set; }
        public bool Storage_Is_Syncing { get; private set; }
        private void STORAGE_SetData(string Data, Action<SetStorageResult> OnResult)
        {
            if (!Application.isEditor)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    //Debug.Log("STORAGE_SetData : " + Data);
                    CallbackHolder.Instance.RegisterCallBack("OnSaveDataSucceed", (x) => { OnResult((SetStorageResult)x); });
                    CallbackHolder.Instance.RegisterCallBack("OnSaveDataFailed", (x) => { OnResult((SetStorageResult)x); });

                    bazaarBridgePlugin.Call("SaveData", Data);
#endif
                }
                else
                {
                    if (OnResult != null)
                    {
                        OnResult(new SetStorageResult() { Status = SetStorageStatus.Failed, Message = "CafeSDK work only on android !" });
                    }
                }
            }
            else
            {
                PlayerPrefs.SetString("cafesdk_storage_data", Data);
                PlayerPrefs.Save();

                if (OnResult != null)
                {
                    OnResult(new SetStorageResult() { Status = SetStorageStatus.Success });
                }
            }
        }
        public void STORAGE_Init(Action<InitStorageResult> OnResult)
        {
            if (!StorageIsInit)
            {
                if (!Application.isEditor)
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
#if UNITY_ANDROID
                        CallbackHolder.Instance.RegisterCallBack("OnGetDataSucceed", (x) => { OnResult((InitStorageResult)x); });
                        CallbackHolder.Instance.RegisterCallBack("OnGetDataFailed", (x) => { OnResult((InitStorageResult)x); });

                        bazaarBridgePlugin.Call("GetSavedData");
#endif
                    }
                    else
                    {
                        InitStorageResult result = new InitStorageResult();
                        result.Status = InitStorageStatus.Failed;
                        result.Message = "CafeSDK work only on android !";
                        if (OnResult != null)
                            OnResult(result);
                    }
                }
                else
                {
                    InitStorageResult result = new InitStorageResult();
                    result.Status = InitStorageStatus.Success;
                    Storage_LoadData(PlayerPrefs.GetString("cafesdk_storage_data", ""));
                    Storage_Is_Synced = true;
                    Storage_Is_Syncing = false;
                    StorageIsInit = true;
                    if (OnResult != null)
                        OnResult(result);
                }
            }
            else
            {
                InitStorageResult result = new InitStorageResult();
                result.Status = InitStorageStatus.Success;
                if (OnResult != null)
                    OnResult(result);
            }
        }
        public void OnGetDataSucceed(string data)
        {
            InitStorageResult initStorageResult = new InitStorageResult();
            initStorageResult.Status = InitStorageStatus.Success;
            Storage_LoadData(data);
            Storage_Is_Synced = true;
            Storage_Is_Syncing = false;
            StorageIsInit = true;
            lastChangeStorage = Time.unscaledTime;

            CallbackHolder.Instance.RemoveCallback("OnGetDataFailed");
            CallbackHolder.Instance.ExecuteCallBack("OnGetDataSucceed", initStorageResult);
        }
        public void OnGetDataFailed(string error)
        {
            InitStorageResult initStorageResult = new InitStorageResult();
            initStorageResult.Status = InitStorageStatus.Failed;
            initStorageResult.Message = error;
            CallbackHolder.Instance.RemoveCallback("OnGetDataSucceed");
            CallbackHolder.Instance.ExecuteCallBack("OnGetDataFailed", initStorageResult);
        }

        public void OnSaveDataSucceed(string data)
        {
            SetStorageResult setStorageResult = new SetStorageResult();
            setStorageResult.Status = SetStorageStatus.Success;
            //Debug.Log("OnSaveDataSucceed " + data);
            CallbackHolder.Instance.RemoveCallback("OnSaveDataFailed");
            CallbackHolder.Instance.ExecuteCallBack("OnSaveDataSucceed", setStorageResult);
        }
        public void OnSaveDataFailed(string error)
        {
            SetStorageResult setStorageResult = new SetStorageResult();
            setStorageResult.Status = SetStorageStatus.Failed;
            setStorageResult.Message = error;
            //Debug.Log("OnSaveDataFailed " + error);
            CallbackHolder.Instance.RemoveCallback("OnSaveDataSucceed");
            CallbackHolder.Instance.ExecuteCallBack("OnSaveDataFailed", setStorageResult);
        }

        private IEnumerator IEOnLoginToCafebazaarSuccessfull(Action<SignInResult> OnResult, SignInResult loginResult)
        {
            yield return true;
            yield return true;
            CafebazaarLoginUI.Instance.Show();
            yield return new WaitForSecondsRealtime(1.4f);

            if (OnResult != null)
                OnResult(loginResult);
        }

        private readonly Dictionary<string, string> storageKeyValue = new Dictionary<string, string>();
        private static bool init_storageLoop;
        private static float lastChangeStorage;

        private string Storage_CalculateSaveData()
        {
            JSONClass saveObject = new JSONClass();

            JSONArray jSONArray = new JSONArray();

            foreach (var i in storageKeyValue)
            {
                JSONClass jSONClass = new JSONClass();
                jSONClass["k"] = i.Key;
                jSONClass["v"] = i.Value;
                jSONArray.Add(jSONClass);
            }

            saveObject["keys"] = jSONArray;
            saveObject["utc"] = DateTime.UtcNow.ToString();
            return saveObject.ToString();
        }
        private void Storage_LoadData(string Data)
        {
            if (Data != "")
            {
                JSONClass json;
                try { json = JSONNode.Parse(Data).AsObject; } catch { json = new JSONClass(); }

                storageKeyValue.Clear();

                if (json["keys"] != null)
                    foreach (JSONClass i in json["keys"].AsArray)
                    {
                        storageKeyValue.Add(i["k"].Value, i["v"].Value);
                    }
            }
            else
            {
                storageKeyValue.Clear();
            }
        }
        private IEnumerator IEProcess_SotrageLoop()
        {
            init_storageLoop = true;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();

                if (!Storage_Is_Synced && StorageIsInit)
                {
                    float _lastChangeStorage = lastChangeStorage;
                    string data_to_save = Storage_CalculateSaveData();

                    int falg = -1;
                    STORAGE_SetData(data_to_save,
                        (result) =>
                        {
                            if (result.Status == SetStorageStatus.Success)
                            {
                                falg = 1;
                            }
                            else
                            {
                                if (result.Message == "CafeSDK work only on android !")
                                {
                                    falg = 10;
                                }
                                else
                                    falg = 0;
                            }

                        });

                    while (falg == -1)
                        yield return true;

                    if (falg == 10)
                    {
                        yield break;
                    }
                    else
                    if (falg == 1)
                    {
                        if (_lastChangeStorage == lastChangeStorage)
                        {
                            Storage_Is_Synced = true;
                            Storage_Is_Syncing = false;
                        }
                        else
                        {
                            Storage_Is_Synced = false;
                            Storage_Is_Syncing = true;
                        }
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(3);
                    }

                }
            }
        }

        private const string CafeStorageInitError = "Init Storage before Set Data key!  Call Storage.Init();";
        public void Storage_SetKey(string Key, string Value)
        {
            if (StorageIsInit)
            {
                if (storageKeyValue.ContainsKey(Key))
                {
                    if (storageKeyValue[Key] != Value)
                    {
                        Storage_Is_Syncing = true;
                        Storage_Is_Synced = false;
                        storageKeyValue[Key] = Value;
                        lastChangeStorage = Time.unscaledTime;
                    }
                }
                else
                {
                    storageKeyValue.Add(Key, Value);
                    Storage_Is_Syncing = true;
                    Storage_Is_Synced = false;
                    lastChangeStorage = Time.unscaledTime;
                }
            }
            else
            {
                Debug.LogError(CafeStorageInitError);
            }
        }
        public string Storage_GetKey(string Key, string defaultValue)
        {
            if (storageKeyValue.ContainsKey(Key))
                return storageKeyValue[Key];
            else
                return defaultValue;
        }
        public bool Storage_HasKey(string Key)
        {
            return storageKeyValue.ContainsKey(Key);
        }
        public void Storage_DeleteKey(string Key)
        {
            if (StorageIsInit)
            {
                if (storageKeyValue.ContainsKey(Key))
                {
                    storageKeyValue.Remove(Key);
                    Storage_Is_Syncing = true;
                    Storage_Is_Synced = false;
                    lastChangeStorage = Time.unscaledTime;
                }
            }
            else
            {
                Debug.LogError(CafeStorageInitError);
            }
        }
        public void Storage_Clear()
        {
            if (StorageIsInit)
            {
                storageKeyValue.Clear();
                Storage_Is_Syncing = true;
                Storage_Is_Synced = false;
                lastChangeStorage = Time.unscaledTime;
            }
            else
            {
                Debug.LogError(CafeStorageInitError);
            }
        }
        #endregion
    }

    public enum GetStorageStatus
    {
        Success,
        Failed,
    }
    public enum SetStorageStatus
    {
        Success,
        Failed,
    }
    public class SetStorageResult : CafeBaseResult
    {
        public SetStorageStatus Status { get; set; }
        
    }
    
    public class InitStorageResult : CafeBaseResult
    {
        public InitStorageStatus Status { get; set; }
    }
}