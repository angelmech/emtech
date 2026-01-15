using UnityEngine;

public class VRBodyFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform head;
    
    [Header("Settings")]
    [SerializeField] private float headToBodyDistance = 1.5f;
    [SerializeField] private bool followRotation = true;
    
    void LateUpdate()
    {
        if (head == null) 
        {
            Debug.LogWarning("[VRBodyFollow] Head reference is missing!");
            return;
        }

        // Body soll headToBodyDistance unter dem Kopf sein
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