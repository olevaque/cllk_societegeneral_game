
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class TimerManager : MonoBehaviour
{
    private const int NB_BRAINTEASER = 5;
    private enum BRAINTEASER_STATE { TODO, STARTED, COMPLETE }

    public event Action OnTimerComplete;
    public event Action<int, int, float, string> OnTimerUpdate;
    public event Action<int> OnBrainTeaserStart;
    public event Action OnBrainTeaserEnd;
    public event Action<string> OnBrainTeaserUpdate;

    private const float TIMER_40MIN = 2400f;
    private const float TIMER_1M30 = 90f;

    private bool isTimer40mStarted, isTimer1m30Started = false;

    private int minutesRemaining, secondsRemaining, totalSecondsRemaining;
    private float startTime, elapsedTime, pctProgress;

    private float extraTimeSec = 0;

    private BRAINTEASER_STATE[] brainteaserState = new BRAINTEASER_STATE[NB_BRAINTEASER];

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
            pctProgress = 1f - totalSecondsRemaining / TIMER_40MIN;

            if (timerData.currentStep != (int)CaptainController.STEP.FINAL_CHOOSE)
            {
                CheckBrainteaser(pctProgress);
            }

            OnTimerUpdate?.Invoke(minutesRemaining, secondsRemaining, pctProgress, minutesStr + ":" + secondsStr);
        });
    }

    private void OnDestroy()
    {
        Main.SocketIOManager.Instance.Off("UpdateTimerGame");
    }

    private void Update()
    {
        if (isTimer40mStarted || isTimer1m30Started)
        {
            elapsedTime = Time.time - startTime;

            float totalTime;
            if (isTimer40mStarted)
            {
                totalTime = TIMER_40MIN + extraTimeSec;
            }
            else
            {
                totalTime = TIMER_1M30;
            }

            if (elapsedTime > totalTime)
            {
                OnTimerComplete?.Invoke();
            }
            else
            {
                totalSecondsRemaining = Mathf.FloorToInt(totalTime - elapsedTime);
                minutesRemaining = Mathf.FloorToInt((totalSecondsRemaining / 60) % 60);
                secondsRemaining = Mathf.FloorToInt(totalSecondsRemaining % 60);
                pctProgress = 1f - totalSecondsRemaining / totalTime;

                string minutesStr = minutesRemaining < 10 ? "0" + minutesRemaining : minutesRemaining.ToString();
                string secondsStr = secondsRemaining < 10 ? "0" + secondsRemaining : secondsRemaining.ToString();

                if (isTimer40mStarted)
                {
                    CheckBrainteaser(pctProgress);
                }

                OnTimerUpdate?.Invoke(minutesRemaining, secondsRemaining, pctProgress, minutesStr + ":" + secondsStr);
            }
        }
    }

    public void StartTimer40min()
    {
        startTime = Time.time;
        isTimer40mStarted = true;
    }

    public void StartTimer1m30()
    {
        startTime = Time.time;
        isTimer1m30Started = true;
    }

    public void StopTimer()
    {
        isTimer40mStarted = false;
        isTimer1m30Started = false;
    }

    public void AddExtraTime(float extraSec)
    {
        extraTimeSec += extraSec;
    }
    public void SubstractExtraTime(float extraSec)
    {
        extraTimeSec -= extraSec;
    }

    private void CheckBrainteaser(float pctProgress)
    {
        float[] pctBrainTrigger = new float[NB_BRAINTEASER];
        pctBrainTrigger[0] = 7 * 60 / TIMER_40MIN;
        pctBrainTrigger[1] = 14 * 60 / TIMER_40MIN;
        pctBrainTrigger[2] = 21 * 60 / TIMER_40MIN;
        pctBrainTrigger[3] = 28 * 60 / TIMER_40MIN;
        pctBrainTrigger[4] = 35 * 60 / TIMER_40MIN;

        /* DEBUG
        pctBrainTrigger = new float[NB_BRAINTEASER];
        pctBrainTrigger[0] = 10 / TIMER_40MIN;
        pctBrainTrigger[1] = 30 / TIMER_40MIN;
        pctBrainTrigger[2] = 50 / TIMER_40MIN;
        pctBrainTrigger[3] = 70 / TIMER_40MIN;
        pctBrainTrigger[4] = 90 / TIMER_40MIN;
        pctBrainTrigger[5] = 110 / TIMER_40MIN;
        pctBrainTrigger[6] = 130 / TIMER_40MIN;
        pctBrainTrigger[7] = 150 / TIMER_40MIN;
        pctBrainTrigger[8] = 170 / TIMER_40MIN;
        pctBrainTrigger[9] = 190 / TIMER_40MIN;
        */

        float pct30s = 90 / TIMER_40MIN;

        for (int i=0; i< NB_BRAINTEASER; i++)
        {
            if (pctProgress >= pctBrainTrigger[i] && brainteaserState[i] == BRAINTEASER_STATE.TODO)
            {
                brainteaserState[i] = BRAINTEASER_STATE.STARTED;
                OnBrainTeaserStart?.Invoke(i);
            }
            else if (pctProgress <= pctBrainTrigger[i] + pct30s && brainteaserState[i] == BRAINTEASER_STATE.STARTED)
            {
                int nbSecRemaining = Mathf.FloorToInt((pctBrainTrigger[i] + pct30s - pctProgress) * TIMER_40MIN);
                OnBrainTeaserUpdate?.Invoke(nbSecRemaining.ToString());
            }
            else if (pctProgress > pctBrainTrigger[i] + pct30s && brainteaserState[i] == BRAINTEASER_STATE.STARTED)
            {
                brainteaserState[i] = BRAINTEASER_STATE.COMPLETE;
                OnBrainTeaserEnd?.Invoke();
            }
        }
    }
}
