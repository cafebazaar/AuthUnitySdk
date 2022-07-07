using UnityEngine;
using CafeBazaar;
using UnityEngine.UI;
using CafeBazaar.Games.BasicApi;
using CafeBazaar.Games;
using CafeBazaar.AuthAndStorage;

public class LoginStorageExample : MonoBehaviour
{
    [SerializeField] GameObject loginPanel, storagePanel;
    [SerializeField] Text storageStatusText, consoleText;
    void Start()
    {
        RefreshButtonEnableStatus();

        Log("CafeBazaar Plugin Version: " + AuthAndStorageBridge.Instance.GetVersion());
        var config = new BazaarGamesClientConfiguration.Builder().EnableSavedGames().Build();
        BazaarGamesPlatform.InitializeInstance(config);
        BazaarGamesPlatform.Activate();
    }

    void Update()
    {
        if (BazaarGamesPlatform.Instance.IsAuthenticated())
        {
            if (BazaarGamesPlatform.Instance.SavedGame.IsSynced)
            {
                storageStatusText.text = "Synced";
            }
            else
            {
                storageStatusText.text = "Syncing ...";
            }
        }
    }

    private void RefreshButtonEnableStatus()
    {
        var isAuthenticated = BazaarGamesPlatform.Instance.IsAuthenticated();
        loginPanel.SetActive(!isAuthenticated);
        storagePanel.SetActive(isAuthenticated);
    }

    public void LoginToBazaar(bool isSilent)
    {
        Log("Signing ... ");
        loginPanel.SetActive(false);
        BazaarGamesPlatform.Instance.Authenticate(isSilent, response =>
        {
            if (response)
                Log("SignedIn to bazaar AccountId : " + BazaarGamesPlatform.Instance.GetUserId());
            else
                Log("SignedIn error ");

            RefreshButtonEnableStatus();
        });
    }

    public void SetKey()
    {
        var data = Random.Range(0, 1000).ToString();
        BazaarGamesPlatform.Instance.SavedGame.SetString("Data1", data);

        Log("Bazaar Storage : Set Data1 -> " + data);
    }

    public void GetKey()
    {
        _GetKey();
    }

    private async void _GetKey()
    {
        var savedGameClient = BazaarGamesPlatform.Instance.SavedGame;
        string data = await savedGameClient.GetString("Data1");
        Log("Bazaar Storage > Data1 = " + data);
    }

    public void Log(string message)
    {
        consoleText.text += message + "\n";
    }
}
