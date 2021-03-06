﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayScript : MonoBehaviour
{
    [SerializeField]
    private GameObject playerRig;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadGame();
        }
    }
    public void LoadGame()
    { 
        Destroy(playerRig);
        SceneManager.LoadScene("PlayerTest2");
    }

    public void ReturnToMenu()
    {
        Data.RECURING_CHARACTER_VISITS = 0;
        Destroy(playerRig);
        SceneManager.LoadScene("Menu2");
    }
}
