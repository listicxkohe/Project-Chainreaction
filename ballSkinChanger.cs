using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballSkinChanger : MonoBehaviour
{

    public ball ball;

    public void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = ball.ballSkin;


    }



}
