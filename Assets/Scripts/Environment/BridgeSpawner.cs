using Fusion;
using UnityEngine;

public class BridgeSpawner : NetworkBehaviour
{
    public static BridgeSpawner Instance;
    
    public Transform spawnerTransform;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void TriggerAllPlayerTeleport()
    {
        if (Object == null || !Object.HasStateAuthority)
        {
            Debug.LogWarning("Cannot trigger teleport: no state authority");
            return;
        }
        
        RPC_TeleportAll();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TeleportAll()
    {
        // Find all NetworkObjects
        NetworkObject[] allNetworkObjects = FindObjectsByType<NetworkObject>(
            FindObjectsSortMode.None
        );
        
        foreach (NetworkObject obj in allNetworkObjects)
        {
            // Only teleport the local player
            if (obj.HasInputAuthority)
            {
                Debug.Log($"Teleporting local player: {obj.name}");
                
                // Find XR Origin
                Transform xrOrigin = obj.transform.Find("XR Origin (VR)");
                
                if (xrOrigin != null)
                {
                    // Disable CharacterController if present
                    CharacterController cc = xrOrigin.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;
                    
                    // Teleport
                    xrOrigin.position = spawnerTransform.position;
                    xrOrigin.rotation = spawnerTransform.rotation;
                    
                    // Re-enable CharacterController
                    if (cc != null) cc.enabled = true;
                    
                    Debug.Log($"Teleported to {spawnerTransform.position}");
                }
                else
                {
                    Debug.LogError("XR Origin (VR) not found as child of player.");
                }
                
                // Reset velocity
                Rigidbody rb = obj.GetComponentInChildren<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                
                return;
            }
        }
        
        Debug.LogWarning("No local player found!");
    }
}
