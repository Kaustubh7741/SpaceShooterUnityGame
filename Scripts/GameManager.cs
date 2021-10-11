using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;      //Library for scene manager

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;

    [SerializeField]
    private bool _isCoop = false;       //If coop scene loaded or not

    private bool _isGamePaused = false;     //variable to toggle pausing
    //private GameObject _gamePausedPanel;        //handle for toggling game paused screen
    private Animator _gamePausedPanelAnimation;        //handle for toggling game paused screen animation

    void Start()
    {
        /*_gamePausedPanel = GameObject.Find("Canvas").transform.Find("Game_Paused_panel").gameObject;
        if (_gamePausedPanel == null)
            Debug.LogError("Game paused screen/panel was not initialized");*/

        _gamePausedPanelAnimation = GameObject.Find("Canvas").transform.Find("Game_Paused_panel").GetComponent<Animator>();
        if (_gamePausedPanelAnimation == null)
            Debug.LogError("Game paused screen/panel animator was not initialized");
        _gamePausedPanelAnimation.updateMode = AnimatorUpdateMode.UnscaledTime;     //will work irrespective of Time.timeScale
    }

    // Update is called once per frame
    void Update()
    {
        //Before this go to File > Build Settings -> Add Open Scenes. After adding note its index
        if(_isGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);      //Load main menu scene
        }

        if (_isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);      //Load current game scene (better than String use)
        }

        //if escape is pressed, quit game
        if (Input.GetKeyDown(KeyCode.Escape) && !_isGameOver)
        {
            //Load main menu scene
            //SceneManager.LoadScene(0);

            //Pause or resume game
            _isGamePaused = !_isGamePaused;
            TogglePause(_isGamePaused);
        }
    }


public void GameOver()
    {
        _isGameOver = true;
    }

    public bool IsCoopMode()
    {
        return _isCoop;
    }

    public void TogglePause(bool isPaused)
    {

        //Time.timeScale = Convert.ToInt32(isPaused);
        if (isPaused)
        {
            Time.timeScale = 0;
            _gamePausedPanelAnimation.SetBool("isPaused", true);
        }
        else
        {
            Time.timeScale = 1;
            _gamePausedPanelAnimation.SetBool("isPaused", false);
        }

        //Enable UI and stop time OR Disable UI and resume time
        //_gamePausedPanel.SetActive(isPaused);
    }
}
