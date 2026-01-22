using UnityEngine;
using TMPro;
using Fusion;

public class Timer : NetworkBehaviour
{
    public static Timer Instance;
    
    [Header("UI References")]
    public TextMeshPro timerText;
    public Canvas timerCanvas;
    
    [Header("Timer Settings")]
    public Vector3 offsetFromBridge = new Vector3(2f, 2f, 0f);
    public bool alwaysFaceCamera = true;
    
    [Networked] private float NetworkStartTime { get; set; }
    [Networked] private NetworkBool IsRunning { get; set; }
    [Networked] private float TimerDuration { get; set; } // Total duration in seconds
    [Networked] private NetworkBool IsCountdown { get; set; } // true = countdown, false = count up
    
    private float localStartTime;
    private bool localTimerRunning = false;
    private float localDuration = 0f;
    private bool localIsCountdown = false;
    
    
    public override void Spawned()
    {
        // Position the canvas next to the bridge
        if (SpawnerScript.Instance != null && timerCanvas != null)
        {
            timerCanvas.transform.position = SpawnerScript.Instance.spawnerTransformBridge.position + offsetFromBridge;
        }
        
        if (Instance == null)
        {
            Instance = this;
        }

    }
    
    void Update()
    {
        // Make canvas face camera
        if (alwaysFaceCamera && timerCanvas != null)
        {
            Camera xrCamera = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>()?.Camera;
            if (xrCamera != null)
            {
                timerCanvas.transform.LookAt(xrCamera.transform);
                timerCanvas.transform.Rotate(0, 180, 0);
            }
        }
        
        UpdateTimerDisplay();
    }
    
    /// <summary>
    /// Set timer duration in minutes (for countdown mode)
    /// </summary>
    public void SetTimerDuration(float minutes)
    {
        if (Object != null && Object.HasStateAuthority)
        {
            TimerDuration = minutes * 60f;
            IsCountdown = true;
        }
        
        localDuration = minutes * 60f;
        localIsCountdown = true;
        
        Debug.Log($"[Timer] Timer set to {minutes} minutes ({localDuration} seconds)");
    }
    
    public void StartTimer()
    {
        if (Object != null && Object.HasStateAuthority)
        {
            NetworkStartTime = (float)Runner.SimulationTime;
            IsRunning = true;
        }
        
        localStartTime = Time.time;
        localTimerRunning = true;
        
        Debug.Log($"[Timer] Timer started. Duration: {localDuration}s, Countdown: {localIsCountdown}");
    }
    
    public void StopTimer()
    {
        if (Object != null && Object.HasStateAuthority)
        {
            IsRunning = false;
        }
        localTimerRunning = false;
    }
    
    public void ResetTimer()
    {
        if (Object != null && Object.HasStateAuthority)
        {
            NetworkStartTime = (float)Runner.SimulationTime;
            IsRunning = true;
        }
        
        localStartTime = Time.time;
        localTimerRunning = true;
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        
        float elapsedTime = GetElapsedTime();
        float displayTime;
        
        // Determine if we're counting down or up
        bool isCountingDown = (Object != null && IsCountdown) || localIsCountdown;
        float duration = (Object != null) ? TimerDuration : localDuration;
        
        if (isCountingDown && duration > 0)
        {
            // Countdown mode
            displayTime = Mathf.Max(0, duration - elapsedTime);
            
            // Check if time's up
            if (displayTime <= 0)
            {
                timerText.color = Color.red;
                timerText.text = "TIME'S UP!";
                StopTimer();
                return;
            }
            
            // Color warning when under 30 seconds
            if (displayTime <= 30f)
            {
                timerText.color = Color.Lerp(Color.yellow, Color.red, 1f - (displayTime / 30f));
            }
            else
            {
                timerText.color = Color.white;
            }
        }
        else
        {
            // Count up mode
            displayTime = elapsedTime;
            timerText.color = Color.white;
        }
        
        // Format: MM:SS:MS (only minutes, seconds, and milliseconds)
        int minutes = Mathf.FloorToInt(displayTime / 60f);
        int seconds = Mathf.FloorToInt(displayTime % 60f);
        int milliseconds = Mathf.FloorToInt((displayTime * 1000f) % 1000f);
        
        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
    
    private float GetElapsedTime()
    {
        // Use networked timer if available and running
        if (Object != null && IsRunning && Runner != null)
        {
            return (float)Runner.SimulationTime - NetworkStartTime;
        }
        
        // Fallback to local timer
        if (localTimerRunning)
        {
            return Time.time - localStartTime;
        }
        
        return 0f;
    }
    
    public float GetCurrentTime()
    {
        return GetElapsedTime();
    }
    
    public float GetRemainingTime()
    {
        bool isCountingDown = (Object != null && IsCountdown) || localIsCountdown;
        float duration = (Object != null) ? TimerDuration : localDuration;
        
        if (isCountingDown && duration > 0)
        {
            return Mathf.Max(0, duration - GetElapsedTime());
        }
        
        return 0f;
    }
}