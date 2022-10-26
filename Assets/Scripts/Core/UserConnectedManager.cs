using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

public class UserConnectedManager : MonoBehaviour
{
    public Player[] ConnectedPlayers
    {
        get
        {
            return connectedPlayers;
        }
    }

    public event Action<Player[]> OnPlayersChanged;

    private const float ANIM_DURATION = .35f;

    [Header("UsersConnected")]
    [SerializeField] private GameObject prefabUser;
    [SerializeField] private GameObject userConnected;
    [SerializeField] private RectTransform userConnectedRedArrow;
    [SerializeField] private Transform containerPlayers;

    private Player[] connectedPlayers = new Player[0];

    //private Button userConnectedBtn;
    private RectTransform userConnectedRect;
    private bool isExtract = true;

    private void Awake()
    {
        //userConnectedBtn = userConnected.GetComponent<Button>();
        userConnectedRect = userConnected.GetComponent<RectTransform>();
    }

    private void Start()
    {
        Main.SocketIOManager.Instance.On("connectedPlayers", (string data) =>
        {
            CleanContainer();

            ConnectedPlayersData cPlayers = JsonUtility.FromJson<ConnectedPlayersData>(data);
            foreach (Player player in cPlayers.players)
            {
                GameObject playerGo = Instantiate(prefabUser, containerPlayers);
                playerGo.GetComponent<TextMeshProUGUI>().text = player.pseudo;
            }

            connectedPlayers = cPlayers.players;

            OnPlayersChanged?.Invoke(cPlayers.players);
        });
    }

    private void OnEnable()
    {
        //userConnectedBtn.onClick.AddListener(ExtractRetractUserPanel);
    }

    private void OnDisable()
    {
        //userConnectedBtn.onClick.RemoveListener(ExtractRetractUserPanel);
    }

    private void ExtractRetractUserPanel()
    {
        userConnectedRect.DOKill();
        userConnectedRedArrow.DOKill();
        userConnectedRedArrow.DORotate(isExtract ? new Vector3(0, 0, 180) : Vector3.zero, ANIM_DURATION).SetEase(Ease.InOutSine);
        userConnectedRect.DOAnchorPos(isExtract ? new Vector2(-210, 0) : Vector2.zero, ANIM_DURATION).SetEase(Ease.InOutSine).OnComplete(() => isExtract = !isExtract);
    }

    private void CleanContainer()
    {
        foreach (Transform child in containerPlayers)
        {
            Destroy(child.gameObject);
        }
    }
}
