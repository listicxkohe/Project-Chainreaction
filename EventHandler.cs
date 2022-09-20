using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using UnityEngine.SceneManagement;

public class EventHandler : MonoBehaviourPunCallbacks
{

    private Vector3 lastGridClickedPOS;
    private GameObject clickedOBJ;

    public int selfballcount = 0;

    public int player1ballcount = 0;
    public int player2ballcount = 0;
    public int player3ballcount = 0;
    public int player4ballcount = 0;

    public bool initialized = false;

    public int aliveCount;
    public int winnerActorId;
    public bool gameEnded = false;

    public int turnCount = 1;

    public void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "game")
        {
            aliveCount = PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }


    public void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && gameObject.GetComponent<TurnManager>().myTurn && !gameObject.GetComponent<TurnManager>().meDead)
        {
                float rayLength = 100.0f;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, rayLength))
                {
                    if (hit.collider != null)
                    {
                        //Debug.Log();
                        lastGridClickedPOS = hit.collider.transform.position;
                        clickedOBJ = hit.collider.gameObject;

                        if (clickedOBJ.tag == "grid" && clickedOBJ.GetComponent<grid>().containsBall == false)
                        {                            
                            //Debug.Log("NO ball detected");
                            gameObject.GetComponent<BallManager>().CallSpawnRpc(clickedOBJ.GetComponent<grid>().x, clickedOBJ.GetComponent<grid>().z, gameObject.GetComponent<TurnManager>().currentActorTurn);

                            gameObject.GetComponent<TurnManager>().turnFinished();

                        }
                        else if (clickedOBJ.tag == "grid" && clickedOBJ.GetComponent<grid>().containsBall == true && clickedOBJ.GetComponent<grid>().ball.GetComponent<ball>().team == gameObject.GetComponent<TurnManager>().currentActorTurn)
                        {
                            //Debug.Log("ball detected");
                            gameObject.GetComponent<BallManager>().CallUpgradeRPC(clickedOBJ.GetComponent<grid>().x, clickedOBJ.GetComponent<grid>().z, gameObject.GetComponent<TurnManager>().currentActorTurn);
                            //Debug.Log("CallUpgradeBall called");

                            gameObject.GetComponent<TurnManager>().turnFinished();

                        }


                    }
                }

            if (!initialized)
            {
                turnCount++;
                if (turnCount >= gameObject.GetComponent<TurnManager>().playersCount+1)
                {
                    initialized = true;
                }
            }



        }


        //ball counter selfplayer
        selfballcount = countballs(gameObject.GetComponent<TurnManager>().currentActorTurn + "ball");

        if (initialized && PhotonNetwork.IsMasterClient && !isProcessing())
        {
            //ball counter all players
            player1ballcount = countballs(1 + "ball");//player 1
            if (player1ballcount == 0 && !gameObject.GetComponent<TurnManager>().player1Dead && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
            {
                gameObject.GetComponent<TurnManager>().CallAnnounceDeathRPC(1);
                aliveCount -= 1;
            }

            player2ballcount = countballs(2 + "ball");//player 2
            if (player2ballcount == 0 && !gameObject.GetComponent<TurnManager>().player2Dead && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                gameObject.GetComponent<TurnManager>().CallAnnounceDeathRPC(2);
                aliveCount -= 1;
            }

            player3ballcount = countballs(3 + "ball");//player 3
            if (player3ballcount == 0 && !gameObject.GetComponent<TurnManager>().player3Dead && PhotonNetwork.CurrentRoom.PlayerCount >= 3)
            {
                gameObject.GetComponent<TurnManager>().CallAnnounceDeathRPC(3);
                aliveCount -= 1;
            }

            player4ballcount = countballs(4 + "ball");//player 4
            if (player4ballcount == 0 && !gameObject.GetComponent<TurnManager>().player4Dead && PhotonNetwork.CurrentRoom.PlayerCount >= 4)
            {
                gameObject.GetComponent<TurnManager>().CallAnnounceDeathRPC(4);
                aliveCount -= 1;
            }


        }

        //checking if game is over
        if (initialized && PhotonNetwork.IsMasterClient && !gameEnded)
        {
            TurnManager turnManager = gameObject.GetComponent<TurnManager>();
            if(aliveCount == 1)
            {
                Debug.Log("game ended");
                winnerActorId = getWinner();
                Debug.Log(">>>WINNER:" + winnerActorId);
                turnManager.CallEndRoundRPC(winnerActorId);
                gameEnded = true;
            }

        }



    }

    public int countballs(string Name)
    {
        int counter = 0;

        var go = GameObject.FindGameObjectsWithTag(Name);

        for (var i = 0; i < go.Length; i++)
        {
            counter++;
        }

        return counter;
    }

    public int getWinner()
    {
        int[] foo = { player1ballcount, player2ballcount, player3ballcount, player4ballcount};

        int highestInt = foo.Max();

        if(highestInt == player1ballcount)
        {
            return 1;
        }
        else if(highestInt == player2ballcount)
        {
            return 2;
        }
        else if (highestInt == player3ballcount)
        {
            return 3;
        }
        else if (highestInt == player4ballcount)
        {
            return 4;
        }
        else
        {
            return 0;
        }


    }


    public bool isProcessing()
    {
        int counter = 0;

        var go = GameObject.FindGameObjectsWithTag("projectile");

        for (var i = 0; i < go.Length; i++)
        {
            counter++;
        }

        if(counter > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

        
    }


    public void FindWinner()
    {

    }


}
