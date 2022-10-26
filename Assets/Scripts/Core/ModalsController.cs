using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ModalsController : MonoBehaviour
{
    private const float ANIM_DURATION = .35f;

    private CanvasGroup modalArea;

    private void Awake()
    {
        modalArea = GetComponent<CanvasGroup>();
        modalArea.alpha = 0;
        modalArea.blocksRaycasts = false;

        HideAllModal();
    }

    private void Start()
    {
    }

    public void OpenModal(GameObject modalGo)
    {
        HideAllModal();
        modalGo.SetActive(true);

        modalArea.blocksRaycasts = true;
        modalArea.DOFade(1f, ANIM_DURATION).SetEase(Ease.InOutQuad);
    }

    public void CloseModal()
    {
        modalArea.DOFade(0f, ANIM_DURATION).SetEase(Ease.InOutQuad).OnComplete(() => modalArea.blocksRaycasts = false);
    }

    private void HideAllModal()
    {
        foreach(Transform child in modalArea.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
