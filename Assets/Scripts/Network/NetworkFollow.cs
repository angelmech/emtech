using Fusion;
using UnityEngine;

public class NetworkFollow : NetworkBehaviour
{
    private Transform xrTarget;
    private bool hasTarget = false;

    public void SetXRTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogError($"[NetworkFollow] SetXRTarget called with NULL on {gameObject.name}!");
            return;
        }

        xrTarget = target;
        hasTarget = true;
        Debug.Log($"[NetworkFollow] {gameObject.name} now following {target.name}");
    }

    void LateUpdate()
    {
        // Nur für lokalen Spieler
        if (!Object || !Object.HasInputAuthority) return;
        if (!hasTarget || xrTarget == null) return;

        // Setze Transform - NetworkTransform synchronisiert automatisch
        transform.SetPositionAndRotation(
            xrTarget.position,
            xrTarget.rotation
        );
    }
}