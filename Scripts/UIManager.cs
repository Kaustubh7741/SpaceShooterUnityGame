using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //Library for UI datatypes like Text

public class UIManager : MonoBehaviour
{

    //[SerializeField]
    private Text _scoreText;    //handle to score text UI
    private Text _gameOverText;

    [SerializeField]
    private Sprite[] _playerLivesSprites;   //Sprites/Images for player lives
    private Image _livesImg;    //Lives image in canvas

    private GameManager _gameManager;       //handle to game manager which handles scene management on game over

    //private GameObject _gamePausedPanel;        //handle for toggling game paused screen
    
    // Start is called before the first frame update
    void Start()
    {
        _scoreText = transform.Find("Score_text").GetComponent<Text>();
        if (_scoreText != null)
            _scoreText.text = "Score: 0";       //Initialize score display
        else
            Debug.LogError("Score text was not initialized");

        _livesImg = transform.Find("Player_Lives_img").GetComponent<Image>();
        if (_livesImg != null)
            _livesImg.sprite = _playerLivesSprites[_playerLivesSprites.Length-1];       //display max lives
        else
            Debug.LogError("Lives image was not initialized");

        _gameOverText = transform.Find("Game_Over_text").GetComponent<Text>();
        if (_gameOverText != null)
            _gameOverText.gameObject.SetActive(false);
        else
            Debug.LogError("Game over text was not initialized");

        _gameManager = GameObject.FindGameObjectWithTag("Game_Manager").GetComponent<GameManager>();
        if(_gameManager == null)
            Debug.LogError("Game manager was not initialized");

        /*_gamePausedPanel = transform.Find("Game_Paused_panel").gameObject;
        if (_gamePausedPanel == null)
            Debug.LogError("Game paused screen/panel was not initialized");*/
    }

    public void UpdateScoreOnScreen(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void DisplayAccuracyOnScreen(float playerAccuracy)
    {
        _scoreText.text = _scoreText.text + "\nAccuracy: " + playerAccuracy.ToString("0.00") + "%";     //ToString will consider only 2 values after decimal point but all values before it
    }

    public void CheckHighScore(int sessionScore, float sessionAccuracy)
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        float bestAccuracy = PlayerPrefs.GetFloat("BestAccuracy", 0f);

        if((highScore * bestAccuracy) < (sessionScore * sessionAccuracy))
        {
            PlayerPrefs.SetInt("HighScore", sessionScore);
            PlayerPrefs.SetFloat("BestAccuracy", sessionAccuracy);
            _scoreText.text += "\nNew High Score!";
        }
        //break ties in favour of score (could have or-ed with prev condition)
        else if (((highScore * bestAccuracy) == (sessionScore * sessionAccuracy)) && highScore < sessionScore)
        {
            PlayerPrefs.SetInt("HighScore", sessionScore);
            PlayerPrefs.SetFloat("BestAccuracy", sessionAccuracy);
            _scoreText.text += "\nNew High Score!";
        }
    }

    public void UpdateLivesOnScreen(int currentLives)
    {
        if(currentLives < 0 || currentLives >= _playerLivesSprites.Length)
        {
            //Debug.LogError("Current lives are beyond the range of available sprites");
            _livesImg.sprite = _playerLivesSprites[0];      //Replace with zero lives if IndexOutOfBounds
            return;
        }

        _livesImg.sprite = _playerLivesSprites[currentLives];       //change current image
    }

    public void InitiateGameOverSequence()
    {
        //Replace game over text for Android
#if UNITY_ANDROID
        _gameOverText.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Press 'Fire' to restart the game or 'Back' to go back to main menu";
#endif
        _gameManager.GameOver();
        StartCoroutine(FlickerGameOverText());
    }
    IEnumerator FlickerGameOverText()       //Flicker (switch on and off) Game Over text
    {
        while (true)        //user selects/inputs new game
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /*public void TogglePause(bool isPaused)
    {
        
        //Time.timeScale = Convert.ToInt32(isPaused);
        if (isPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;

        //Enable UI and stop time OR Disable UI and resume time
        _gamePausedPanel.SetActive(isPaused);
    }*/
}
