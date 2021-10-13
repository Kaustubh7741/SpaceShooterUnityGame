using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;      //Library for scene manager
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;

    [SerializeField]
    private bool _isCoop = false;       //If coop scene loaded or not

    private bool _isGamePaused = false;     //variable to toggle pausing
    //private GameObject _gamePausedPanel;        //handle for toggling game paused screen
    private Animator _gamePausedPanelAnimation;        //handle for toggling game paused screen animation

    //Structure to store platform user inputs
    private struct UserInputs
    {
        public bool escape;
        public bool restart;
    };
    UserInputs _userInputs;


    void Start()
    {
        /*_gamePausedPanel = GameObject.Find("Canvas").transform.Find("Game_Paused_panel").gameObject;
        if (_gamePausedPanel == null)
            Debug.LogError("Game paused screen/panel was not initialized");*/

        _gamePausedPanelAnimation = GameObject.Find("Canvas").transform.Find("Game_Paused_panel").GetComponent<Animator>();
        if (_gamePausedPanelAnimation == null)
            Debug.LogError("Game paused screen/panel animator was not initialized");
        _gamePausedPanelAnimation.updateMode = AnimatorUpdateMode.UnscaledTime;     //will work irrespective of Time.timeScale

        _userInputs = new UserInputs
        {
            escape = false,
            restart = false
        };


    }

    // Update is called once per frame
    void Update()
    {
        //store user inputs to avoid code repetition
#if UNITY_STANDALONE
        _userInputs.escape = Input.GetKeyDown(KeyCode.Escape);
        _userInputs.restart = Input.GetKeyDown(KeyCode.R);
#elif UNITY_ANDROID || UNITY_IOS
        _userInputs.escape = Input.GetKeyDown(KeyCode.Escape);  //its the same for Android
        _userInputs.restart = CrossPlatformInputManager.GetButtonUp("Fire");
#endif

        //Before this go to File > Build Settings -> Add Open Scenes. After adding note its index
        if (_isGameOver)
        {
            //Replace game over text for Android
            if (_isGameOver && _userInputs.escape)
            {
                SceneManager.LoadScene(0);      //Load main menu scene
            }

            if (_isGameOver && _userInputs.restart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);      //Load current game scene (better than String use)
            }
        }

        else
        {
            //if escape is pressed, pause game
            if (_userInputs.escape)// && !_isGameOver)
            {
                //Load main menu scene
                //SceneManager.LoadScene(0);

                //Pause or resume game
                _isGamePaused = !_isGamePaused;
                TogglePause(_isGamePaused);
            }
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

    public bool GamePaused()
    {
        return _isGamePaused;
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
