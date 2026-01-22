using Fusion;
using UnityEngine;

public class BridgeSpawner : NetworkBehaviour
{
    public static BridgeSpawner Instance;
    
    public Transform spawnerTransformBridge;
    public Transform spawnerTransformField;

    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void TriggerAllPlayerTeleportBridge()
    {
        if (Object == null || !Object.HasStateAuthority)
        {
            Debug.LogWarning("Cannot trigger teleport: no state authority");
            return;
        }
        
        RPC_TeleportAll(spawnerTransformBridge.position, spawnerTransformBridge.rotation);
    }
    
    public void TriggerAllPlayerTeleportField()
    {
        if (Object == null || !Object.HasStateAuthority)
        {
            Debug.LogWarning("Cannot trigger teleport: no state authority");
            return;
        }
        
        RPC_TeleportAll(spawnerTransformField.position, spawnerTransformField.rotation);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TeleportAll(Vector3 targetPos, Quaternion targetRot)
    {
        // Finde das XR Origin in der Szene
        Unity.XR.CoreUtils.XROrigin xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        
        if (xrOrigin == null)
        {
            Debug.LogError("XR Origin not found in scene!");
            return;
        }
        
        Debug.Log($"Teleporting XR Origin to {targetPos}");
        
        // Disable CharacterController if present
        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        
        // Teleport
        xrOrigin.transform.position = targetPos;
        xrOrigin.transform.rotation = targetRot;
        
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
