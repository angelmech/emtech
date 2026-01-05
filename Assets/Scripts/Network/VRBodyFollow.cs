using UnityEngine;

public class VRBodyFollow : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float headToBodyDistance = 1.5f;
    [SerializeField] private bool followRotation = true;
    
    void LateUpdate()
    {
        if (head == null) return;

        // Body soll immer headToBodyDistance unter dem Kopf sein
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