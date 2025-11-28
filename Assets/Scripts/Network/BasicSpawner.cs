using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef playerPrefab;

    private NetworkRunner _runner;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();

    // ---------------------------
    // Start Host or Client
    // ---------------------------
    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    // ---------------------------
    // Simple UI buttons
    // ---------------------------
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(20, 20, 200, 40), "Host"))
                StartGame(GameMode.Host);

            if (GUI.Button(new Rect(20, 70, 200, 40), "Join"))
                StartGame(GameMode.Client);
        }
    }

    // ---------------------------
    // Callbacks
    // ---------------------------
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPos = new Vector3(player.RawEncoded * 2, 1, 0);

            NetworkObject obj = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
            
            // Assign role: host = Therapist (0), others = Patient (1)
            var playerScript = obj.GetComponent<Player>();
            if (_spawnedCharacters.Count == 0)
                playerScript.Role = 0; // Therapist
            else
                playerScript.Role = 1; // Patient

            _spawnedCharacters[player] = obj;
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject o))
        {
            runner.Despawn(o);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.wKey.isPressed) data.direction.z += 1f;
        if (keyboard.sKey.isPressed) data.direction.z -= 1f;
        if (keyboard.aKey.isPressed) data.direction.x -= 1f;
        if (keyboard.dKey.isPressed) data.direction.x += 1f;


        input.Set(data);
    }


    // unused callbacks
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
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
