using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

public class CaptainController : MonoBehaviour
{
    public event Action<string> onProposePassword;
    public event Action<string> onProposeCode;
    public event Action onProposeSendReport;
    public event Action onProposeFinalChoose;
    public event Action<WGCC_Data> onCaptainInfoChange;

    public enum STEP { PASSWORD, CODE, PRINCIPAL_MISSION, BRAINTEASER, TRANSVERSAL, FINAL_CHOOSE, CONGRATULATION };
    private STEP currentStep = STEP.PASSWORD;

    [Header("General")]
    [SerializeField] private TextMeshProUGUI headerTimerTxt;
    [SerializeField] private Sprite btnExtendSpt, btnRetractSpt;
    [SerializeField] private Button transversalBtn, sendReportBtn;
    [SerializeField] private Image youAreCaptainImg;

    [Header("Password")]
    [SerializeField] private GameObject passwordPnl;
    [SerializeField] private GBInputButton passwordGbib;

    [Header("Code")]
    [SerializeField] private GameObject codePnl;
    [SerializeField] private GBInputButton codeGbib;

    [Header("PrincipalMission")]
    [SerializeField] private GameObject principalMissionPnl;
    [SerializeField] private GameObject content1Pnl, content2Pnl, content3Pnl, content4Pnl;
    [SerializeField] private Button extendRectractP1Btn, extendRectractP2Btn, extendRectractP3Btn, extendRectractP4Btn;
    [SerializeField] private Image extendRectractP1Spt, extendRectractP2Spt, extendRectractP3Spt, extendRectractP4Spt;
    [SerializeField] private Image iconC1P1Img, iconC2P1Img, iconC1P2Img, iconC2P2Img, iconC1P3Img, iconC2P3Img, iconC1P4Img, iconC2P4Img;
    [SerializeField] private Slider bngL1C1P1Sld, bngL1C2P1Sld, bngL2C1P1Sld, bngL2C2P1Sld, bngL3C1P1Sld, bngL3C2P1Sld;
    [SerializeField] private Slider bngL1C1P2Sld, bngL1C2P2Sld, bngL2C1P2Sld, bngL2C2P2Sld, bngL3C1P2Sld, bngL3C2P2Sld;
    [SerializeField] private Slider bngL1C1P3Sld, bngL1C2P3Sld, bngL2C1P3Sld, bngL2C2P3Sld, bngL3C1P3Sld, bngL3C2P3Sld;
    [SerializeField] private Slider bngL1C1P4Sld, bngL1C2P4Sld, bngL2C1P4Sld, bngL2C2P4Sld, bngL3C1P4Sld, bngL3C2P4Sld;

    [Header("BrainTeaserPnl")]
    [SerializeField] private GameObject brainTeaserPnl;

    [Header("TransversalPnl")]
    [SerializeField] private GameObject transversalPnl;
    [SerializeField] private GBInputButton transversal1Gbib, transversal2Gbib, transversal3Gbib, transversal4Gbib, transversal5Gbib;

    [Header("FinalChoosePnl")]
    [SerializeField] private GameObject finalChoosePnl;
    [SerializeField] private TextMeshProUGUI chooseC1Txt, chooseC2Txt;
    [SerializeField] private Button finalChooseBtn;

    [Header("CongratulationPnl")]
    [SerializeField] private GameObject congratulationPnl;

    WGCC_Data wgccData = new WGCC_Data();

    private void OnEnable()
    {
        Main.TimerManager.OnTimerUpdate += OnTimerUpdate;

        passwordGbib.onClick.AddListener(OnPasswordClick);
        codeGbib.onClick.AddListener(OnCodeClick);
        sendReportBtn.onClick.AddListener(OnSendReportClick);
        finalChooseBtn.onClick.AddListener(OnFinalChooseClick);

        passwordGbib.onValueChanged.AddListener(OnPasswordChange);
        codeGbib.onValueChanged.AddListener(OnCodeChange);

        extendRectractP1Btn.onClick.AddListener(OnExtendRectractP1Click);
        extendRectractP2Btn.onClick.AddListener(OnExtendRectractP2Click);
        extendRectractP3Btn.onClick.AddListener(OnExtendRectractP3Click);
        extendRectractP4Btn.onClick.AddListener(OnExtendRectractP4Click);

        bngL1C1P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C2P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C1P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C2P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C1P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C2P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C1P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C2P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C1P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C2P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C1P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C2P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C1P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C2P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C1P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C2P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C1P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C2P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C1P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C2P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C1P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C2P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C1P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C2P4Sld.onValueChanged.AddListener(OnBngSliderChange);
    }

