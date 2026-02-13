using UnityEngine;

namespace Network
{
    /// <summary>
    /// Simple script to make the body follow the head in VR.
    /// </summary>
    public class VRBodyFollow : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform head;

        [Header("Settings")] [SerializeField] private float headToBodyDistance = 1.5f;
        [SerializeField] private bool followRotation = true;

        void LateUpdate()
        {
            if (head == null)
            {
                Debug.LogWarning("[VRBodyFollow] Head reference is missing!");
                return;
            }

            // Body position is head position minus a fixed distance on the Y axis
            Vector3 targetPos = head.position;
            targetPos.y -= headToBodyDistance;

            transform.position = targetPos;

            if (followRotation)
            {
                Vector3 headForward = head.forward;
                headForward.y = 0;
                if (headForward != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(headForward);
                }
            }
        }
    }
}