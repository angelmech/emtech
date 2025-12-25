using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public int Role { get; set; }

    [SerializeField] private GameObject bodyMesh;

    public override void Spawned()
    {
        // Hide own body
        if (Object.HasInputAuthority && bodyMesh != null)
            bodyMesh.SetActive(false);

        // Color visible body for others
        if (!Object.HasInputAuthority && bodyMesh != null)
        {
            var r = bodyMesh.GetComponent<Renderer>();
            if (r != null)
                r.material.color = (Role == 0) ? Color.blue : Color.green;
        }
    }
}