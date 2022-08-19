using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager _instance;
    public static ScoreManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    TMP_Text highScoreText;
    [SerializeField]
    TMP_Text currentScoreText;
    int score;
    int highScore;
    [SerializeField]
    GameObject gameOverPanel;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentScoreText.text = "0";
        score = 0;
        highScore = 0;

        if (PlayerPrefs.HasKey("highscore"))
        {
            highScore = PlayerPrefs.GetInt("highscore");
            highScoreText.text = "Highscore: " + highScore.ToString();
        }
    }

    public void UpdateScore(int num)
    {
        score += num;
        currentScoreText.text = score.ToString();
        if(score > highScore)
        {
            highScore = score;
            highScoreText.text = "HighScore: " + highScore.ToString();
            PlayerPrefs.SetInt("highscore", highScore);
            PlayerPrefs.Save();
        }
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
