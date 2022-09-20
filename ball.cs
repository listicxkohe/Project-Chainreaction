using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ball : MonoBehaviourPunCallbacks
{
    public int level = 0;
    public int team;
    public Color teamColour;
    public BallManager manager;

    public ParticleSystem explosionEffect;

    public GameObject level0;
    public GameObject level1;
    public GameObject level2;

    public Sprite ballSkin;


    public void Update()
    {


        switch (team)
        {
            case 1:
                this.gameObject.GetComponent<ball>().ballSkin = manager.player1Skin;
                break;
            case 2:
                this.gameObject.GetComponent<ball>().ballSkin = manager.player2Skin;
                break;
            case 3:
                this.gameObject.GetComponent<ball>().ballSkin = manager.player3Skin;
                break;
            case 4:
                this.gameObject.GetComponent<ball>().ballSkin = manager.player4Skin;
                break;
        }


        switch (level)
        {
            case 1:
                level0.SetActive(false);
                level1.SetActive(true);
                level2.SetActive(false);
                break;
            case 2:
                level0.SetActive(false);
                level1.SetActive(false);
                level2.SetActive(true);
                break;
            case 0:
                level0.SetActive(true);
                level1.SetActive(false);
                level2.SetActive(false);
                break;
        }


    }




    public void explode(Vector3 spawnPos, GameObject targetBall, GameObject grid1, GameObject grid2, GameObject grid3, GameObject grid4, GameObject projectile, GameObject managerREF)
    {//spawnPos = pos of the explosion, targetball = ref of the ball, adj grids to the ball, projectile prefab, gamemanager object reference
        BallManager m = managerREF.GetComponent<BallManager>();

        Instantiate(explosionEffect,gameObject.transform.position,Quaternion.identity);

        {
            if (grid1 != null)//ruight
            {
                causeExplosion(projectile, spawnPos, grid1, m, "right");
            }
            if (grid2 != null)//left
            {
                causeExplosion(projectile, spawnPos, grid2, m, "left");
            }
            if (grid3 != null)//up
            {
                causeExplosion(projectile, spawnPos, grid3, m, "up");
            }
            if (grid4 != null)//down
            {
                causeExplosion(projectile, spawnPos, grid4, m, "down");
            }
        }


        Destroy(targetBall);

    }



    public void causeExplosion(GameObject projectilePrefab, Vector3 spawnPos, GameObject grid, BallManager m, string direction)
    {
        GameObject explosion;


        explosion = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        explosion.GetComponent<projectileController>().destroyPos = grid.gameObject.transform.position;
        explosion.GetComponent<projectileController>().Move(direction);

        explosion.GetComponent<projectileController>().grid = grid;
        explosion.GetComponent<projectileController>().m = m;
        explosion.GetComponent<projectileController>().team = team;

    }

}
