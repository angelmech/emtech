using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    public float speed = 3f;
    public string Role; // "Therapist" or "Patient"

    void Update()
    {
        if (!Object.HasInputAuthority)
            return;

        Vector2 move = Vector2.zero;

        // Keyboard movement
        if (Keyboard.current != null)
        {
            move.x = (Keyboard.current.dKey.isPressed ? 1 : 0) + (Keyboard.current.aKey.isPressed ? -1 : 0);
            move.y = (Keyboard.current.wKey.isPressed ? 1 : 0) + (Keyboard.current.sKey.isPressed ? -1 : 0);
        }

        // Apply movement
        transform.Translate(new Vector3(move.x, 0, move.y) * speed * Time.deltaTime, Space.World);

    }
}