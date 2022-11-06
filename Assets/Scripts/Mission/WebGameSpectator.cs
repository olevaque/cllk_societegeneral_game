using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebGameSpectator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameStatusTxt, timerTxt, sessionDateTxt;
    [SerializeField] private Slider sliderStep, sliderScene;

    private void Start()
    {
        Main.SocketIOManager.Instance.On("spectatorInfo", (string data) =>
        {
            SpectatorData specData = JsonUtility.FromJson<SpectatorData>(data);
            gameStatusTxt.text = "GAME: " + specData.name + " - <color=#E9041E>" + (specData.currentScene < 4 ? "In progress" : "Completed") + "</color> <size=22>- Version" + (specData.isVersionA ? "A" : "B") + "</size>";

            sliderScene.value = specData.currentScene;
            sliderStep.value = specData.currentStep;
        });

        sessionDateTxt.text = "Session of " + DateTime.Now.ToString("MM/dd/yyyy");

        StartCoroutine(UpdateSpectator());
    }

    private void OnDestroy()
    {
        Main.SocketIOManager.Instance.Off("spectatorInfo");
    }

    private IEnumerator UpdateSpectator()
    {
        yield return new WaitForSeconds(1f);

        UuidSessionData uuidSession = new UuidSessionData()
        {
            uuid = GameVersion.SessionUUID
        };

        Main.SocketIOManager.Instance.Emit("requestSpectator", JsonUtility.ToJson(uuidSession), false);
    }
}