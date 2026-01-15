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
        // Finde das XR Origin in der Szene
        Unity.XR.CoreUtils.XROrigin xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        
        if (xrOrigin == null)
        {
            Debug.LogError("XR Origin not found in scene!");
            return;
        }
        
        Debug.Log($"Teleporting XR Origin to {spawnerTransform.position}");
        
        // Disable CharacterController if present
        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        
        // Teleport
        xrOrigin.transform.position = spawnerTransform.position;
        xrOrigin.transform.rotation = spawnerTransform.rotation;
        
        // Re-enable CharacterController
        if (cc != null) cc.enabled = true;
        
        // Reset velocity on all Rigidbodies
        Rigidbody[] rbs = xrOrigin.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        Debug.Log("Teleport complete");
    }
}
