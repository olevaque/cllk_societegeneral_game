using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(AudioSource))]
public class MiniAudioPlayer : MonoBehaviour
{
    private const float REWIND_FORWARD_TIME = 5;

    [SerializeField] private Button playPauseBtn, rewindBtn, forwardBtn;
    [SerializeField] private TextMeshProUGUI audioTimerTxt;
    [SerializeField] private Sprite pauseSpt, playSpt;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        playPauseBtn.onClick.AddListener(OnPlayPauseClick);
        rewindBtn.onClick.AddListener(OnRewindClick);
        forwardBtn.onClick.AddListener(OnForwardClick);

        UpdatePlayPauseBtn();
    }

    private void OnDisable()
    {
        playPauseBtn.onClick.RemoveListener(OnPlayPauseClick);
        rewindBtn.onClick.RemoveListener(OnRewindClick);
        forwardBtn.onClick.RemoveListener(OnForwardClick);
    }


    private void Start()
    {
        
    }

    private void Update()
    {
        float currentMin = Mathf.FloorToInt((audioSource.time / 60) % 60);
        float currentSec = Mathf.FloorToInt(audioSource.time % 60);
        string currentMinStr = currentMin < 10 ? "0" + currentMin : currentMin.ToString();
        string currentSecStr = currentSec < 10 ? "0" + currentSec : currentSec.ToString();

        float totalMin = Mathf.FloorToInt((audioSource.clip.length / 60) % 60);
        float totalSec = Mathf.FloorToInt(audioSource.clip.length % 60);
        string totalMinStr = totalMin < 10 ? "0" + totalMin : totalMin.ToString();
        string totalSecStr = totalSec < 10 ? "0" + totalSec : totalSec.ToString();

        audioTimerTxt.text = currentMinStr + ":" + currentSecStr + "/" + totalMinStr + ":" + totalSecStr;
    }

    private void OnPlayPauseClick()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }

        UpdatePlayPauseBtn();
    }

    private void OnRewindClick()
    {
        if (audioSource.time >= REWIND_FORWARD_TIME)
        {
            audioSource.time -= REWIND_FORWARD_TIME;
        }
    }

    private void OnForwardClick()
    {
        if (audioSource.time + REWIND_FORWARD_TIME < audioSource.clip.length)
        {
            audioSource.time += REWIND_FORWARD_TIME;
        }
    }

    private void UpdatePlayPauseBtn()
    {
        playPauseBtn.GetComponent<Image>().sprite = audioSource.isPlaying ? pauseSpt : playSpt;
    }
}
