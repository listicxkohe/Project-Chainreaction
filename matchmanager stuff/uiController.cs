using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class uiController : MonoBehaviour
{
    public static uiController instance;

    public GameObject deathScreen;

    public GameObject leaderBoard;
    public leaderboardPlayer leaderBoardPlayerDisplay;

    public GameObject endScreen;

    public GameObject optionsScreen;

    public GameObject winnerText;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OptionsToggle();
        }

    }


    public void OptionsToggle()
    {
        if (!optionsScreen.activeInHierarchy)
        {
            optionsScreen.SetActive(true);
        }
        else
        {
            optionsScreen.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
