using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileController : MonoBehaviour
{
    public Rigidbody RB;
    public float force = 0.01f;
    public Vector3 destroyPos;
    public string moveDir;

    public GameObject grid;
    public BallManager m;
    public int team;

    public void Start()
    {
        
    }

    public void Update()
    {
        m.processing = true;
        if (moveDir == "up" && this.gameObject.transform.position.z >= destroyPos.z)
        {
            changeBall();
            
            Destroy(this.gameObject);
        }
        if (moveDir == "down" && this.gameObject.transform.position.z <= destroyPos.z)
        {
            changeBall();
            
            Destroy(this.gameObject);
        }
        if (moveDir == "right" && this.gameObject.transform.position.x >= destroyPos.x)
        {
            changeBall();
            
            Destroy(this.gameObject);
        }
        if (moveDir == "left" && this.gameObject.transform.position.x <= destroyPos.x)
        {
            changeBall();
            
            Destroy(this.gameObject);
        }

    }


    public void Move(string direction)
    {
        moveDir = direction;
        switch (direction)
        {

            case "up":
                force = 5f;
                RB.AddForce(0, 0, force, ForceMode.Impulse);
                break;
            case "down":
                force = -5f;
                RB.AddForce(0, 0, force, ForceMode.Impulse);
                break;
            case "right":
                force = 5f;
                RB.AddForce(force, 0, 0 ,ForceMode.Impulse);
                break;
            case "left":
                force = -5f;
                RB.AddForce(force, 0, 0, ForceMode.Impulse);
                break;
        }
            

    }

    public void changeBall()
    {

        if (!(grid.gameObject.GetComponent<grid>().containsBall))
        {
            m._SpawnBall(grid.gameObject.GetComponent<grid>().x, grid.gameObject.GetComponent<grid>().z, team);
            
        }
        else if (grid.gameObject.GetComponent<grid>().containsBall)
        {
            m._upgradeBall(grid.gameObject.GetComponent<grid>().x, grid.gameObject.GetComponent<grid>().z, team);
        }
        
    }


}
