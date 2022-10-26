using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;
using TMPro;
using System;

public class SharedCanvas : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private Slider soundVolumeSld;

    [Header("Others")]
    [SerializeField] private TextMeshProUGUI debugMessage;

    private CanvasGroup sharedCanvasCg;

    private void Awake()
    {
        sharedCanvasCg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Main.SocketIOManager.Instance.On("MX_Debug", (string data) =>
        {
            MX_Debug debug = JsonUtility.FromJson<MX_Debug>(data);
            debugMessage.text = debugMessage.text.Insert(0, debug.debugMessage + "\n");
        });
    }

    private void OnEnable()
    {
        soundVolumeSld.onValueChanged.AddListener(OnVolumeChanged);

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        soundVolumeSld.onValueChanged.RemoveListener(OnVolumeChanged);

        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnDestroy()
    {
        Main.SocketIOManager.Instance.Off("MX_Debug");
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        Main.SoundManager.ChangeVolume(0);

        /*if (next.name == SceneName.WEBGAME_JOINROOM || next.name == SceneName.WEBGAME_READMISSION || next.name == SceneName.WEBGAME_CONGRATULATION)
        {
            sharedCanvasCg.alpha = 0;
            sharedCanvasCg.blocksRaycasts = false;
        }
        else
        {
            sharedCanvasCg.alpha = 1;
            sharedCanvasCg.blocksRaycasts = true;
        }*/
    }

    public void ChangeVolumeUi(float volume)
    {
        soundVolumeSld.SetValueWithoutNotify(volume);
    }

    private void OnVolumeChanged(float volume)
    {
        Main.SoundManager.ChangeVolume(volume);
    }
}
