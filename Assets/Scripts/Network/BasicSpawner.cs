using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Canvas mainUI;

    private NetworkRunner _runner;
    private readonly Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();

    private async void Start()
    {
        // Automatically start host on app launch
        await StartGame(GameMode.Host);
    }

    private async System.Threading.Tasks.Task StartGame(GameMode mode)
    {
        if (mainUI != null)
            mainUI.enabled = false;

        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = false; // XR input handled locally

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        await _runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        Debug.Log($"Game started as {mode}");
    }

    // ---------------------------
    // Callbacks
    // ---------------------------
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
            return; // Only server spawns players

        Debug.Log($"Player joined: {player} | Spawning prefab...");

        Transform spawn = GameObject.FindWithTag("Respawn")?.transform;
        Vector3 spawnPos = spawn != null ? spawn.position : Vector3.zero;
        Quaternion spawnRot = spawn != null ? spawn.rotation : Quaternion.identity;

        NetworkObject obj = runner.Spawn(
            playerPrefab,
            spawnPos,
            spawnRot,
            player
        );

        // Assign role: first = host/therapist
        Player playerScript = obj.GetComponent<Player>();
        playerScript.Role = (_spawnedCharacters.Count == 0) ? 0 : 1;

        _spawnedCharacters[player] = obj;

        Debug.Log($"Spawned player prefab at {spawnPos}. Role: {(playerScript.Role == 0 ? "Therapist" : "Patient")}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject obj))
        {
            runner.Despawn(obj);
            _spawnedCharacters.Remove(player);
        }
    }

    // ---------------------------
    // Unused callbacks
    // ---------------------------
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
