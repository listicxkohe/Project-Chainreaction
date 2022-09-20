using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GridSpawner : MonoBehaviour
{

    public GameObject block1;

    public int worldWidth = 10;
    public int worldHeight = 10;

    public GameObject[,] Grid;
    

    public float spawnSpeed = 0;

    void Start()
    {
        StartCoroutine(CreateWorld());
    }

    IEnumerator CreateWorld()
    {
        Grid = new GameObject[worldWidth, worldHeight];

        for (int x = 0; x < worldWidth; x++)
        {
            yield return new WaitForSeconds(spawnSpeed);

            for (int z = 0; z < worldHeight; z++)
            {
                yield return new WaitForSeconds(spawnSpeed);

                GameObject block = Instantiate(block1, Vector3.zero, block1.transform.rotation) as GameObject;
                block.transform.parent = transform;
                block.transform.localPosition = new Vector3(x, 0, z);
                Grid[x, z] = block;
                block.GetComponent<grid>().x = x;
                block.GetComponent<grid>().z = z;

                if(block.GetComponent<grid>().z == 0)
                {
                    block.GetComponent<grid>().spawnLvl = 1;
                }
                if (block.GetComponent<grid>().z == 0 || block.GetComponent<grid>().z == worldHeight-1 || block.GetComponent<grid>().x == 0 || block.GetComponent<grid>().x == worldWidth-1)
                {
                    block.GetComponent<grid>().spawnLvl = 1;
                }

                if(block.GetComponent<grid>().z == 0 && block.GetComponent<grid>().x == 0)
                {
                    block.GetComponent<grid>().spawnLvl = 2;
                }
                else if(block.GetComponent<grid>().z == 0 && block.GetComponent<grid>().x == worldWidth - 1)
                {
                    block.GetComponent<grid>().spawnLvl = 2;
                }
                else if (block.GetComponent<grid>().z == worldHeight - 1 && block.GetComponent<grid>().x == worldWidth - 1)
                {
                    block.GetComponent<grid>().spawnLvl = 2;
                }
                else if (block.GetComponent<grid>().z == worldHeight - 1 && block.GetComponent<grid>().x == 0)
                {
                    block.GetComponent<grid>().spawnLvl = 2;
                }

            }
        }
    }
}


