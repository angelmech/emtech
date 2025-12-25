using Fusion;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRBinds : NetworkBehaviour
{
    public NetworkFollow headFollow;
    public NetworkFollow leftHandFollow;
    public NetworkFollow rightHandFollow;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
            return;

        // Find scene XR Origin
        var xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("No XR Origin found in scene!");
            return;
        }

        var camera = xrOrigin.Camera.transform;
        var leftHand = xrOrigin.transform.Find("Camera Offset/Left Controller");
        var rightHand = xrOrigin.transform.Find("Camera Offset/Right Controller");

        headFollow.SetXRTarget(camera);
        leftHandFollow.SetXRTarget(leftHand);
        rightHandFollow.SetXRTarget(rightHand);
    }
}