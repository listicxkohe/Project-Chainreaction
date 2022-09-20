using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{

    public static MatchManager instance;

    public bool gameStarted = false;

    private List<leaderboardPlayer> lboardPlayers = new List<leaderboardPlayer>();

    private void Awake()
    {
        instance = this;
    }



    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayer,
        UpdateStat,
        NextMatch
    }


    public enum GameState
    {
        Waiting,
        Playing,
        Edning
    }




    public int killsToWin = 3;

    public GameState state = GameState.Waiting;
    public float waitTimeAfterWaiting = 5f;

    public List<playerInfo> allPlayers = new List<playerInfo>();

    private int index;

    public bool RepeatMatch;


    void Start()
    {

        if (!PhotonNetwork.IsConnected)//if not connect then back to mainmenu
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);
        }

        state = GameState.Playing;


    }


    void Update()
    {
        ShowLeaderBoard();
        gameStarted = true;
    }

    public void OnEvent(EventData photonEvent)
    {

        if (photonEvent.Code < 200)
        {

            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            Debug.Log("recieved event " + theEvent);

            switch (theEvent)
            {
                case EventCodes.NewPlayer:

                    NewPlayerRecieve(data);

                    break;

                case EventCodes.ListPlayer:

                    ListPlayerRecieve(data);

                    break;

                case EventCodes.UpdateStat:

                    UpdateStatsRecieve(data);

                    break;

                case EventCodes.NextMatch:
                    NextMatchRecieve();

                    break;

            }

        }

    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }



    public void NewPlayerSend(string userName) //new player sends this event to master client
    {
        object[] package = new object[4];
        package[0] = userName;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );

    }

    public void NewPlayerRecieve(object[] dataRecieved) //new player event code recived in master client
    {
        //??
        playerInfo player = new playerInfo((string)dataRecieved[0], (int)dataRecieved[1], (int)dataRecieved[2], (int)dataRecieved[3]);
        allPlayers.Add(player);

        ListPlayerSend();
    }

    public void ListPlayerSend()
    {
        object[] package = new object[allPlayers.Count + 1];

        package[0] = state;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            object[] piece = new object[4];
            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].actor;
            piece[2] = allPlayers[i].kills;
            piece[3] = allPlayers[i].deaths;

            package[i + 1] = piece;
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );

    }

    public void ListPlayerRecieve(object[] dataRecieved)
    {
        allPlayers.Clear();

        state = (GameState)dataRecieved[0];

        for (int i = 1; i < dataRecieved.Length; i++)
        {
            object[] piece = (object[])dataRecieved[i];
            playerInfo player = new playerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3]
                );

            allPlayers.Add(player);

            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i - 1;
            }

        }
        StateCheck();
    }

    public void UpdateStatsSend(int actorSending, int stateToUpdate, int amountToChange)
    {

        object[] package = new object[] { actorSending, stateToUpdate, amountToChange };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdateStat,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );

    }

    void ShowLeaderBoard()
    {
        uiController.instance.leaderBoard.SetActive(true);

        foreach (leaderboardPlayer lp in lboardPlayers)
        {
            Destroy(lp.gameObject);
        }
        lboardPlayers.Clear();

        uiController.instance.leaderBoardPlayerDisplay.gameObject.SetActive(false);

        List<playerInfo> sorted = SortPlayers(allPlayers);

        foreach (playerInfo player in sorted)
        {
            leaderboardPlayer newPlayerDisplay = Instantiate(uiController.instance.leaderBoardPlayerDisplay, uiController.instance.leaderBoardPlayerDisplay.transform.parent);

            newPlayerDisplay.SetDetails(player.name);



            if (player.actor == gameObject.GetComponent<TurnManager>().currentActorTurn)
            {
                newPlayerDisplay.gameObject.GetComponent<Image>().color = Color.red;
            }

            

            if (player.actor == 1 && !gameObject.GetComponent<TurnManager>().player1Dead)
            {
                newPlayerDisplay.gameObject.SetActive(true);
            }
            else if(player.actor == 2 && !gameObject.GetComponent<TurnManager>().player2Dead)
            {
                newPlayerDisplay.gameObject.SetActive(true);
            }
            else if (player.actor == 3 && !gameObject.GetComponent<TurnManager>().player3Dead)
            {
                newPlayerDisplay.gameObject.SetActive(true);
            }
            else if (player.actor == 4 && !gameObject.GetComponent<TurnManager>().player4Dead)
            {
                newPlayerDisplay.gameObject.SetActive(true);
            }
            else
            {
                newPlayerDisplay.gameObject.SetActive(true);
            }

            lboardPlayers.Add(newPlayerDisplay);
        }


    }

    private List<playerInfo> SortPlayers(List<playerInfo> players)
    {
        List<playerInfo> sorted = new List<playerInfo>();

        while (sorted.Count < players.Count)
        {
            int highest = -1;
            playerInfo selectedPlayer = players[0];

            foreach (playerInfo player in players)
            {
                if (!sorted.Contains(player))
                {
                    if (player.kills > highest)
                    {
                        selectedPlayer = player;
                        highest = player.kills;
                    }
                }
            }

            sorted.Add(selectedPlayer);

        }


        return sorted;
    }




    public void UpdateStatsRecieve(object[] dataRecieved)
    {

        int actor = (int)dataRecieved[0];
        int statType = (int)dataRecieved[1];
        int amount = (int)dataRecieved[2];

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].actor == actor)
            {
                switch (statType)
                {
                    case 0: //kills
                        allPlayers[i].kills += amount;
                        Debug.Log("player " + allPlayers[i].name + ": kills " + allPlayers[i].kills);
                        break;
                    case 1: //deaths
                        allPlayers[i].deaths += amount;
                        Debug.Log("player " + allPlayers[i].name + ": death " + allPlayers[i].deaths);
                        break;
                }

                if (i == index)
                {
                    UpdateStatsDisplay();
                }

                if (uiController.instance.leaderBoard.activeInHierarchy)
                {
                    ShowLeaderBoard();
                }

                break;
            }
        }
        ScoreCheck();

    }

    public void UpdateStatsDisplay()
    {
        if (allPlayers.Count > index)
        {
            //uiController.instance.killsText.text = "kills; " + allPlayers[index].kills;
            //uiController.instance.deathsText.text = "deaths; " + allPlayers[index].deaths;
        }
        else
        {
            //uiController.instance.killsText.text = "kills; 0";
            //uiController.instance.deathsText.text = "deaths; 0";
        }

    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        SceneManager.LoadScene(0);

    }

    void ScoreCheck()//win condition
    {
        bool winnerFound = false;

        foreach (playerInfo player in allPlayers)
        {
            if (player.kills >= killsToWin && killsToWin > 0)
            {
                winnerFound = true;
                break;
            }
        }
        if (winnerFound)
        {
            if (PhotonNetwork.IsMasterClient && state != GameState.Edning)
            {
                state = GameState.Edning;
                ListPlayerSend();
            }
        }
    }


    void StateCheck()
    {
        if (state == GameState.Edning)
        {
            EndGame(0);
        }
    }

    public void EndGame(int actorID)
    {
        state = GameState.Edning;

        var player = PhotonNetwork.CurrentRoom.GetPlayer(actorID);
        string winnerName = player.NickName;
        Debug.Log("mathmanage winnername: " + winnerName + " actorid: " + actorID);
        uiController.instance.winnerText.GetComponent<TMP_Text>().text = winnerName;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }
        uiController.instance.endScreen.SetActive(true);
        ShowLeaderBoard();
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;


        StartCoroutine(EndCo());
    }


    private IEnumerator EndCo()
    {
        if (!RepeatMatch)
        {
            yield return new WaitForSeconds(waitTimeAfterWaiting);
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NextMatchSend();
            }
        }
    }

    public void NextMatchSend()
    {

        PhotonNetwork.RaiseEvent(
    (byte)EventCodes.NextMatch,
    null,
    new RaiseEventOptions { Receivers = ReceiverGroup.All },
    new SendOptions { Reliability = true }
    );

    }

    public void NextMatchRecieve()
    {

        state = GameState.Playing;
        uiController.instance.endScreen.SetActive(false);
        uiController.instance.leaderBoard.SetActive(false);

        foreach (playerInfo player in allPlayers)
        {
            player.kills = 0;
            player.deaths = 0;
        }
        UpdateStatsDisplay();

        //playerSpawner.instance.SpawnPlayer();

    }


    //turn stuff



    //turn stuff end





}



[System.Serializable]
public class playerInfo
{
    public string name;
    public int actor, kills, deaths;


    public playerInfo(string _name, int _actor, int _kills, int _deaths)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;

    }



}
