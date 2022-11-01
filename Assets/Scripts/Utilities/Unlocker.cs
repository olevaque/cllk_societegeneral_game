using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

public class Unlocker : MonoBehaviour
{
    private const float SPEED_ANIM = .1f;
    private const int NB_LOOP = 8;

    public UnityEvent onDigitsValid;

    public string validDigits;

    [SerializeField] private TMP_InputField digit1Ipt, digit2Ipt, digit3Ipt, digit4Ipt;
    [SerializeField] private Button validateBtn;

    private void OnEnable()
    {
        validateBtn.onClick.AddListener(OnValidateClick);
    }

    private void OnDisable()
    {
        validateBtn.onClick.RemoveListener(OnValidateClick);
    }

    private void OnValidateClick()
    {
        string digitProposition = digit1Ipt.text + digit2Ipt.text + digit3Ipt.text + digit4Ipt.text;

        if (digitProposition != validDigits)
        {
            Color baseColor1 = digit1Ipt.textComponent.color;
            Color baseColor2 = digit2Ipt.textComponent.color;
            Color baseColor3 = digit3Ipt.textComponent.color;
            Color baseColor4 = digit4Ipt.textComponent.color;

            digit1Ipt.gameObject.GetComponent<Image>().DOColor(ColorPalette.badAnswerColor, SPEED_ANIM).SetLoops(NB_LOOP, LoopType.Yoyo).OnComplete(() =>
                digit1Ipt.gameObject.GetComponent<Image>().DOColor(baseColor1, SPEED_ANIM));
            digit2Ipt.gameObject.GetComponent<Image>().DOColor(ColorPalette.badAnswerColor, SPEED_ANIM).SetLoops(NB_LOOP, LoopType.Yoyo).OnComplete(() =>
                digit2Ipt.gameObject.GetComponent<Image>().DOColor(baseColor2, SPEED_ANIM));
            digit3Ipt.gameObject.GetComponent<Image>().DOColor(ColorPalette.badAnswerColor, SPEED_ANIM).SetLoops(NB_LOOP, LoopType.Yoyo).OnComplete(() =>
                digit3Ipt.gameObject.GetComponent<Image>().DOColor(baseColor3, SPEED_ANIM));
            digit4Ipt.gameObject.GetComponent<Image>().DOColor(ColorPalette.badAnswerColor, SPEED_ANIM).SetLoops(NB_LOOP, LoopType.Yoyo).OnComplete(() =>
                digit4Ipt.gameObject.GetComponent<Image>().DOColor(baseColor4, SPEED_ANIM));

            digit1Ipt.textComponent.DOColor(ColorPalette.badAnswerColor, SPEED_ANIM).SetLoops(NB_LOOP, LoopType.Yoyo).OnComplete(() =>
                digit1Ipt.textComponent.DOColor(baseColor1, SPEED_ANIM));
            digit2Ipt.textComponent.DOColor(ColorPalette.badAnswerColor, SPEED_ANIM).SetLoops(NB_LOOP, LoopType.Yoyo).OnComplete(() =>
                digit2Ipt.textComponent.DOColor(baseColor2, SPEED_ANIM));
            digit3Ipt.textComponent.DOColor(ColorPalette.badAnswerColor, SPEED_ANIM).SetLoops(NB_LOOP, LoopType.Yoyo).OnComplete(() =>
                digit3Ipt.textComponent.DOColor(baseColor3, SPEED_ANIM));
            digit4Ipt.textComponent.DOColor(ColorPalette.badAnswerColor, SPEED_ANIM).SetLoops(NB_LOOP, LoopType.Yoyo).OnComplete(() =>
                digit4Ipt.textComponent.DOColor(baseColor4, SPEED_ANIM));
        }
        else
        {
            onDigitsValid?.Invoke();
        }
    }
}
