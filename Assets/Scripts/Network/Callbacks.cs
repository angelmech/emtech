using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Callbacks : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPrefabRef playerPrefab;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Decide spawn position
        Vector3 spawnPos = new Vector3(player.RawEncoded * 2f, 0, 0);

        // Determine role
        string role = runner.IsServer && player == runner.LocalPlayer ? "Therapist" : "Patient";

        var playerObject = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
        playerObject.GetComponent<NetworkPlayer>().Role = role;

        Debug.Log($"Player {player} joined as {role}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}
    public void OnInput(NetworkRunner runner, NetworkInput input) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) {}
    public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnSceneLoadDone(NetworkRunner runner) {}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) {}
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) {}
}
