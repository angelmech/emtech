using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject uiCanvas;
    public InputActionReference toggleReference;
    private float selectedTimerDuration = 1f;

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
    
    // timer methods
    public void OnTimerButtonClicked(float minutes)
    {
        selectedTimerDuration = minutes;

        // Set the timer
        if (Timer.Instance != null)
            Timer.Instance.SetTimerDuration(minutes);

        // Get the clicked button automatically
        Button clicked = EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
        if (clicked == null) return;

        // Reset all sibling buttons' colors
        foreach (Transform sibling in clicked.transform.parent)
        {
            Button b = sibling.GetComponent<Button>();
            if (b != null)
                b.image.color = b.colors.normalColor; // back to normal
        }

        // Darken the clicked button slightly
        Color c = clicked.image.color;
        clicked.image.color = new Color(c.r * 0.7f, c.g * 0.7f, c.b * 0.7f, c.a);
    }

    public void SetTimerDuration(float minutes)
    {
        selectedTimerDuration = minutes;
        Debug.Log($"[UIManager] Timer set to {minutes} minutes");

        if (Timer.Instance != null)
        {
            Timer.Instance.SetTimerDuration(minutes);
        }
    }
    
    public void OnPlayButtonClicked()
    {
        Debug.Log("[UIManager] Play button clicked");
        
        // Start the timer
        if (Timer.Instance != null)
        {
            Timer.Instance.StartTimer();
        }
        else
        {
            Debug.LogWarning("[UIManager] Timer.Instance is null!");
        }
        
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