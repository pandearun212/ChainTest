using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GamePageManager : MonoBehaviour
{
    public TMP_Text highScoreText, currentScoreText;
    public GameObject settingsPanel;
    void Start()
    {
        GoogleAdMobManager.Instance.RequestBannerAd();
    }
    /*Settings button is clicked.*/
    public void OnSettingsButtonClicked()
    {
        settingsPanel.SetActive(true);
    }
    /*Resume Button is clicked.*/
    public void OnResumeButtonClicked()
    {
        settingsPanel.SetActive(false);
    }
    /*Home Button is clicked.*/
    public void OnHomeButtonClicked()
    {
        GameManager.Instance.LoadNewScene("Main");
    }
    /*Restart Button is clicked.*/
    public void OnRestartButtonClicked()
    {
        GameManager.Instance.LoadNewScene("Game");
    }    
}
