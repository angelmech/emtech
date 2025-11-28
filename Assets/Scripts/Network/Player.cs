using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    private Renderer _renderer;
    // We don't need the camera's Transform for calculating horizontal movement direction anymore.
    // private Transform _cameraTransform; 

    [Networked] public int Role { get; set; }

    [SerializeField]
    private float moveSpeed = 3.5f; // Reduced speed

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _renderer = GetComponentInChildren<Renderer>();
        // Find the camera's transform if needed for other things, but not for movement direction.
        // If the FirstPersonCamera script is on a child object, this is fine, but unused for movement here.
        // _cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // 1. Raw input direction
            Vector3 raw = new Vector3(data.direction.x, 0, data.direction.z);

            // Normalize BEFORE transforming
            if (raw.sqrMagnitude > 1f)
                raw.Normalize();

            // The movement direction is relative to the Player's **forward** and **right** vectors, 
            // which are rotated horizontally by the camera script (next section).

            // 2. Calculate movement direction relative to the player's current rotation (yaw)
            Vector3 moveDir =
                transform.forward * raw.z +
                transform.right   * raw.x;

            // 3. Apply movement
            // Use moveSpeed, and multiply by Runner.DeltaTime for fixed update
            _cc.Move(moveDir * moveSpeed * Runner.DeltaTime);
        }
    }

    public override void Spawned()
    {
        if (_renderer != null)
            _renderer.material.color = (Role == 0) ? Color.blue : Color.green;
    }
}