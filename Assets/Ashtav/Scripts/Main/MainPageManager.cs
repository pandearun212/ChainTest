using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class MainPageManager : MonoBehaviour
{
    [SerializeField] TextAsset languageJson;
    [SerializeField] TMP_Text playBtnText;
    [SerializeField] List<Image> languageHighlightImageList = new List<Image>();
    void Start()
    {
        string jsonData = languageJson.text;
        Language[] language = JsonHelper.FromJson<Language>(jsonData);
        for (int i = 0; i < language.Length; i++)
        {
            GameManager.Instance.languageDataList.Add(language[i]);
        }
        SetHighlight(GameManager.Instance.currentLanguageId);
    }
    public void OnLanguageChange(int id)
    {
        if (GameManager.Instance.currentLanguageId != id)
        {
            SetHighlight(id);
        }
    }
    public void OnPlayButtonClicked()
    {
        GameManager.Instance.LoadNewScene("Game");
    }
   
    public void SetHighlight(int id)
    {
        for (int i = 0; i < languageHighlightImageList.Count; i++)
        {
            if (i == id)
            {
                languageHighlightImageList[i].enabled = true;
            }
            else
            {
                languageHighlightImageList[i].enabled = false;
            }
        }
        playBtnText.text = GameManager.Instance.languageDataList[id].languageData.playBtnText.ToString();
        GameManager.Instance.SetNewLanguageId(id);
    }
}