    private void OnDisable()
    {
        Main.TimerManager.OnTimerUpdate -= OnTimerUpdate;

        passwordGbib.onClick.RemoveListener(OnPasswordClick);
        codeGbib.onClick.RemoveListener(OnCodeClick);
        sendReportBtn.onClick.RemoveListener(OnSendReportClick);
        finalChooseBtn.onClick.RemoveListener(OnFinalChooseClick);

        passwordGbib.onValueChanged.RemoveListener(OnPasswordChange);
        codeGbib.onValueChanged.RemoveListener(OnCodeChange);

        extendRectractP1Btn.onClick.RemoveListener(OnExtendRectractP1Click);
        extendRectractP2Btn.onClick.RemoveListener(OnExtendRectractP2Click);
        extendRectractP3Btn.onClick.RemoveListener(OnExtendRectractP3Click);
        extendRectractP4Btn.onClick.RemoveListener(OnExtendRectractP4Click);
    }

    private void Start()
    {
        youAreCaptainImg.gameObject.SetActive(false);

#if UNITY_WEBGL
        transversalBtn.gameObject.SetActive(false);
#else
        transversalBtn.gameObject.SetActive(true);
#endif

        UpdatePanelDisplay();
    }

    public void Initialise()
    {
        Sprite company1 = GameVersion.GetCompany1Icon();
        Sprite company2 = GameVersion.GetCompany2Icon();
        iconC1P1Img.sprite = company1;
        iconC2P1Img.sprite = company2;
        iconC1P2Img.sprite = company1;
        iconC2P2Img.sprite = company2;
        iconC1P3Img.sprite = company1;
        iconC2P3Img.sprite = company2;
        iconC1P4Img.sprite = company1;
        iconC2P4Img.sprite = company2;

        chooseC1Txt.text = GameVersion.GetCompany1Name();
        chooseC2Txt.text = GameVersion.GetCompany2Name();
    }

    public void GotoStep(STEP step)
    {
        currentStep = step;

        UpdatePanelDisplay();
    }
    
    public void SetCaptainMode(bool isCaptain)
    {
        youAreCaptainImg.gameObject.SetActive(isCaptain);
        passwordGbib.SetCaptainMode(isCaptain);
        codeGbib.SetCaptainMode(isCaptain);

        bngL1C1P1Sld.interactable = bngL1C2P1Sld.interactable = bngL2C1P1Sld.interactable = bngL2C2P1Sld.interactable = bngL3C1P1Sld.interactable = bngL3C2P1Sld.interactable =
        bngL1C1P2Sld.interactable = bngL1C2P2Sld.interactable = bngL2C1P2Sld.interactable = bngL2C2P2Sld.interactable = bngL3C1P2Sld.interactable = bngL3C2P2Sld.interactable =
        bngL1C1P3Sld.interactable = bngL1C2P3Sld.interactable = bngL2C1P3Sld.interactable = bngL2C2P3Sld.interactable = bngL3C1P3Sld.interactable = bngL3C2P3Sld.interactable =
        bngL1C1P4Sld.interactable = bngL1C2P4Sld.interactable = bngL2C1P4Sld.interactable = bngL2C2P4Sld.interactable = bngL3C1P4Sld.interactable = bngL3C2P4Sld.interactable = isCaptain;
    }

    public void SetInfoFromCaptain(WGCC_Data data)
    {
        passwordGbib.text = data.password;
        codeGbib.text = data.code;

        bngL1C1P1Sld.SetValueWithoutNotify(data.bngL1C1P1);
        bngL1C2P1Sld.SetValueWithoutNotify(data.bngL1C2P1);
        bngL2C1P1Sld.SetValueWithoutNotify(data.bngL2C1P1);
        bngL2C2P1Sld.SetValueWithoutNotify(data.bngL2C2P1);
        bngL3C1P1Sld.SetValueWithoutNotify(data.bngL3C1P1);
        bngL3C2P1Sld.SetValueWithoutNotify(data.bngL3C2P1);

        bngL1C1P2Sld.SetValueWithoutNotify(data.bngL1C1P2);
        bngL1C2P2Sld.SetValueWithoutNotify(data.bngL1C2P2);
        bngL2C1P2Sld.SetValueWithoutNotify(data.bngL2C1P2);
        bngL2C2P2Sld.SetValueWithoutNotify(data.bngL2C2P2);
        bngL3C1P2Sld.SetValueWithoutNotify(data.bngL3C1P2);
        bngL3C2P2Sld.SetValueWithoutNotify(data.bngL3C2P2);

        bngL1C1P3Sld.SetValueWithoutNotify(data.bngL1C1P3);
        bngL1C2P3Sld.SetValueWithoutNotify(data.bngL1C2P3);
        bngL2C1P3Sld.SetValueWithoutNotify(data.bngL2C1P3);
        bngL2C2P3Sld.SetValueWithoutNotify(data.bngL2C2P3);
        bngL3C1P3Sld.SetValueWithoutNotify(data.bngL3C1P3);
        bngL3C2P3Sld.SetValueWithoutNotify(data.bngL3C2P3);

        bngL1C1P4Sld.SetValueWithoutNotify(data.bngL1C1P4);
        bngL1C2P4Sld.SetValueWithoutNotify(data.bngL1C2P4);
        bngL2C1P4Sld.SetValueWithoutNotify(data.bngL2C1P4);
        bngL2C2P4Sld.SetValueWithoutNotify(data.bngL2C2P4);
        bngL3C1P4Sld.SetValueWithoutNotify(data.bngL3C1P4);
        bngL3C2P4Sld.SetValueWithoutNotify(data.bngL3C2P4);
    }

