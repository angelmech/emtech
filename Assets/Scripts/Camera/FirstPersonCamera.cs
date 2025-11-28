using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    // The sensitivity should be higher since we are removing Time.deltaTime
    public float mouseSensitivity = 150f; 

    private float xRotation = 0f; // Stores pitch (vertical) rotation

    private void Start()
    {
        // This should only execute for the *local* player's camera
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // We use Update for input processing
        if (Mouse.current == null) return;

        // Get raw delta values
        float mouseX = Mouse.current.delta.x.ReadValue();
        float mouseY = Mouse.current.delta.y.ReadValue();

        // 1. Apply Horizontal Rotation (Yaw) to the Player's Body/Root (this object's parent)
        // This rotates the Player's forward/right vectors for movement calculation in Player.cs
        // Note: We multiply by Time.deltaTime here for framerate-independent rotation
        transform.parent.Rotate(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);

        // 2. Apply Vertical Rotation (Pitch) to the Camera Object (this object)
        xRotation -= mouseY * mouseSensitivity * Time.deltaTime; // Use Time.deltaTime here
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Set the camera's local pitch rotation
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}