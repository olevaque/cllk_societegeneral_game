using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class GBInputButton : MonoBehaviour
{
    public string text
    {
        get
        {
            return input.text;
        }

        set
        {
            input.text = value;
        }
    }
    public string textResult
    {
        get
        {
            return resultTxt.text;
        }
        set
        {
            resultTxt.text = value;
        }
    }

    [HideInInspector] public UnityEvent onClick;

    [SerializeField] private Image inputSquareImg, buttonImg;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI resultTxt;
    [SerializeField] private TMP_InputField input;

    private void OnEnable()
    {
        button.onClick.AddListener(OnBtnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnBtnClick);
    }

    private void Start()
    {

    }

    public void SetHasNeutral()
    {
        inputSquareImg.color = Color.black;
        buttonImg.color = Color.black;

        input.gameObject.SetActive(false);
    }

    public void SetHasGood(string info)
    {
        inputSquareImg.color = ColorPalette.goodAnswerColor;
        buttonImg.color = ColorPalette.goodAnswerColor;

        input.gameObject.SetActive(true);

        resultTxt.text = info;
    }

    public void SetHasBad(string info)
    {
        inputSquareImg.color = ColorPalette.badAnswerColor;
        buttonImg.color = ColorPalette.badAnswerColor;

        input.gameObject.SetActive(true);

        resultTxt.text = info;
    }

    private void OnBtnClick()
    {
        onClick?.Invoke();
    }
}
