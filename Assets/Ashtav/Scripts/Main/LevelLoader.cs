using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public void LoadNextLevel(string levelName)
    {
        StartCoroutine(LoadLevel(levelName));
    }
    /*Loads level with delay to show animation.*/
    IEnumerator LoadLevel(string levelName)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(levelName);
    }
}
