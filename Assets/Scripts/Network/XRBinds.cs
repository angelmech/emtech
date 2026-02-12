using Fusion;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// This component is responsible for binding the XR camera and controllers to the networked player object.
    /// It ensures that the local player's XR rig follows their actual headset and controllers,
    /// while remote players will just follow the assigned transforms (which will be updated by the local player's movements).
    /// </summary>
    public class XRBinds : NetworkBehaviour
    {
        [SerializeField] private NetworkFollow headFollow;
        [SerializeField] private NetworkFollow leftHandFollow;
        [SerializeField] private NetworkFollow rightHandFollow;

        [SerializeField] private XROrigin xrOrigin;

        public override void Spawned()
        {
            Debug.Log($"[XRBinds] Spawned - HasInputAuthority: {Object.HasInputAuthority}");

            xrOrigin = FindObjectOfType<XROrigin>();

            if (xrOrigin == null)
            {
                Debug.LogError("[XRBinds] XR Origin not found in scene!");
                return;
            }

            if (headFollow == null || leftHandFollow == null || rightHandFollow == null)
            {
                Debug.LogError("[XRBinds] Some follow components not assigned!");
                return;
            }

            var camera = xrOrigin.Camera?.transform;

            if (camera == null)
            {
                Debug.LogError("[XRBinds] Camera not found!");
                return;
            }

            // find controllers with multiple possible paths to support different XR setups
            Transform leftHand = FindController("Left");
            Transform rightHand = FindController("Right");

            Debug.Log(
                $"[XRBinds] Camera: {camera.name}, Left: {leftHand?.name ?? "NULL"}, Right: {rightHand?.name ?? "NULL"}");

            if (!Object.HasInputAuthority)
            {
                Debug.Log("[XRBinds] Remote player - skipping XR setup");
                return;
            }

            Debug.Log("[XRBinds] Local player - setting up XR bindings");

            // set targets for local player only, remote players will just follow the assigned transforms
            headFollow.SetXRTarget(camera);

            if (leftHand != null)
                leftHandFollow.SetXRTarget(leftHand);
            else
                Debug.LogWarning("[XRBinds] Left controller not found!");

            if (rightHand != null)
                rightHandFollow.SetXRTarget(rightHand);
            else
                Debug.LogWarning("[XRBinds] Right controller not found!");

            Debug.Log($"[XRBinds] All targets set!");

            camera.GetComponent<Camera>().tag = "MainCamera";
        }

        private Transform FindController(string hand)
        {

            if (xrOrigin == null) return null;

            string[] possiblePaths = new string[]
            {
                $"Camera Offset/{hand} Controller",
                $"Camera Offset/{hand}Hand Controller",
                $"Camera Offset/{hand} Hand",
                $"Camera Offset/XR {hand} Controller",
                $"{hand} Controller",
                $"{hand}Hand Controller"
            };

            foreach (string path in possiblePaths)
            {
                Transform controller = xrOrigin.transform.Find(path);
                if (controller != null)
                {
                    Debug.Log($"[XRBinds] Found {hand} controller at path: {path}");
                    return controller;
                }
            }

            Debug.Log($"[XRBinds] Searching for {hand} controller in all children...");
            foreach (Transform child in xrOrigin.GetComponentsInChildren<Transform>())
            {
                string childNameLower = child.name.ToLower();
                string handLower = hand.ToLower();

                if (childNameLower.Contains(handLower) &&
                    (childNameLower.Contains("controller") || childNameLower.Contains("hand")))
                {
                    Debug.Log($"[XRBinds] Found {hand} controller in children: {child.name}");
                    return child;
                }
            }

            Debug.LogError($"[XRBinds] Could not find {hand} controller!");
            return null;
        }
    }
}