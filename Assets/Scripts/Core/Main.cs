using Firesplash.UnityAssets.SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Main : MonoBehaviour
{
    private const string RESOURCES_PATH = "Core/";

    private const string SOCKETIO_MANAGER = "SocketIOManager";
    private const string SHARED_CANVAS = "SharedCanvas";
    private const string SOUND_MANAGER = "SoundManager";
    private const string TIMER_MANAGER = "TimerManager";

    public static SocketIOCommunicator SocketIOManager { get; private set; }
    public static SharedCanvas SharedCanvas { get; private set; }
    public static SoundManager SoundManager { get; private set; }
    public static UserConnectedManager UserConnectedManager { get; private set; }
    public static TimerManager TimerManager { get; private set; }


    /// <summary>
    /// Charge les managers avant qu'une scène ne soit chargé. Ces managers seront disponibles pour toutes les scènes.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadManagers()
    {
        // Gestion des sockets
        GameObject socketManagerGo = Instantiate(Resources.Load<GameObject>(RESOURCES_PATH + SOCKETIO_MANAGER));
        socketManagerGo.name = SOCKETIO_MANAGER;
        SocketIOManager = socketManagerGo.GetComponent<SocketIOCommunicator>();

        DontDestroyOnLoad(socketManagerGo);

        // Gestion du canvas partagé
        GameObject sharedCanvasGo = Instantiate(Resources.Load<GameObject>(RESOURCES_PATH + SHARED_CANVAS));
        sharedCanvasGo.name = SHARED_CANVAS;
        SharedCanvas = sharedCanvasGo.GetComponent<SharedCanvas>();
        UserConnectedManager = sharedCanvasGo.GetComponent<UserConnectedManager>();

        DontDestroyOnLoad(sharedCanvasGo);

        // Gestion des sons
        GameObject soundManagerGo = Instantiate(Resources.Load<GameObject>(RESOURCES_PATH + SOUND_MANAGER));
        soundManagerGo.name = SOUND_MANAGER;
        SoundManager = soundManagerGo.GetComponent<SoundManager>();

        DontDestroyOnLoad(soundManagerGo);

        // Gestion du timer
        GameObject timerManagerGo = Instantiate(Resources.Load<GameObject>(RESOURCES_PATH + TIMER_MANAGER));
        timerManagerGo.name = TIMER_MANAGER;
        TimerManager = timerManagerGo.GetComponent<TimerManager>();

        DontDestroyOnLoad(timerManagerGo);
    }
}
