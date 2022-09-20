using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public int myActor;
    public int currentActorTurn = 1;
    public bool myTurn =  false;
    public int playersCount;
    

    private int turnIndex = 0;

    public bool meDead = false;
    public bool player1Dead = false;
    public bool player2Dead = false;
    public bool player3Dead = false;
    public bool player4Dead = false;

    public GameObject timerText;
    public float maxTime = 20f;

    private timer timer;

 

    public void Start()
    {
        timer = gameObject.GetComponent<timer>();

        timer.Duration = maxTime;

        timer.Run();

        myActor = PhotonNetwork.LocalPlayer.ActorNumber;
    }



    public void Update()
    {
        selfCheck();


        timerText.GetComponent<TMPro.TextMeshProUGUI>().SetText(((int)timer.elapsedSeconds).ToString());

        if (timer.elapsedSeconds >= maxTime)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPCsendTurnUpdates", RpcTarget.All);
            }         
        }

        playersCount = gameObject.GetComponent<MatchManager>().allPlayers.Count;

        if (currentActorTurn == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            myTurn = true;
            if (meDead)
            {
                uiController.instance.deathScreen.SetActive(true);
                photonView.RPC("RPCsendTurnUpdates", RpcTarget.All);
            }
        }
    }


    public void turnFinished()
    {
        myTurn = false;
        photonView.RPC("RPCsendTurnUpdates", RpcTarget.All);

    }


    [PunRPC]
    public void RPCsendTurnUpdates()
    {
        turnIndex++;              
        if (turnIndex == gameObject.GetComponent<MatchManager>().allPlayers.Count)
        {
            turnIndex = 0;
        }
        currentActorTurn = gameObject.GetComponent<MatchManager>().allPlayers[turnIndex].actor;
        Debug.Log("turn rpc sent");
        Debug.Log("turn: " + gameObject.GetComponent<MatchManager>().allPlayers[turnIndex].name );

        timer.running = false;

        timer.Duration = maxTime;

        timer.Run();
    }

    public void CallAnnounceDeathRPC(int actor)
    {
        photonView.RPC("RPCAnnounceDeath", RpcTarget.All, actor);
    }


    [PunRPC]
    public void RPCAnnounceDeath(int actor)
    {
        if(actor == 1)
        {
            player1Dead = true;
        }
        if (actor == 2)
        {
            player2Dead = true;
        }
        if (actor == 3)
        {
            player3Dead = true;
        }
        if (actor == 4)
        {
            player4Dead = true;
        }


    }

    public void selfCheck()
    {
        if(myActor == 1 && player1Dead)
        {
            meDead = true;
        }
        else if(myActor == 2 && player2Dead)
        {
            meDead = true;
        }
        else if (myActor == 3 && player3Dead)
        {
            meDead = true;
        }
        else if (myActor == 4 && player4Dead)
        {
            meDead = true;
        }
    }

    public void CallEndRoundRPC(int winner)
    {
        photonView.RPC("EndRoundRPC", RpcTarget.All, winner);
    }

    [PunRPC]
    public void EndRoundRPC(int winner)
    {
        gameObject.GetComponent<MatchManager>().EndGame(winner);
    }

}
