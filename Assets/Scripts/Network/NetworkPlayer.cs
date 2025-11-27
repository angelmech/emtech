using Fusion;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public NetworkPrefabRef playerPrefab;
    private NetworkRunner runner;

    async void Awake()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        // Add callbacks
        var callbacks = gameObject.AddComponent<Callbacks>();
        callbacks.playerPrefab = playerPrefab;
        runner.AddCallbacks(callbacks);

        // Start host without specifying Scene (current scene used)
        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = "TherapyRoom",
            PlayerCount = 2
        });

        Debug.Log("HOST STARTED");
    }
}