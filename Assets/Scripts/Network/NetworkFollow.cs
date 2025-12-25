using Fusion;
using UnityEngine;

public class NetworkFollow : NetworkBehaviour
{
    private Transform xrTarget;

    public void SetXRTarget(Transform target)
    {
        xrTarget = target;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;
        if (xrTarget == null) return;

        transform.SetPositionAndRotation(
            xrTarget.position,
            xrTarget.rotation
        );
    }
}