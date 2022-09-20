using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    public GameObject loadingScreen;
    public GameObject menuButtons;
    public TMP_Text loadingText;
    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;
    public GameObject roomScreen;
    public TMP_Text roomNameText;
    public GameObject errorScreen;
    public TMP_Text errorText;
    public GameObject roomBrowserScreen;
    public roomButtonScript theRoomButton;
    public TMP_Text playerNameLabel;
    public GameObject nameInputScreen;
    public TMP_InputField nameInput;
    public string LVLtoplayer;
    public GameObject startgameBTN;
    public GameObject roomTestBTN;

    private List<roomButtonScript> allRoomButtons = new List<roomButtonScript>();
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();
    private static bool hasSetNickName;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CloseMenus();

        loadingScreen.SetActive(true);
        //loadingText.text = "Connecting to Network....";

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        

#if UNITY_EDITOR
        roomTestBTN.SetActive(true);
#endif

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;



    }




    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby();

        //loadingText.text = "joining lobby...";

        PhotonNetwork.AutomaticallySyncScene = true;

    }

    public override void OnJoinedLobby()
    {

        CloseMenus();
        menuButtons.SetActive(true);

        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        if (!hasSetNickName)
        {
            CloseMenus();
            nameInputScreen.SetActive(true);
            if (PlayerPrefs.HasKey("playerName"))
            {
                nameInput.text = PlayerPrefs.GetString("playerName");
            }

        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }





    }


    void CloseMenus()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
    }

    public void openRoomCreateScreen()
    {
        CloseMenus();
        createRoomScreen.SetActive(true);
    }

    public void createRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 4;

            PhotonNetwork.CreateRoom(roomNameInput.text,options);

            CloseMenus();
            //loadingText.text = "creating room .....";
            loadingScreen.SetActive(true);

        }
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        roomScreen.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        listAllPlayers();

        if (PhotonNetwork.IsMasterClient)
        {
            startgameBTN.SetActive(true);
        }
        else
        {
            startgameBTN.SetActive(false);
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayerNames.Add(newPlayerLabel);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        listAllPlayers();
    }


    //roomstuff
    private void listAllPlayers()
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;
        for(int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayerNames.Add(newPlayerLabel);
        }

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "failed to create room: " + message;
        CloseMenus();
        errorScreen.SetActive(true);
    }

    public void closeErrorScreen()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "leaving room...";
        loadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void openRoomBrowser()
    {
        CloseMenus();
        roomBrowserScreen.SetActive(true);
    }

    public void closeRoomBrowser()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    //putkir rosh bair hoye gese eta lekhte giya
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(roomButtonScript rb in allRoomButtons)//destroy all room buttons in my list
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();

        theRoomButton.gameObject.SetActive(false);//original room button inactive set korlam

        for (int i= 0; i < roomList.Count; i++)//shob room er bhitor loop check
        {
            if(roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)//jodi room full na thake and room deleted na
            {
                roomButtonScript newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);//taile notun ekta button create kori
                newButton.SetButtonDetails(roomList[i]);//buttonscript e room details pass kori to set the button name
                newButton.gameObject.SetActive(true);//button ta ke active kori

                allRoomButtons.Add(newButton);//and finally button take list e add kori

            }
        }

    }

    public void JoinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);
        CloseMenus();
        //loadingText.text = "joining room";
        loadingScreen.SetActive(true);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void setNickName()
    {

        if (!string.IsNullOrEmpty(nameInput.text))
        {
            PlayerPrefs.SetString("playerName", nameInput.text);
            PhotonNetwork.NickName = nameInput.text;
            CloseMenus();
            menuButtons.gameObject.SetActive(true);
            hasSetNickName = true;
        }

    }


    public void startGame()
    {
        PhotonNetwork.LoadLevel(LVLtoplayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startgameBTN.SetActive(true);
        }
        else
        {
            startgameBTN.SetActive(false);
        }

    }


    public void CreateTestRoom()
    {
        RoomOptions testOptions = new RoomOptions();
        testOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom("test");
        CloseMenus();
        //loadingText.text = "Creating Testing Room";
        loadingScreen.SetActive(true);
    }



}
