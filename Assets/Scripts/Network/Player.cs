using Fusion;
using UnityEngine;
using System.Linq;

namespace Network
{
    public class Player : NetworkBehaviour
    {
        [Networked] public NetworkBool IsTherapist { get; set; }

        [SerializeField] private GameObject bodyMesh;
        private MeshRenderer _renderer;

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

        public override void FixedUpdateNetwork()
        {
            if (Object.HasInputAuthority) return;
            UpdateVisuals();
        }

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