using Fusion;
using UnityEngine;
using System.Threading.Tasks;

public class TherapyNetworkManager : MonoBehaviour
{
    public NetworkPrefabRef playerPrefab; // assign later in inspector
    private NetworkRunner runner;

    async void Awake()
    {
        // Add NetworkRunner component
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        // Start the host automatically
        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = "TherapyRoom",
           // Scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex,
            PlayerCount = 2
        });

        Debug.Log("Host started automatically!");
    }
}