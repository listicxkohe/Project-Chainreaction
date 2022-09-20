using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalTimer : MonoBehaviour
{

    public static globalTimer instance;
    public bool timerFinished = false;
    public bool ballDestroyed;

    private void Awake()
    {
        instance = this;
    }

    public void startTimer(float duration)
    {
        StartCoroutine(startTimerCourotine(duration));
    }

    IEnumerator startTimerCourotine(float Duration)
    {
        timerFinished = false;

        while (Duration > 0.0f )
        {
            Duration -= Time.deltaTime;
        }

        timerFinished = true;
        
        yield return null;
    }

}
