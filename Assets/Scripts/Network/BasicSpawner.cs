using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Canvas mainUI;

    private NetworkRunner runner;

    // Local counter to track first player
    private static int localPlayerCount = 0;

    private async void Start()
    {
        if (mainUI != null)
            mainUI.enabled = false;

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = false; // XR input handled locally

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "VRRoom",
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Spawn only for the local player
        if (player != runner.LocalPlayer) return;

        Transform spawn = GameObject.FindWithTag("Respawn")?.transform;

        NetworkObject obj = runner.Spawn(
            playerPrefab,
            spawn ? spawn.position : Vector3.zero,
            spawn ? spawn.rotation : Quaternion.identity,
            player
        );

        // Assign role: first local player = Therapist, others = Patient
        Player playerScript = obj.GetComponent<Player>();
        playerScript.Role = (localPlayerCount == 0) ? 0 : 1;

        localPlayerCount++;

        Debug.Log($"Spawned local player. Role: {(playerScript.Role == 0 ? "Therapist" : "Patient")}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    // Unused callbacks
    public void OnInput(NetworkRunner r, NetworkInput i) { }
    public void OnShutdown(NetworkRunner r, ShutdownReason s) { }
    public void OnConnectedToServer(NetworkRunner r) { }
    public void OnDisconnectedFromServer(NetworkRunner r, NetDisconnectReason e) { }
    public void OnConnectRequest(NetworkRunner r, NetworkRunnerCallbackArgs.ConnectRequest q, byte[] t) { }
    public void OnConnectFailed(NetworkRunner r, NetAddress a, NetConnectFailedReason r2) { }
    public void OnSessionListUpdated(NetworkRunner r, List<SessionInfo> l) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
