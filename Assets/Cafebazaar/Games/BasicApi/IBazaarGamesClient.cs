using System;
using CafeBazaar.Storage;

namespace CafeBazaar.Games.BasicApi
{
    public interface IBazaarGamesClient 
    {
        void Authenticate(bool silent, Action<SignInStatus> callback);
        void InitStorage(Action<InitStorageStatus> callback);
        bool IsAuthenticated();
        string GetUserDisplayName();
        string GetUserId();

        SavedGame.ISavedGameClient GetSavedGameClient();
    }
}
