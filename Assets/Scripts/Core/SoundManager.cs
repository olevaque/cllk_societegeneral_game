using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource mainMusic;

    private void Awake()
    {
        mainMusic = GetComponent<AudioSource>();
    }

    private void Start()
    {
    }

    public void ChangeVolume(float volume)
    {
        mainMusic.volume = volume;
        Main.SharedCanvas.ChangeVolumeUi(volume);
    }
}
