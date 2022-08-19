using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }
    public List<Language> languageDataList = new List<Language>();

    public int currentLanguageId;
    public int collisionCount;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        if (CheckPlayerPrefsKey("currentLanguageId"))
        {
            currentLanguageId = PlayerPrefs.GetInt("currentLanguageId");
        }
        else
        {
            SetNewLanguageId(0);
        }
    }
    void Start()
    {
        collisionCount = 0;
    }
    public bool CheckPlayerPrefsKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    public void SetNewLanguageId(int id)
    {
        currentLanguageId = id;
        PlayerPrefs.SetInt("currentLanguageId", id);
        PlayerPrefs.Save();
    }
    /*Shows interstitial ad after 10 cube shots.*/
    public void CheckForInterstitialAds()
    {
        if (collisionCount == 10)
        {
            collisionCount = 0;
            GoogleAdMobManager.Instance.DestroyBannerAd();
            GoogleAdMobManager.Instance.ShowInterstitialAd();
        }
    }
    public void LoadNewScene(string sceneName)
    {
      GameObject.Find("LevelLoader").GetComponent<LevelLoader>().LoadNextLevel(sceneName);
    }
}
