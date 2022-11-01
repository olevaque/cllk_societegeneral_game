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
    [SerializeField] private GameObject docZoomed;
    [SerializeField] private Transform docZoomedCnt;
    [SerializeField] private Button closeZoomBtn, backBtn;

    private bool[] unlockedFolders = new bool[NB_FOLDER];

    private CanvasGroup docsCg;

    private void Awake()
    {
        docsCg = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        for (int i=0; i<NB_FOLDER; i++)
        {
            int closureIndex = i;
            folderBtns[i].onClick.AddListener(() => OnFolderClick(closureIndex));
        }

        backBtn.onClick.AddListener(OnBackClick);
        closeZoomBtn.onClick.AddListener(OnCloseZoomClick);
    }

    private void OnDisable()
    {
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

    public void ShowZoomedDoc(GameObject docTarget)
    {
        docZoomed.SetActive(true);
        docTarget.SetActive(true);
    }
    private void HideZoomedDoc()
    {
        foreach(Transform child in docZoomedCnt)
        {
            child.gameObject.SetActive(false);
        }
        docZoomed.SetActive(false);
    }

    private void OnBackClick()
    {
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
}
