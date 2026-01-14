using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject uiCanvas;
    public InputActionReference toggleReference;

    private void OnEnable()
    {
        toggleReference.action.performed += ToggleUI;
    }

    private void OnDisable()
    {
        toggleReference.action.performed -= ToggleUI;
    }

    private void ToggleUI(InputAction.CallbackContext context)
    {
        bool isActive = !uiCanvas.activeSelf;
        uiCanvas.SetActive(isActive);
        
    }
    
    public void OnBridgeSliderChanged(float value)
    {
        if (BridgeController.Instance != null)
        {
            BridgeController.Instance.UpdateHeight(value);
        }
    }
    
    public void OnPlayButtonClicked()
    {
        if (BridgeSpawner.Instance != null)
        {
            Debug.Log("UIManager: Triggering teleport for all players");
            BridgeSpawner.Instance.TriggerAllPlayerTeleport();
            uiCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("UIManager: sBridgeSpawner.Instance is null!");
        }
    }
}