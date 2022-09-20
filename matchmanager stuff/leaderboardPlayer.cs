using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class leaderboardPlayer : MonoBehaviour
{

    public TMP_Text playerNameText;

    public void SetDetails(string name)
    {
        playerNameText.text = name;
    }



}
