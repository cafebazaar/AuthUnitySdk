using System;
using CafeBazaar.AuthAndStorage;
using CafeBazaar.Games.BasicApi;
using CafeBazaar.Games.BasicApi.SavedGame;
using CafeBazaar.Storage;
using UnityEngine;

namespace CafeBazaar.Games
{
    public class AndroidClient : IBazaarGamesClient
    {
        private readonly BazaarGamesClientConfiguration mConfiguration;
        private readonly ISavedGameClient mSavedGameClient;

        private volatile Player mUser = null;

        private bool isAuthenticated;
        internal AndroidClient(BazaarGamesClientConfiguration configuration)
        {
            this.mConfiguration = configuration;
            this.mSavedGameClient = new AndroidSavedGameClient();
        }


        public void Authenticate(bool silent, Action<BasicApi.SignInStatus> callback)
        {
            AuthAndStorageBridge.Instance.LOGIN_SignIn(silent,
                (result) =>
                {
                    isAuthenticated = result.Status == CoreSignInStatus.Success;
                    if (isAuthenticated)
                    {
                        mUser = new Player(result.AccountId, result.AccountId);
                        //afterSuccessFull authenticate
                        if (mConfiguration.EnableSavedGames)
                        {
                            //Inital Storage
                            InitStorage(storageResponse =>
                            {
                                if (callback != null)
                                    callback(SignInStatus.Success);
                            });
                        }
                        else
                        {
                            if (callback != null)
                                callback(SignInStatus.Success);
                        }
                    }
                    else
                    {
                        if (callback != null)
                            callback(SignInStatus.Failed);
                    }
                });
        }

        public void InitStorage(Action<InitStorageStatus> callback)
        {
            AuthAndStorageBridge.Instance.STORAGE_Init(
                (result) =>
                {
                    Debug.Log("Storage status: " + result.Status);
                    if (callback != null)
                        callback(result.Status);
                });
        }

        public string GetUserDisplayName()
        {
            if (mUser == null)
            {
                return null;
            }

            return mUser.userName;
        }

        public string GetUserId()
        {
            if (mUser == null)
            {
                return null;
            }

            return mUser.id;
        }


        public bool IsAuthenticated()
        {
            return isAuthenticated;
        }

        public ISavedGameClient GetSavedGameClient()
        {
            return mSavedGameClient;
        }
    }
}
