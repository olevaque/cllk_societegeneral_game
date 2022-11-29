using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DocumentsController : MonoBehaviour
{
    private const int NB_FOLDER = 6;

    [SerializeField] private ModalsController modalsController;

    [SerializeField] private Button[] folderBtns;
    [SerializeField] private GameObject[] thumbnailsVersionA, thumbnailsVersionB;
    [SerializeField] private Unlocker[] unlockerVersionA, unlockerVersionB;
    [SerializeField] private Image[] lockerImgs;
    [SerializeField] private GameObject soundDocPnl, paperDocPnl, docZoomedPnl, docZoomedImg;
    [SerializeField] private Button closeZoomBtn, prevBtn, nextBtn, backBtn;

    private bool[] unlockedFolders = new bool[NB_FOLDER];

    private Document[] docThumbnails;

    private Transform thumbParent;
    private int thumbSelected;

    private Document currentDocOpen;

    private CanvasGroup docsCg;

    private void Awake()
    {
        docThumbnails = GetComponentsInChildren<Document>(true);
        docsCg = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        for (int i=0; i<NB_FOLDER; i++)
        {
            int closureIndex = i;
            folderBtns[i].onClick.AddListener(() => OnFolderClick(closureIndex));
        }
        for (int i=0; i< docThumbnails.Length; i++)
        {
            int closureIndex = i;
            docThumbnails[closureIndex].GetComponent<Button>().onClick.AddListener(() => ShowDocument(docThumbnails[closureIndex].transform));
        }

        prevBtn.onClick.AddListener(onPrevClick);
        nextBtn.onClick.AddListener(onNextClick);
        backBtn.onClick.AddListener(OnBackClick);
        closeZoomBtn.onClick.AddListener(OnCloseZoomClick);
    }

    private void OnDisable()
    {
        prevBtn.onClick.RemoveListener(onPrevClick);
        nextBtn.onClick.RemoveListener(onNextClick);
        backBtn.onClick.RemoveListener(OnBackClick);
        closeZoomBtn.onClick.RemoveListener(OnCloseZoomClick);
    }

    private void Start()
    {
        HideDocs();
        HideThumbnails();
        HideZoomedDoc();

        // Par defaut, le premier locker est déverouillé
        UnlockLocker(0);
    }

    public void UnlockLocker(int idLock)
    {
        modalsController.CloseModal();

        unlockedFolders[idLock] = true;
        UpdateDisplayLockers();
    }

    private void OnFolderClick(int index)
    {
        if (unlockedFolders[index])
        {
            ShowDocs();

            ShowThumbnailDocs(index);
        }
        else
        {
            foreach(Unlocker unlockerA in unlockerVersionA)
            {
                unlockerA.gameObject.SetActive(false);
            }
            foreach (Unlocker unlockerB in unlockerVersionB)
            {
                unlockerB.gameObject.SetActive(false);
            }

            if (GameVersion.IsVersionA)
            {
                modalsController.OpenModal(unlockerVersionA[index].gameObject);
            }
            else
            {

                modalsController.OpenModal(unlockerVersionB[index].gameObject);
            }
        }
    }

    private void UpdateDisplayLockers()
    {
        for (int i=0; i<NB_FOLDER; i++)
        {
            lockerImgs[i].gameObject.SetActive(!unlockedFolders[i]);
        }
    }

    private void ShowDocs()
    {
        docsCg.alpha = 1f;
        docsCg.blocksRaycasts = true;

        backBtn.gameObject.SetActive(true);
    }

    private void HideDocs()
    {
        docsCg.alpha = 0f;
        docsCg.blocksRaycasts = false;

        backBtn.gameObject.SetActive(false);
    }

    private void ShowThumbnailDocs(int index)
    {
        HideThumbnails();
        if (GameVersion.IsVersionA)
        {
            thumbnailsVersionA[index].SetActive(true);
        }
        else
        {

            thumbnailsVersionB[index].SetActive(true);
        }
    }

    private void ShowDocument(Transform thumbnailRequest)
    {
        UpdateArrows(thumbnailRequest.parent, thumbnailRequest.GetSiblingIndex());

        docZoomedPnl.SetActive(true);

        // Close the previous document
        if (currentDocOpen) currentDocOpen.Close(currentDocOpen.name);

        // Open the new one
        currentDocOpen = thumbnailRequest.GetComponent<Document>();
        if (currentDocOpen is SoundDocument)
        {
            soundDocPnl.SetActive(true);
            paperDocPnl.SetActive(false);
        }
        else if (currentDocOpen is PaperDocument)
        {
            soundDocPnl.SetActive(false);
            paperDocPnl.SetActive(true);

            var imgTarget = thumbnailRequest.GetComponentsInChildren<Image>()[1];
            docZoomedImg.GetComponent<Image>().sprite = imgTarget.sprite;
            docZoomedImg.GetComponent<LayoutElement>().preferredHeight = imgTarget.sprite.rect.height > imgTarget.sprite.rect.width ? 1280f : 926f;
        }
        currentDocOpen.Open(currentDocOpen.name);
    }

    private void HideZoomedDoc()
    {
        // Close the previous document
        if (currentDocOpen) currentDocOpen.Close(currentDocOpen.name);

        docZoomedPnl.SetActive(false);
    }

    private void OnBackClick()
    {
        HideZoomedDoc();
        HideDocs();
    }
    private void OnCloseZoomClick()
    {
        HideZoomedDoc();
    }

    private void HideThumbnails()
    {
        foreach (GameObject thumb in thumbnailsVersionA)
        {
            thumb.SetActive(false);
        }
        foreach (GameObject thumb in thumbnailsVersionB)
        {
            thumb.SetActive(false);
        }
    }

    private void onNextClick()
    {
        thumbSelected++;

        Transform nextElem = thumbParent.GetChild(thumbSelected);
        ShowDocument(nextElem);
    }

    private void onPrevClick()
    {
        thumbSelected--;

        Transform nextElem = thumbParent.GetChild(thumbSelected);
        ShowDocument(nextElem);
    }

    private void UpdateArrows(Transform thumbnailParent, int sibling)
    {
        thumbParent = thumbnailParent;
        thumbSelected = sibling;

        prevBtn.interactable = thumbSelected > 0;
        nextBtn.interactable = thumbSelected < thumbParent.childCount - 1;
    }
}
