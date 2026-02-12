using Fusion;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// Makes the GameObject follow a target Transform (e.g. XR Rig) and synchronizes it across the network.
    /// </summary>
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

        public override void FixedUpdateNetwork()
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
}