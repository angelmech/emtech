using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    
    public GameObject uiCanvas;
    public InputActionReference toggleReference;
    
    public Transform xrOrigin;

    private void OnEnable()
    {
        toggleReference.action.performed += ToggleUI;
    }

    private void OnDisable()
    {
        toggleReference.action.performed += ToggleUI;
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
        if (BridgeController.Instance != null && xrOrigin != null)
        {
            Transform spawner = BridgeController.Instance.spawnerTransform;
            
            // Teleportiere den XR Origin
            xrOrigin.position = spawner.position;
            xrOrigin.rotation = spawner.rotation;

            // UI nach Teleport schließen
            uiCanvas.SetActive(false);
        }
    }
}
