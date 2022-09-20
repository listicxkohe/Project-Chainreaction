using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallManager : MonoBehaviourPunCallbacks
{
    public Vector3 lastBallPos;
    public GameObject ball0;
    public GameObject ball1;
    public GameObject ball2;
    public GameObject projectile;

    private GameObject ball;
    private GameObject grid;
    private Vector3 spwnPos;
    private GameObject newBall;

    public bool processing = true;

    public static BallManager instance;

    public Sprite player1Skin;
    public Sprite player2Skin;
    public Sprite player3Skin;
    public Sprite player4Skin;


    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        gameObject.GetComponent<timer>().Duration = 1f;
        gameObject.GetComponent<timer>().Run();
    }


    public void _SpawnBall(int x, int z, int player )
    {
        GameObject spawnedBall = null;
        grid = this.gameObject.GetComponent<GridSpawner>().Grid[x, z];
        spwnPos = grid.transform.position;
        spwnPos.y = 0.55f;


        if (grid.GetComponent<grid>().spawnLvl == 0)
        {
            spawnedBall = Instantiate(ball0, spwnPos, Quaternion.identity);
            //Debug.Log("spawned ball");
        }
        else if (grid.GetComponent<grid>().spawnLvl == 1)
        {
            spawnedBall = Instantiate(ball0, spwnPos, Quaternion.identity);
            //Debug.Log("spawned ball");
        }
        else if (grid.GetComponent<grid>().spawnLvl == 2)
        {
            spawnedBall = Instantiate(ball0, spwnPos, Quaternion.identity);
            //Debug.Log("spawned ball");
        }


        spawnedBall.GetComponent<ball>().team = player;
        spawnedBall.GetComponent<ball>().level = grid.GetComponent<grid>().spawnLvl;
        spawnedBall.tag = player + "ball";
        spawnedBall.GetComponent<ball>().manager = this.gameObject.GetComponent<BallManager>();

        grid.GetComponent<grid>().containsBall = true;
        grid.GetComponent<grid>().ball = spawnedBall;

        processing = false;
    }

    public void _upgradeBall(int x, int z, int actor)
    {

        grid = this.gameObject.GetComponent<GridSpawner>().Grid[x, z];
        ball = grid.gameObject.GetComponent<grid>().ball;
       

        lastBallPos = ball.transform.position;

        if (ball.gameObject.GetComponent<ball>().level == 0)
        {

            ball.GetComponent<ball>().team = actor;
            ball.tag = actor + "ball";
            ball.GetComponent<ball>().level += 1;

        }else if (ball.gameObject.GetComponent<ball>().level == 1)
        {

            ball.GetComponent<ball>().team = actor;
            ball.tag = actor + "ball";
            ball.GetComponent<ball>().level += 1;

        }else if(ball.gameObject.GetComponent<ball>().level == 2)//if more than lvl 2 explode, calling explode method on the target ball
        {
            Debug.Log("explode executed");

            grid.GetComponent<grid>().containsBall = false;
            grid.GetComponent<grid>().ball = null;
 
            GameObject[] grids = new GameObject[4];
            grids = findAdjacentGrids(x, z);

            ball.GetComponent<ball>().team = actor;
            ball.tag = actor + "ball";

            ball.gameObject.GetComponent<ball>().explode(ball.transform.position, ball, grids[0], grids[1], grids[2], grids[3], projectile, this.gameObject);

        }

        processing = false;

    }



    public void CallUpgradeRPC(int x, int z, int actor)
    {
        photonView.RPC("RPCupgradeBall", RpcTarget.All, x, z, actor);
    }

    public void CallSpawnRpc(int x, int z, int actor)
    {
        //Debug.Log("called spawn rpc");
        photonView.RPC("RPCspawnBall", RpcTarget.All, x, z, actor);
    }

    [PunRPC]
    public void RPCspawnBall(int x, int z, int actor)
    {
        _SpawnBall(x, z, actor);
    }

    [PunRPC]
    public void RPCupgradeBall(int x, int z, int actor)
    {
        _upgradeBall(x, z, actor);
    }


    public GameObject[] findAdjacentGrids(int x, int z) //passes the coordinates of the source grid returns array of reference to the adj grids
    {
        int limitX = gameObject.GetComponent<GridSpawner>().worldWidth -1;
        int limitZ = gameObject.GetComponent<GridSpawner>().worldHeight -1;

        GameObject[] gridList = new GameObject[4];

        grid = gameObject.GetComponent<GridSpawner>().Grid[x, z];

        if (!(x + 1 > limitX))
        {
            gridList[0] = gameObject.GetComponent<GridSpawner>().Grid[x + 1, z];
        }
        if(!(x - 1 < 0))
        {

            gridList[1] = gameObject.GetComponent<GridSpawner>().Grid[x - 1, z];
        }

        if (!(z + 1 > limitZ))
        {
            gridList[2] = gameObject.GetComponent<GridSpawner>().Grid[x, z + 1];

        }
        if(!(z - 1 < 0)) 
        {

            gridList[3] = gameObject.GetComponent<GridSpawner>().Grid[x, z - 1];
        }

        return gridList;
    }


}
