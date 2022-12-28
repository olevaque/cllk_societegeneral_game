using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebGameSpectator : MonoBehaviour
{
    private enum TAB { CAPTAIN, DOCUMENTS, COMPANY };
    private TAB currentTab = TAB.CAPTAIN;

    [Header("PlayerPrefab")]
    [SerializeField] private GameObject prefabPlayerName, prefabPlayerInfos;
    [SerializeField] private Transform playerNameContainer;

    [Header("Header")]
    [SerializeField] private Slider sliderScene;
    [SerializeField] private TextMeshProUGUI gameStatusTxt, timerTxt, sessionDateTxt, versionTxt;

    [Header("General")]
    [SerializeField] private Button tabGeneral;
    [SerializeField] private GameObject tabContent;
    [SerializeField] private TextMeshProUGUI captainsTxt;

    [Header("Documents")]
    [SerializeField] private Button tabDocuments;
    [SerializeField] private GameObject tabContentDocuments;
    [SerializeField] private GameObject XMenu;
    [SerializeField] private Sprite docLockSpt, docViewSpt;

    [Header("Company")]
    [SerializeField] private Button tabCompany;
    [SerializeField] private Image company1_1, company1_2, company2_1, company2_2;
    [SerializeField] private GameObject tabContentCompany;
    [SerializeField] private Slider bngL1C1P1Sld, bngL1C2P1Sld, bngL2C1P1Sld, bngL2C2P1Sld, bngL3C1P1Sld, bngL3C2P1Sld;
    [SerializeField] private Slider bngL1C1P2Sld, bngL1C2P2Sld, bngL2C1P2Sld, bngL2C2P2Sld, bngL3C1P2Sld, bngL3C2P2Sld;
    [SerializeField] private Slider bngL1C1P3Sld, bngL1C2P3Sld, bngL2C1P3Sld, bngL2C2P3Sld, bngL3C1P3Sld, bngL3C2P3Sld;
    [SerializeField] private Slider bngL1C1P4Sld, bngL1C2P4Sld, bngL2C1P4Sld, bngL2C2P4Sld, bngL3C1P4Sld, bngL3C2P4Sld;
    [SerializeField] private TextMeshProUGUI finalCompanyTxt;

    private SpectatorDoc[] spectatorsDocs;
    private float timeSinceLastUpdateSpectator;

    private void Awake()
    {
        spectatorsDocs = FindObjectsOfType<SpectatorDoc>();
    }

    private void OnEnable()
    {
        tabGeneral.onClick.AddListener(OnTabCaptainClick);
        tabDocuments.onClick.AddListener(OnTabDocumentsClick);
        tabCompany.onClick.AddListener(OnTabCompanyClick);
    }

    private void OnDisable()
    {
        tabGeneral.onClick.RemoveListener(OnTabCaptainClick);
        tabDocuments.onClick.RemoveListener(OnTabDocumentsClick);
        tabCompany.onClick.RemoveListener(OnTabCompanyClick);
    }

    private void Start()
    {
        Main.SocketIOManager.Instance.On("spectatorInfo", (string data) =>
        {
            SpectatorData specData = JsonUtility.FromJson<SpectatorData>(data);
            GameVersion.IsVersionA = specData.isVersionA;

            gameStatusTxt.text = specData.name + " - <color=#E9041E>" + (specData.currentScene < 4 ? "In progress" : "Completed") + "</color>";
            versionTxt.text = "Version " + (specData.isVersionA ? "A" : "B");
            timerTxt.text = specData.timer;

            company1_1.sprite = GameVersion.GetCompany1Icon();
            company1_2.sprite = GameVersion.GetCompany1Icon();
            company2_1.sprite = GameVersion.GetCompany2Icon();
            company2_2.sprite = GameVersion.GetCompany2Icon();

            sliderScene.value = specData.currentScene;

            //////////////////////
            // General
            string captains = "-";
            if (specData.roomSpectatorInfo.captainVoteFinal != null)
            {
                captains = string.Empty;
                foreach (UserVoteCaptain uv in specData.roomSpectatorInfo.captainVoteFinal)
                {
                    captains += "<color=#E9041E>" + uv.player.pseudo + "</color> voted for <color=#E9041E>" + uv.vote.pseudo + "</color>\n";
                }
            }
            captainsTxt.text = captains;

            //////////////////////
            // Documents
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

            foreach (PlayerForSpectator playerForSpectator in specData.roomSpectatorInfo.playersForSpectator)
            {
                // PlayerName
                GameObject playerGo = Instantiate(prefabPlayerName, playerNameContainer);
                if (playerForSpectator.hasDisconnect)
                {
                    playerGo.GetComponentInChildren<TextMeshProUGUI>().text = "<s>" + playerForSpectator.pseudo + "</s>";
                    playerGo.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
                }
                else
                {
                    playerGo.GetComponentInChildren<TextMeshProUGUI>().text = playerForSpectator.pseudo;
                }
                if (specData.roomSpectatorInfo.captainForSpectator.psckId == playerForSpectator.psckId) playerGo.GetComponentInChildren<TextMeshProUGUI>().text += "(C)";

                bool playerHasADocOpen = false;

                // PlayerDocs
                foreach (Docs docs in playerForSpectator.docViewed)
                {
                    string objToFind = "X" + docs.name.Substring(1);
                    GameObject docGo = GameObject.Find(objToFind);
                    if (docGo)
                    {

                        Transform playerInfosPnl = docGo.transform.GetChild(1);
                        GameObject playerInfoGo = Instantiate(prefabPlayerInfos, playerInfosPnl);
                        playerInfoGo.GetComponentInChildren<Image>().sprite = docs.isLock ? docLockSpt : docViewSpt;
                        playerInfoGo.GetComponentInChildren<Image>().color = docs.isOpen ? Color.red : (docs.isLock ? new Color(.75f, .75f, .75f, .5f) : new Color(.5f, .5f, .5f, .75f));
                        playerInfoGo.GetComponentsInChildren<TextMeshProUGUI>()[0].text = docs.nbOpen.ToString();
                        playerInfoGo.GetComponentsInChildren<TextMeshProUGUI>()[1].text = secondToFormattedTime(docs.timeViewed);
                        playerInfoGo.GetComponentsInChildren<TextMeshProUGUI>()[0].alpha = docs.isLock ? .25f : 1f;
                        playerInfoGo.GetComponentsInChildren<TextMeshProUGUI>()[1].alpha = docs.isLock ? .25f : 1f;

                        if (docs.isOpen) playerHasADocOpen = true;
                    }
                }

                // Menu
                Transform menuInfosPnl = XMenu.transform.GetChild(1);
                GameObject menuInfoGo = Instantiate(prefabPlayerInfos, menuInfosPnl);
                menuInfoGo.GetComponentInChildren<Image>().sprite = docViewSpt;
                menuInfoGo.GetComponentInChildren<Image>().color = !playerHasADocOpen ? Color.red : new Color(.5f, .5f, .5f, .5f);
                menuInfoGo.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "-";
                menuInfoGo.GetComponentsInChildren<TextMeshProUGUI>()[1].text = secondToFormattedTime(playerForSpectator.menuTimeViewed);
            }

            //////////////////////
            // Company
            bngL1C1P1Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL1C1P1);
            bngL1C2P1Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL1C2P1);
            bngL2C1P1Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL2C1P1);
            bngL2C2P1Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL2C2P1);
            bngL3C1P1Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL3C1P1);
            bngL3C2P1Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL3C2P1);

            bngL1C1P2Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL1C1P2);
            bngL1C2P2Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL1C2P2);
            bngL2C1P2Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL2C1P2);
            bngL2C2P2Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL2C2P2);
            bngL3C1P2Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL3C1P2);
            bngL3C2P2Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL3C2P2);

            bngL1C1P3Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL1C1P3);
            bngL1C2P3Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL1C2P3);
            bngL2C1P3Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL2C1P3);
            bngL2C2P3Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL2C2P3);
            bngL3C1P3Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL3C1P3);
            bngL3C2P3Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL3C2P3);

            bngL1C1P4Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL1C1P4);
            bngL1C2P4Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL1C2P4);
            bngL2C1P4Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL2C1P4);
            bngL2C2P4Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL2C2P4);
            bngL3C1P4Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL3C1P4);
            bngL3C2P4Sld.SetValueWithoutNotify(specData.roomSpectatorInfo.wgccData.bngL3C2P4);

            if (specData.roomSpectatorInfo.wgccData.company == 0)
            {
                finalCompanyTxt.text = "-";
            }
            else if (specData.roomSpectatorInfo.wgccData.company == 1)
            {
                finalCompanyTxt.text = GameVersion.GetCompany1Name();
            }
            else if (specData.roomSpectatorInfo.wgccData.company == 2)
            {
                finalCompanyTxt.text = GameVersion.GetCompany2Name();
            }
            else
            {
                finalCompanyTxt.text = "No choose";
            }
        });

        sessionDateTxt.text = "Session of " + DateTime.Now.ToString("MM/dd/yyyy");
        UpdateTabs();
    }

    private void OnDestroy()
    {
        Main.SocketIOManager.Instance.Off("spectatorInfo");
    }

    private void Update()
    {
        timeSinceLastUpdateSpectator += Time.deltaTime;

        if (timeSinceLastUpdateSpectator > 2)
        {
            timeSinceLastUpdateSpectator = 0;

            UuidSessionData uuidSession = new UuidSessionData()
            {
                uuid = GameVersion.SessionUUID
            };
            Main.SocketIOManager.Instance.Emit("requestSpectator", JsonUtility.ToJson(uuidSession), false);
        }
    }

    private void OnTabCaptainClick()
    {
        currentTab = TAB.CAPTAIN;
        UpdateTabs();
    }

    private void OnTabDocumentsClick()
    {
        currentTab = TAB.DOCUMENTS;
        UpdateTabs();
    }

    private void OnTabCompanyClick()
    {
        currentTab = TAB.COMPANY;
        UpdateTabs();
    }

    private void UpdateTabs()
    {
        tabContent.SetActive(currentTab == TAB.CAPTAIN);
        tabContentDocuments.SetActive(currentTab == TAB.DOCUMENTS);
        tabContentCompany.SetActive(currentTab == TAB.COMPANY);

        tabGeneral.GetComponent<Image>().color = currentTab == TAB.CAPTAIN ? new Color(0.8039216f, 0, 0.01176471f, 1f) : new Color(0.8980392f, 0.372549f, 0.3137255f, 0.5019608f);
        tabDocuments.GetComponent<Image>().color = currentTab == TAB.DOCUMENTS ? new Color(0.8039216f, 0, 0.01176471f, 1f) : new Color(0.8980392f, 0.372549f, 0.3137255f, 0.5019608f);
        tabCompany.GetComponent<Image>().color = currentTab == TAB.COMPANY ? new Color(0.8039216f, 0, 0.01176471f, 1f) : new Color(0.8980392f, 0.372549f, 0.3137255f, 0.5019608f);
    }

    private string secondToFormattedTime(int sec)
    {
        float minutesRemaining = Mathf.FloorToInt((sec / 60) % 60);
        float secondsRemaining = Mathf.FloorToInt(sec % 60);
        string minutesStr = minutesRemaining < 10 ? "0" + minutesRemaining : minutesRemaining.ToString();
        string secondsStr = secondsRemaining < 10 ? "0" + secondsRemaining : secondsRemaining.ToString();

        return minutesStr + ":" + secondsStr;
    }
}