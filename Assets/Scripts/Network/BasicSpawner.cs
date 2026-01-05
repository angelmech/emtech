using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkPrefabRef playerPrefab;

        [Header("Role Spawn Points")]
        [SerializeField] private Transform therapistSpawnPoint;
        [SerializeField] private Transform patientSpawnPoint;

        private NetworkRunner _runner;

        private async void Start()
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.AddCallbacks(this);

            StartGameArgs args = new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = "VRRoom",
                Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            await _runner.StartGame(args);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (player != runner.LocalPlayer)
                return;

            // Kleinste PlayerId = Therapist
            bool isTherapist = player.PlayerId == GetSmallestPlayerId(runner);

            Transform spawn = isTherapist
                ? therapistSpawnPoint
                : patientSpawnPoint;

            Vector3 pos = spawn ? spawn.position : Vector3.zero;
            Quaternion rot = spawn ? spawn.rotation : Quaternion.identity;

            runner.Spawn(playerPrefab, pos, rot, player);
        }

        private int GetSmallestPlayerId(NetworkRunner runner)
        {
            int smallest = int.MaxValue;
            foreach (var p in runner.ActivePlayers)
                if (p.PlayerId < smallest)
                    smallest = p.PlayerId;
            return smallest;
        }

        // --- unused callbacks ---
        public void OnPlayerLeft(NetworkRunner r, PlayerRef p) { }
        public void OnInput(NetworkRunner r, NetworkInput i) { }
        public void OnShutdown(NetworkRunner r, ShutdownReason s) { }
        public void OnConnectedToServer(NetworkRunner r) { }
        public void OnDisconnectedFromServer(NetworkRunner r, NetDisconnectReason e) { }
        public void OnConnectRequest(NetworkRunner r, NetworkRunnerCallbackArgs.ConnectRequest q, byte[] t) { }
        public void OnConnectFailed(NetworkRunner r, NetAddress a, NetConnectFailedReason r2) { }
        public void OnSessionListUpdated(NetworkRunner r, List<SessionInfo> l) { }
        public void OnCustomAuthenticationResponse(NetworkRunner r, Dictionary<string, object> d) { }
        public void OnHostMigration(NetworkRunner r, HostMigrationToken t) { }
        public void OnInputMissing(NetworkRunner r, PlayerRef p, NetworkInput i) { }
        public void OnObjectEnterAOI(NetworkRunner r, NetworkObject o, PlayerRef p) { }
        public void OnObjectExitAOI(NetworkRunner r, NetworkObject o, PlayerRef p) { }
        public void OnReliableDataProgress(NetworkRunner r, PlayerRef p, ReliableKey k, float pr) { }
        public void OnReliableDataReceived(NetworkRunner r, PlayerRef p, ReliableKey k, System.ArraySegment<byte> d) { }
        public void OnSceneLoadDone(NetworkRunner r) { }
        public void OnSceneLoadStart(NetworkRunner r) { }
        public void OnUserSimulationMessage(NetworkRunner r, SimulationMessagePtr m) { }
    }
}
