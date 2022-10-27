
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class TimerManager : MonoBehaviour
{
    public event Action OnTimerComplete;
    public event Action<int, int, float, string> OnTimerUpdate;

    private const float TIMER_MAX_SEC = 1800f;

    private bool isTimerStarted = false;

    private int minutesRemaining, secondsRemaining, totalSecondsRemaining;
    private float startTime, elapsedTime, pctProgress;

    private void Start()
    {
        Main.SocketIOManager.Instance.On("UpdateTimerGame", (string data) =>
        {
            TimerData timerData = JsonUtility.FromJson<TimerData>(data);

            string minutesStr = timerData.minutes < 10 ? "0" + timerData.minutes : timerData.minutes.ToString();
            string secondsStr = timerData.seconds < 10 ? "0" + timerData.seconds : timerData.seconds.ToString();

            minutesRemaining = int.Parse(minutesStr);
            secondsRemaining = int.Parse(secondsStr);

            totalSecondsRemaining = minutesRemaining * 60 + secondsRemaining;
            pctProgress = 1f - totalSecondsRemaining / TIMER_MAX_SEC;

            OnTimerUpdate?.Invoke(minutesRemaining, secondsRemaining, pctProgress, minutesStr + ":" + secondsStr);
        });
    }

    private void OnDestroy()
    {
        Main.SocketIOManager.Instance.Off("UpdateTimerGame");
    }

    private void Update()
    {
        if (isTimerStarted)
        {
            elapsedTime = Time.time - startTime;

            if (elapsedTime > TIMER_MAX_SEC)
            {
                OnTimerComplete?.Invoke();
            }
            else
            {
                totalSecondsRemaining = Mathf.FloorToInt(TIMER_MAX_SEC - elapsedTime);
                minutesRemaining = Mathf.FloorToInt((totalSecondsRemaining / 60) % 60); 
                secondsRemaining = Mathf.FloorToInt(totalSecondsRemaining % 60);
                pctProgress = 1f - totalSecondsRemaining / TIMER_MAX_SEC;

                string minutesStr = minutesRemaining < 10 ? "0" + minutesRemaining : minutesRemaining.ToString();
                string secondsStr = secondsRemaining < 10 ? "0" + secondsRemaining : secondsRemaining.ToString();

                OnTimerUpdate?.Invoke(minutesRemaining, secondsRemaining, pctProgress, minutesStr + ":" + secondsRemaining);
            }
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isTimerStarted = true;
    }

    public void SetTimer(int secondRemaining)
    {

    }
}
