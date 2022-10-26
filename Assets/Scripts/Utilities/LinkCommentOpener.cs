using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

[RequireComponent(typeof(TMP_Text))]
public class LinkCommentOpener : MonoBehaviour, IPointerClickHandler
{
    [DllImport("__Internal")]
    private static extern void OpenCommentLink(string url);

    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text> ();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, null);  // If you are not in a Canvas using Screen Overlay, put your camera instead of null
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            OpenCommentLink(linkInfo.GetLinkText());
        }
    }
}