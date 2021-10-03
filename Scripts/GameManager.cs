using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;      //Library for scene manager

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;

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
            SceneManager.LoadScene(1);      //Load current game scene (better than String use)
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
}
