using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject uiCanvas;
    public InputActionReference toggleReference;

    private void OnEnable()
    {
        if (toggleReference?.action != null)
        {
            toggleReference.action.performed += ToggleUI;
        }
    }

    private void OnDisable()
    {
        if (toggleReference?.action != null)
        {
            toggleReference.action.performed -= ToggleUI;
        }
    }

    private void ToggleUI(InputAction.CallbackContext context)
    {
        if (uiCanvas != null)
        {
            bool isActive = !uiCanvas.activeSelf;
            uiCanvas.SetActive(isActive);
            Debug.Log($"[UIManager] UI toggled to: {isActive}");
        }
    }
    
    public void OnBridgeSliderChanged(float value)
    {
        Debug.Log($"[UIManager] Slider changed to: {value}");
        
        if (BridgeController.Instance != null)
        {
            BridgeController.Instance.UpdateHeight(value);
        }
        else
        {
            Debug.LogError("[UIManager] BridgeController.Instance is null!");
        }
    }
    
    public void OnPlayButtonClicked()
    {
        Debug.Log("[UIManager] Play button clicked");
        
        if (BridgeSpawner.Instance != null)
        {
            BridgeSpawner.Instance.TriggerAllPlayerTeleport();
            
            if (uiCanvas != null)
            {
                uiCanvas.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("[UIManager] BridgeSpawner.Instance is null!");
        }
    }
}