    public void DisplayWrongPassword()
    {
        passwordGbib.SetHasBad(string.Empty);
    }

    public void DisplayWrongCode()
    {
        codeGbib.SetHasBad(string.Empty);
    }

    private void OnTimerUpdate(int minutes, int seconds, float pct, string minsecStr)
    {
        headerTimerTxt.text = "Presentation to the general management in : <color=#E9041E>" + minsecStr + "</color>";
    }

    private void UpdatePanelDisplay()
    {
        passwordPnl.SetActive(currentStep == STEP.PASSWORD);
        codePnl.SetActive(currentStep == STEP.CODE);
        principalMissionPnl.SetActive(currentStep == STEP.PRINCIPAL_MISSION);
        brainTeaserPnl.SetActive(currentStep == STEP.BRAINTEASER);
        transversalPnl.SetActive(currentStep == STEP.TRANSVERSAL);
        finalChoosePnl.SetActive(currentStep == STEP.FINAL_CHOOSE);
        congratulationPnl.SetActive(currentStep == STEP.CONGRATULATION);
    }

    private void OnPasswordChange(string pass)
    {
        wgccData.password = pass;
        onCaptainInfoChange?.Invoke(wgccData);
    }

    private void OnCodeChange(string code)
    {
        wgccData.code = code;
        onCaptainInfoChange?.Invoke(wgccData);
    }

    private void OnBngSliderChange(float arg0)
    {
        wgccData.bngL1C1P1 = (int)bngL1C1P1Sld.value;
        wgccData.bngL1C2P1 = (int)bngL1C2P1Sld.value;
        wgccData.bngL2C1P1 = (int)bngL2C1P1Sld.value;
        wgccData.bngL2C2P1 = (int)bngL2C2P1Sld.value;
        wgccData.bngL3C1P1 = (int)bngL3C1P1Sld.value;
        wgccData.bngL3C2P1 = (int)bngL3C2P1Sld.value;

        wgccData.bngL1C1P2 = (int)bngL1C1P2Sld.value;
        wgccData.bngL1C2P2 = (int)bngL1C2P2Sld.value;
        wgccData.bngL2C1P2 = (int)bngL2C1P2Sld.value;
        wgccData.bngL2C2P2 = (int)bngL2C2P2Sld.value;
        wgccData.bngL3C1P2 = (int)bngL3C1P2Sld.value;
        wgccData.bngL3C2P2 = (int)bngL3C2P2Sld.value;

        wgccData.bngL1C1P3 = (int)bngL1C1P3Sld.value;
        wgccData.bngL1C2P3 = (int)bngL1C2P3Sld.value;
        wgccData.bngL2C1P3 = (int)bngL2C1P3Sld.value;
        wgccData.bngL2C2P3 = (int)bngL2C2P3Sld.value;
        wgccData.bngL3C1P3 = (int)bngL3C1P3Sld.value;
        wgccData.bngL3C2P3 = (int)bngL3C2P3Sld.value;

        wgccData.bngL1C1P4 = (int)bngL1C1P4Sld.value;
        wgccData.bngL1C2P4 = (int)bngL1C2P4Sld.value;
        wgccData.bngL2C1P4 = (int)bngL2C1P4Sld.value;
        wgccData.bngL2C2P4 = (int)bngL2C2P4Sld.value;
        wgccData.bngL3C1P4 = (int)bngL3C1P4Sld.value;
        wgccData.bngL3C2P4 = (int)bngL3C2P4Sld.value;
        onCaptainInfoChange?.Invoke(wgccData);
    }


    #region Clicks
    private void OnPasswordClick()
    {
        onProposePassword?.Invoke(passwordGbib.text);
    }

    private void OnCodeClick()
    {
        onProposeCode?.Invoke(codeGbib.text);
    }

    private void OnSendReportClick()
    {
        onProposeSendReport?.Invoke();
    }

    private void OnFinalChooseClick()
    {
        onProposeFinalChoose?.Invoke();
    }

    private void OnExtendRectractP1Click()
    {
        content1Pnl.SetActive(!content1Pnl.activeSelf);
    }

    private void OnExtendRectractP2Click()
    {
        content2Pnl.SetActive(!content2Pnl.activeSelf);
    }

    private void OnExtendRectractP3Click()
    {
        content3Pnl.SetActive(!content3Pnl.activeSelf);
    }

    private void OnExtendRectractP4Click()
    {
        content4Pnl.SetActive(!content4Pnl.activeSelf);
    }
#endregion
}
