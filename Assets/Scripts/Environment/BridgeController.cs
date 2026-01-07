using UnityEngine;
using Fusion;

public class BridgeController : NetworkBehaviour
{
    public static BridgeController Instance;
    
    public Transform bridgeTransform;
    public Transform spawnerTransform;
    
    // Variable wird autamtisch über Netzwerk synchronisiert
    [Networked, OnChangedRender(nameof(OnHeightChanged))]
    public float NetworkedHeightStep { get; set; }
    
    void Awake()
    {
        // Brücke registriert sich selbst
        Instance = this;
    }
    
    public void UpdateHeight(float step)
    {
        if (HasStateAuthority)
        {
            NetworkedHeightStep = step;
        }
        else
        {
            RPC_RequestHeightChange(step);
        }
    }
    
    public void RPC_RequestHeightChange(float step)
    {
        NetworkedHeightStep = step;
    }
    
    // callback
    void OnHeightChanged()
    {
        ApplyHeight(NetworkedHeightStep);
    }

    private void ApplyHeight(float step)
    {
        float targetY = 0f;

        switch (Mathf.RoundToInt(step))
        {
            case 1: targetY = 30f; break;
            case 2: targetY = 45f; break;
            case 3: targetY = 60f; break; 
        }

        Vector3 pos = bridgeTransform.localPosition;
        bridgeTransform.localPosition = new Vector3(pos.x, targetY, pos.z);
    }
}
