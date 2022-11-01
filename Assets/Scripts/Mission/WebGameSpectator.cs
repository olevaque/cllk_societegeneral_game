using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebGameSpectator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerTxt;
    [SerializeField] private Slider partPlaying;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        Main.TimerManager.OnTimerUpdate += OnTimerUpdate;
    }

    private void OnDisable()
    {
        Main.TimerManager.OnTimerUpdate -= OnTimerUpdate;
    }

    private void Start()
    {
        partPlaying.SetValueWithoutNotify(2);
    }

    private void OnDestroy()
    {
        
    }

    private void OnTimerUpdate(int arg1, int arg2, float arg3, string arg4)
    {
        timerTxt.text = arg4;
    }
}