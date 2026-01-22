using UnityEngine;
using UnityEngine.InputSystem;

public class PanicButton : MonoBehaviour
{
    [Header("VR Input")]
    [Tooltip("Map this to Left Controller Primary Button")]
    public InputActionProperty panicButtonAction;

    [Header("References")]
    public UIManager uiManager;

    private bool wasButtonPressed = false;

    void Start()
    {
        // Enable the input action
        panicButtonAction.action?.Enable();

        // Try to find UIManager if not assigned
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("[PanicButton] UIManager not found in scene!");
            }
        }
    }

    void Update()
    {
        HandlePanicButton();
    }

    void HandlePanicButton()
    {
        bool isButtonDown = panicButtonAction.action?.ReadValue<float>() > 0.5f;

        // Detect button press (rising edge)
        if (isButtonDown && !wasButtonPressed)
        {
            TriggerPanic();
        }

        wasButtonPressed = isButtonDown;
    }

    void TriggerPanic()
    {
        Debug.Log("[PanicButton] PANIC BUTTON PRESSED - Ending session!");

        if (uiManager != null)
        {
            // Call the private EndSession method via the public OnPlayButtonClicked
            // check if session is active first
            if (uiManager.GetType().GetField("isSessionActive", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance)?.GetValue(uiManager) is bool isActive)
            {
                if (isActive)
                {
                    uiManager.OnPlayButtonClicked();
                }
                else
                {
                    Debug.LogWarning("[PanicButton] Session is not active, nothing to end.");
                }
            }
        }
        else
        {
            Debug.LogError("[PanicButton] UIManager reference is null!");
        }
    }

    void OnDestroy()
    {
        panicButtonAction.action?.Disable();
    }
}