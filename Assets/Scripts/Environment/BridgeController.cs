using UnityEngine;

public class BridgeController : MonoBehaviour
{
    public static BridgeController Instance;
    
    public Transform bridgeTransform;
    public Transform spawnerTransform;
    
    void Awake()
    {
        // Brücke registriert sich selbst
        Instance = this;
    }
    
    public void UpdateHeight(float value)
    {
        float targetY = 0f;

        switch (Mathf.RoundToInt(value))
        {
            case 1: targetY = 30f; break;
            case 2: targetY = 45f; break;
            case 3: targetY = 60f; break; 
        }

        Vector3 pos = bridgeTransform.localPosition;
        bridgeTransform.localPosition = new Vector3(pos.x, targetY, pos.z);
    }
}
