﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    //UI
    public Text timeLabel; //only use if your timer uses a label
    public Button timerButton; //used to disable button when needed
    public Image _progress;
    public Text countdownTimer;
    //TIME ELEMENTS
    public int hours; //to set the hours
    public int minutes; //to set the minutes
    public int seconds; //to set the seconds
    private bool _timerComplete = false;
    private bool _timerIsReady;
    private TimeSpan _startTime;
    private TimeSpan _endTime;
    private TimeSpan _remainingTime;
    //progress filler
    private float _value = 1f;
    //reward to claim
    public int RewardToEarn;



    //startup
    void Start()
    {
        IncreaseTime();
        if (PlayerPrefs.GetString("_timer") == "") //First Login
        {
            PlayerPrefs.SetInt("rewardsClaimedToday", 0);
            Debug.Log("==> Enableing button");
            enableButton();
        }
        else
        {
            disableButton();
            StartCoroutine("CheckTime");
        }
    }



    //update the time information with what we got some the internet
    private void updateTime()
    {
        if (PlayerPrefs.GetString("_timer") == "Standby") //Button just clicked
        {
            PlayerPrefs.SetString("_timer", TimeManager.sharedInstance.getCurrentTimeNow());
            PlayerPrefs.SetInt("_date", TimeManager.sharedInstance.getCurrentDateNow());
        }
        else if (PlayerPrefs.GetString("_timer") != "" && PlayerPrefs.GetString("_timer") != "Standby")
        {
            int _old = PlayerPrefs.GetInt("_date");
            int _now = TimeManager.sharedInstance.getCurrentDateNow();


            //check if a day as passed
            if (_now > _old)
            {//day as passed
                PlayerPrefs.SetInt("rewardsClaimedToday", 0);
                Debug.Log("Day has passed");
                enableButton();
                return;
            }
            else if (_now == _old)
            {//same day
                Debug.Log("Same Day - configuring now");
                _configTimerSettings();
                return;
            }
            else
            {
                Debug.Log("error with date");
                return;
            }
        }
        Debug.Log("Day had passed - configuring now");
        _configTimerSettings();
    }

    //setting up and configureing the values
    //update the time information with what we got some the internet
    private void _configTimerSettings()
    {
        _startTime = TimeSpan.Parse(PlayerPrefs.GetString("_timer")); //time of last claim
        _endTime = TimeSpan.Parse(hours + ":" + minutes + ":" + seconds); //amount of time after which other claim is allowed
        TimeSpan temp = TimeSpan.Parse(TimeManager.sharedInstance.getCurrentTimeNow()); //current time
        TimeSpan diff = temp.Subtract(_startTime);
        _remainingTime = _endTime.Subtract(diff);
        FindObjectOfType<CountdownTimer>().GetCountdown(_remainingTime); //start timer
        //start timmer where we left off
        setProgressWhereWeLeftOff();

        if (diff >= _endTime)
        {
            _timerComplete = true;
            enableButton();
        }
        else
        {
            _timerComplete = false;
            disableButton();
            _timerIsReady = true;
        }
    }

    private void IncreaseTime()
    {
        if (hours < 24 && !_timerComplete)
        {
            seconds *= PlayerPrefs.GetInt("rewardsClaimedToday") / RewardToEarn;
            minutes *= PlayerPrefs.GetInt("rewardsClaimedToday") / RewardToEarn;
            hours *= PlayerPrefs.GetInt("rewardsClaimedToday") / RewardToEarn;
            while (seconds >= 60)
            {
                seconds -= 60;
                minutes += 1;
            }
            while (minutes >= 60)
            {

                minutes -= 60;
                hours += 1;
            }
        }
    }

    //initializing the value of the timer
    private void setProgressWhereWeLeftOff()
    {
        float ah = 1f / (float)_endTime.TotalSeconds;
        float bh = 1f / (float)_remainingTime.TotalSeconds;
        _value = ah / bh;
        _progress.fillAmount = _value;
    }



    //enable button function
    private void enableButton()
    {
        timerButton.interactable = true;
        timeLabel.text = "CLAIM REWARD";
    }



    //disable button function
    private void disableButton()
    {
        timerButton.interactable = false;
        timeLabel.text = "NOT READY";
    }


    //use to check the current time before completely any task. use this to validate
    private IEnumerator CheckTime()
    {
        disableButton();
        timeLabel.text = "Checking the time";
        Debug.Log("==> Checking for new time");
        yield return StartCoroutine(
            TimeManager.sharedInstance.getTime()
        );
        updateTime();
        Debug.Log("==> Time check complete!");

    }


    //trggered on button click
    public void rewardClicked()
    {
        Debug.Log("==> Claim Button Clicked");
        claimReward(RewardToEarn);
        IncreaseTime();
        PlayerPrefs.SetString("_timer", "Standby");
        StartCoroutine("CheckTime");
    }



    //update method to make the progress tick
    void Update()
    {
        if (_timerIsReady)
        {
            if (!_timerComplete && PlayerPrefs.GetString("_timer") != "")
            {
                _value -= Time.deltaTime * 1f / (float)_endTime.TotalSeconds;
                _progress.fillAmount = _value;

                //this is called once only
                if (_value <= 0 && !_timerComplete)
                {
                    //when the timer hits 0, let do a quick validation to make sure no speed hacks.
                    validateTime();
                    _timerComplete = true;
                }
            }
        }
    }

    //validator
    private void validateTime()
    {
        Debug.Log("==> Validating time to make sure no speed hack!");
        StartCoroutine("CheckTime");
    }


    private void claimReward(int x)
    { 
        PlayerPrefs.SetInt("rewardsClaimedToday", PlayerPrefs.GetInt("rewardsClaimedToday") + x);
        Debug.Log("YOU EARN " + x + " REWARDS");
        StartCoroutine(GoBackDelay());
    }

    IEnumerator GoBackDelay()
    {
        yield return new WaitForSeconds(2);
        FindObjectOfType<SceneManager>().GoBack();
    }
}