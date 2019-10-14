using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    TimeSpan countdownTimer;

    [SerializeField] Text countdownText;
    public void GetCountdown(TimeSpan x) //Gets the remaining time from DailyReward script
    {
        countdownTimer = x;
        InvokeRepeating("StartCountDown", 1, 1);
    }

    private void StartCountDown()
    {
        countdownTimer = countdownTimer.Subtract(new TimeSpan(0, 0, 1));
        if (countdownTimer.TotalSeconds > 0)
        {
            countdownText.text = countdownTimer.ToString();
        }
    }
}
