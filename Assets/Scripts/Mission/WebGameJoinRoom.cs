
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WebGameJoinRoom : MonoBehaviour
{
    private const string CONNECT = "connect";
    private const string DISCONNECT = "disconnect";
    private const string JOIN_SESSION = "joinSession";
    private const string INFO_SESSION = "infoSession";

    [SerializeField] private Button joinBtn;
    [SerializeField] private TMP_InputField firstnameIpt, initialNameIpt;
    [SerializeField] private TextMeshProUGUI messageTxt;

    private string sessionUUID;

    private void Awake()
    {
        messageTxt.text = string.Empty;
        joinBtn.interactable = false;
    }

    private void OnEnable()
    {
        joinBtn.onClick.AddListener(OnJoinClick);
    }

    private void OnDisable()
    {
        joinBtn.onClick.RemoveListener(OnJoinClick);
    }

    private void Start()
    {
        Main.SocketIOManager.Instance.On(CONNECT, (string data) =>
        {
            joinBtn.interactable = true;
        });

        Main.SocketIOManager.Instance.On(INFO_SESSION, (string data) =>
        {
            InfoSessionData infoSession = JsonUtility.FromJson<InfoSessionData>(data);
            if (infoSession.status == "OK")
            {
                SceneManager.LoadScene(infoSession.currentScene);
            }
            else
            {
                messageTxt.text = infoSession.info;
            }
        });

        Main.SocketIOManager.Instance.On(DISCONNECT, (string payload) =>
        {
            joinBtn.gameObject.SetActive(false);
            if (payload.Equals("io server disconnect"))
            {
                Debug.LogWarning("Disconnected from server.");
            }
            else
            {
                Debug.LogWarning("We have been unexpecteldy disconnected. This will cause an automatic reconnect. Reason: " + payload);
            }
        });

#if UNITY_EDITOR
        OnSendMessageReceived("ab704241-eaf6-4857-a815-9d2801e3c48e");
#endif
    }

    private void OnDestroy()
    {
        Main.SocketIOManager.Instance.Off(CONNECT);
        Main.SocketIOManager.Instance.Off(INFO_SESSION);
        Main.SocketIOManager.Instance.Off(DISCONNECT);
    }

    public void OnSendMessageReceived(string uuidWithLg)
    {
        sessionUUID = uuidWithLg;
    }

    private void OnJoinClick()
    {
        if (firstnameIpt.text != string.Empty && initialNameIpt.text != string.Empty)
        {
            WGJR_Data wgjrData = new WGJR_Data()
            {
                uuid = sessionUUID,
                firstname = firstnameIpt.text,
                initialName = initialNameIpt.text
            };

            Main.SocketIOManager.Instance.Emit(JOIN_SESSION, JsonUtility.ToJson(wgjrData), false);
        }
        else
        {
            messageTxt.text = "The firstname and initial are required";
        }
    }
}
