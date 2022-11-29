using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Document : MonoBehaviour
{
    public void Open(string document)
    {
        WGCC_OpenCloseDocument ocData = new WGCC_OpenCloseDocument()
        {
            docName = document
        };
        Main.SocketIOManager.Instance.Emit("WGCC_OpenDocument", JsonUtility.ToJson(ocData), false);
    }

    public void Close(string document)
    {
        WGCC_OpenCloseDocument ocData = new WGCC_OpenCloseDocument()
        {
            docName = document
        };
        Main.SocketIOManager.Instance.Emit("WGCC_CloseDocument", JsonUtility.ToJson(ocData), false);
    }
}
