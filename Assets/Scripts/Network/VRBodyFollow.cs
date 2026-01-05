using UnityEngine;

public class VRBodyFollow : MonoBehaviour
{
    [SerializeField] private string headFollowName = "NetworkHead"; // ← ZURÜCK!
    [SerializeField] private float bodyHeight = 1f;
    [SerializeField] private bool followRotation = true;

    private Transform headTransform;
    private bool foundHead = false;
    private float searchTimer = 0f;

    void Start()
    {
        Debug.Log($"[VRBodyFollow] Start called on {gameObject.name}, searching for '{headFollowName}'");
    }

    void FindHeadTransform()
    {
        // Suche im Parent nach dem Head
        Transform parent = transform.parent;
        if (parent == null)
        {
            Debug.LogWarning("[VRBodyFollow] No parent found!");
            return;
        }

        // Suche direkt im Parent nach dem Namen
        foreach (Transform child in parent)
        {
            if (child.name == headFollowName)
            {
                headTransform = child;
                foundHead = true;
                Debug.Log($"[VRBodyFollow] ✅ Found {headFollowName}: {headTransform.name} at {headTransform.position}");
                return;
            }
        }

        // Debug: Liste alle Siblings (nur alle 2 Sekunden)
        if (Time.time - searchTimer > 2f)
        {
            searchTimer = Time.time;
            Debug.LogWarning($"[VRBodyFollow] '{headFollowName}' not found. Available siblings in {parent.name}:");
            foreach (Transform child in parent)
            {
                Debug.Log($"  - {child.name}");
            }
        }
    }

    void LateUpdate()
    {
        if (!foundHead)
        {
            FindHeadTransform();
            return;
        }

        if (headTransform == null)
        {
            foundHead = false;
            return;
        }

        // Folge dem NetworkHead
        Vector3 targetPos = new Vector3(
            headTransform.position.x,
            bodyHeight,
            headTransform.position.z
        );

        transform.position = targetPos;

        if (followRotation)
        {
            Vector3 headForward = headTransform.forward;
            headForward.y = 0;
            if (headForward != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(headForward);
            }
        }
    }
}