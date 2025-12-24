using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public int Role { get; set; }

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }

    public override void Spawned()
    {
        // Color based on role (host vs client)
        if (_renderer != null)
        {
            _renderer.material.color =
                (Role == 0) ? Color.blue : Color.green;
        }

        // Optional: enable XR only for the local player
        if (Object.HasInputAuthority == false)
        {
            DisableLocalXR();
        }
    }

    private void DisableLocalXR()
    {
        // Disable camera + XR input on remote players
        var cameras = GetComponentsInChildren<Camera>();
        foreach (var cam in cameras)
            cam.enabled = false;
    }
}