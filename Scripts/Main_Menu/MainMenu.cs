using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;    //library for scene manager
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Text _highScoreText, _coopDisabledText;

    private void Start()
    {
        
        //if current scene is main menu
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            _highScoreText = transform.Find("High_Score_text").GetComponent<Text>();
            LoadHighScore();

            _coopDisabledText = transform.Find("Menu").transform.Find("Temp_text").GetComponent<Text>();
        }
    }

    public void LoadHighScore()
    {
        _highScoreText.text
                = "Your High Score"
                + "\nScore: " + PlayerPrefs.GetInt("HighScore", 0)
                + "\nAccuracy: " + PlayerPrefs.GetFloat("BestAccuracy", 0f).ToString("0.00") + "%";
    }

    public void ResetHighScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
        PlayerPrefs.SetFloat("BestAccuracy", 0f);
        LoadHighScore();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;        //when returning from paused game menu
        SceneManager.LoadScene(0);
    }

    public void LoadSinglePlayerGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadCoopGame()
    {
#if UNITY_STANDALONE
        SceneManager.LoadScene(3);
#elif UNITY_ANDROID || UNITY_IOS
        StartCoroutine(PromptText());
        return;
#endif
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator PromptText()
    {
        _coopDisabledText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        _coopDisabledText.gameObject.SetActive(false);
    }
}
