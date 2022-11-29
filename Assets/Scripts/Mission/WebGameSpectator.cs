using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebGameSpectator : MonoBehaviour
{
    [SerializeField] private GameObject prefabPlayerName, prefabPlayerInfos;

    [SerializeField] private Transform playerNameContainer;
    [SerializeField] private TextMeshProUGUI gameStatusTxt, timerTxt, sessionDateTxt;
    [SerializeField] private Slider sliderStep, sliderScene;
    [SerializeField] private CanvasGroup foldersCg;

    private SpectatorDoc[] spectatorsDocs;
    private float timeSinceLastUpdateSpectator;

    private void Awake()
    {
        spectatorsDocs = FindObjectsOfType<SpectatorDoc>();
    }

    private void Start()
    {
        Main.SocketIOManager.Instance.On("spectatorInfo", (string data) =>
        {
            SpectatorData specData = JsonUtility.FromJson<SpectatorData>(data);
            gameStatusTxt.text = "GAME: " + specData.name + " - <color=#E9041E>" + (specData.currentScene < 4 ? "In progress" : "Completed") + "</color> <size=22>- Version" + (specData.isVersionA ? "A" : "B") + "</size>";

            timerTxt.text = specData.timer;

            sliderScene.value = specData.currentScene;
            sliderStep.value = specData.currentStep;

            foldersCg.alpha = specData.currentScene == 3 ? 1f : .1f;
            foldersCg.interactable = specData.currentScene == 3;
            foldersCg.blocksRaycasts = specData.currentScene == 3;

            // Clean les playersName et playersInfos précédent
            foreach (Transform child in playerNameContainer)
            {
                Destroy(child.gameObject);
            }
            foreach(SpectatorDoc doc in spectatorsDocs)
            {
                foreach(Transform child in doc.transform.GetChild(1))
                {
                    Destroy(child.gameObject);
                }
            }

            foreach(Player player in specData.players)
            {
                GameObject playerGo = Instantiate(prefabPlayerName, playerNameContainer);
                playerGo.GetComponentInChildren<TextMeshProUGUI>().text = player.pseudo;

                foreach(Docs docs in player.docViewed)
                {
                    string objToFind = "X" + docs.name.Substring(1);
                    GameObject docGo = GameObject.Find(objToFind);
                    if (docGo)
                    {
                        float minutesRemaining = Mathf.FloorToInt((docs.timeViewed / 60) % 60);
                        float secondsRemaining = Mathf.FloorToInt(docs.timeViewed % 60);
                        string minutesStr = minutesRemaining < 10 ? "0" + minutesRemaining : minutesRemaining.ToString();
                        string secondsStr = secondsRemaining < 10 ? "0" + secondsRemaining : secondsRemaining.ToString();

                        Transform playerInfosPnl = docGo.transform.GetChild(1);
                        GameObject playerInfoGo = Instantiate(prefabPlayerInfos, playerInfosPnl);
                        playerInfoGo.GetComponentInChildren<Image>().color = docs.isOpen ? Color.red : new Color(.5f, .5f, .5f, .5f);
                        playerInfoGo.GetComponentsInChildren<TextMeshProUGUI>()[0].text = docs.nbOpen.ToString();
                        playerInfoGo.GetComponentsInChildren<TextMeshProUGUI>()[1].text = minutesStr + ":" + secondsStr;
                    }
                    else
                    {
                        Debug.Log("Couldn't find doc: " + objToFind);
                    }
                }
            }
        });

        sessionDateTxt.text = "Session of " + DateTime.Now.ToString("MM/dd/yyyy");
    }

    private void OnDestroy()
    {
        Main.SocketIOManager.Instance.Off("spectatorInfo");
    }

    private void Update()
    {
        timeSinceLastUpdateSpectator += Time.deltaTime;

        if (timeSinceLastUpdateSpectator > 1)
        {
            timeSinceLastUpdateSpectator = 0;

            UuidSessionData uuidSession = new UuidSessionData()
            {
                uuid = GameVersion.SessionUUID
            };
            Main.SocketIOManager.Instance.Emit("requestSpectator", JsonUtility.ToJson(uuidSession), false);
        }
    }
}