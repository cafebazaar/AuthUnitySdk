using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSceneController : MonoBehaviour
{
    private static DemoSceneController instance;
    public static DemoSceneController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<DemoSceneController>();

            return instance;
        }
    }

    public GameObject mainPanel;
    public GameObject loginStoragePanel;
    
    void Awake()
    {
        instance = this;
    }
    
    public void SwitchToMainPanel()
    {
        mainPanel.SetActive(true);
        loginStoragePanel.SetActive(false);
    }
    public void SwitchToLoginStoragePanel()
    {
        mainPanel.SetActive(false);
        loginStoragePanel.SetActive(true);
    }
}
