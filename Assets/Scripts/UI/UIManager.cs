using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject uiCanvas;
    public InputActionReference toggleReference;
    private float selectedTimerDuration = 1f;
    private bool isSessionActive = false;

    // UI Elements
    public Button playButton;
    public TextMeshProUGUI playButtonText;
    public Slider bridgeSlider;
    public Button[] timerButtons;

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
            
            if (isActive)
            {
                UpdateUIState();
            }
            
            Debug.Log($"[UIManager] UI toggled to: {isActive}");
        }
    }

    private void UpdateUIState()
    {
        if (isSessionActive)
        {
            // Session läuft: End Session anzeigen
            playButtonText.text = "End Session";
            
            // Alle Controls deaktivieren
            bridgeSlider.interactable = false;
            foreach (Button btn in timerButtons)
            {
                btn.interactable = false;
            }
        }
        else
        {
            // Session nicht aktiv: Play anzeigen
            playButtonText.text = "Start Session";
            
            // Alle Controls aktivieren
            bridgeSlider.interactable = true;
            foreach (Button btn in timerButtons)
            {
                btn.interactable = true;
            }
        }
    }
    
    public void OnBridgeSliderChanged(float value)
    {
        if (isSessionActive) return; // Während Session nichts ändern
        
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
        if (isSessionActive) return; // Während Session nichts ändern
        
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
        if (isSessionActive) return;
        
        selectedTimerDuration = minutes;
        Debug.Log($"[UIManager] Timer set to {minutes} minutes");

        if (Timer.Instance != null)
        {
            Timer.Instance.SetTimerDuration(minutes);
        }
    }
    
    public void OnPlayButtonClicked()
    {
        if (isSessionActive)
        {
            EndSession();
        }
        else
        {
            StartSession();
        }
    }

    public void StartSession()
    {
        Debug.Log("[UIManager] Play button clicked - Starting session");
        
        isSessionActive = true;
        
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
            BridgeSpawner.Instance.TriggerAllPlayerTeleportBridge();
            
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

    public void EndSession()
    {
        Debug.Log("[UIManager] Play button clicked - Ending session");
        
        isSessionActive = false;
        
        if (Timer.Instance != null)
        {
            Timer.Instance.StopTimer();
        }
        
        if (BridgeSpawner.Instance != null)
        {
            BridgeSpawner.Instance.TriggerAllPlayerTeleportField();
        }
        else
        {
            Debug.LogError("[UIManager] BridgeSpawner.Instance is null!");
        }
        
        // UI bleibt offen nach End Session
        UpdateUIState();
    }
}