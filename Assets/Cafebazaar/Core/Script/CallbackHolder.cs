using System;
using System.Collections.Generic;
using UnityEngine;

namespace CafeBazaar.Core
{
    public class CallbackHolder
    {

        private CallbackHolder() { }

        private readonly Dictionary<string, List<Action<CafeBaseResult>>> callBackResult = new Dictionary<string, List<Action<CafeBaseResult>>>();

        #region Singleton
        private static CallbackHolder _instance;
        public static CallbackHolder Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log("New instance of CallbackHolder created.");
                    _instance = new CallbackHolder();
                }
                return _instance;
            }
        }
        #endregion

        public void ExecuteCallBack(string MethodName, CafeBaseResult cafeBaseResult)
        {
            if (callBackResult.TryGetValue(MethodName, out List<Action<CafeBaseResult>> list))
            {
                callBackResult.Remove(MethodName);

                foreach (Action<CafeBaseResult> action in list)
                {
                    try
                    {
                        if (action != null)
                            action(cafeBaseResult);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }
        public void RegisterCallBack(string MethodName, Action<CafeBaseResult> callBackAction)
        {
            if (callBackAction != null)
            {
                if (!callBackResult.TryGetValue(MethodName, out List<Action<CafeBaseResult>> list))
                {
                    list = new List<Action<CafeBaseResult>>();
                    callBackResult.Add(MethodName, list);
                }

                list.Add(callBackAction);
            }
        }
        public void RemoveCallback(string MethodName)
        {
            callBackResult.Remove(MethodName);
        }
    }
}