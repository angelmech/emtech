using Fusion;
using UnityEngine;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour
{
    public NetworkPrefabRef playerPrefab;

    private NetworkRunner runner;

    public async Task StartHost()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        var callbacks = gameObject.AddComponent<Callbacks>();
        callbacks.playerPrefab = playerPrefab;
        runner.AddCallbacks(callbacks);

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = "TherapyRoom",
            PlayerCount = 2
        });

        Debug.Log("HOST STARTED");
    }

    public async Task JoinSession()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        var callbacks = gameObject.AddComponent<Callbacks>();
        callbacks.playerPrefab = playerPrefab;
        runner.AddCallbacks(callbacks);

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = "TherapyRoom"
        });

        Debug.Log("CLIENT JOINED");
    }
}