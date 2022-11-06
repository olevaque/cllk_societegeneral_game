using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Document : MonoBehaviour
{
    public void Open(string docName)
    {
        Debug.Log("Open: " + docName);
        Main.SocketIOManager.Instance.Emit("requestOpenDocument");
    }

    public void Close(string docName)
    {
        Debug.Log("Close: " + docName);
        Main.SocketIOManager.Instance.Emit("requestCloseDocument");
    }
}
