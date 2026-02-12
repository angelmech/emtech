using Fusion;
using UnityEngine;
using System.Linq;

namespace Network
{
    /// <summary>
    /// Represents a player in the networked VR environment. Each player is assigned a role (therapist or patient) based on their PlayerId,
    /// with the player having the smallest PlayerId becoming the therapist.
    /// The player's visual representation is updated to reflect their role, and local players do not see their own body mesh to avoid visual clutter
    /// </summary>
    public class Player : NetworkBehaviour
    {
        [Networked] public NetworkBool IsTherapist { get; set; }

        [SerializeField] private GameObject bodyMesh;
        private MeshRenderer _renderer;

        // Set role and update visuals on spawn
        public override void Spawned()
        {
            _renderer = bodyMesh.GetComponent<MeshRenderer>();

            if (Object.HasStateAuthority)
            {
                int smallestId = Runner.ActivePlayers.Min(p => p.PlayerId);
                IsTherapist = Object.InputAuthority.PlayerId == smallestId;
            }

            UpdateVisuals();
        }

        // Update visuals for remote players
        public override void FixedUpdateNetwork()
        {
            if (Object.HasInputAuthority) return;
            UpdateVisuals();
        }

        // sets the color based on role and hides for local player
        private void UpdateVisuals()
        {
            if (_renderer == null) return;

            if (Object.HasInputAuthority)
            {
                _renderer.enabled = false;
                return;
            }

            _renderer.enabled = true;
            _renderer.material.color =
                IsTherapist ? Color.blue : Color.green;
        }
    }
}