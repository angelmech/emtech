using UnityEngine;
using Fusion;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class AudioController : MonoBehaviour
{
    [Header("Ground Ambient Audio (Birds, Forest Wind)")]
    public AudioSource[] groundAmbientAudios;

    [Header("Bridge Wind Audio")]
    public AudioSource[] bridgeWindAudios;

    [Header("Relaxing Music")]
    public AudioSource relaxingMusicSource;

    [Header("Player (XR Camera)")]
    public NetworkPrefabRef playerPrefab;
    private Transform playerCameraTransform;

    [Header("Height References (World Y)")]
    [Tooltip("Camera Y when player is on the ground")]
    public float groundLevelY = 0f;

    [Tooltip("Camera Y when player is fully on the bridge")]
    public float bridgeLevelY = 10f;

    [Header("Volume Settings")]
    public float groundAmbientMaxVolume = 0.5f;
    public float groundAmbientMinVolume = 0f;

    public float bridgeWindMinVolume = 0f;
    public float bridgeWindMaxVolume = 0.5f;
    public float relaxingMusicVolume = 0.7f;

    [Header("Smoothing")]
    public float volumeChangeSpeed = 2f;
    public float musicFadeSpeed = 1f;

    [Header("VR Input")]
    public InputActionProperty toggleMusicAction;

    private bool isRelaxingMusicMode = false;
    private bool wasButtonPressed = false;

    void Start()
    {
        // Setup relaxing music source
        if (relaxingMusicSource != null)
        {
            relaxingMusicSource.loop = true;
            relaxingMusicSource.volume = 0f;
        }

        // Enable the input action
        toggleMusicAction.action?.Enable();
    }

    void Update()
    {
        HandleMusicToggle();

        // Find XR camera once
        if (playerCameraTransform == null)
        {
            var xrOrigin = GameObject.FindObjectOfType<XROrigin>();
            if (xrOrigin != null && xrOrigin.Camera != null)
            {
                playerCameraTransform = xrOrigin.Camera.transform;
            }
            else
            {
                return;
            }
        }

        float playerHeightY = playerCameraTransform.position.y;

        // Normalized height factor (0 = ground, 1 = bridge)
        float heightT = Mathf.Clamp01(
            (playerHeightY - groundLevelY) / (bridgeLevelY - groundLevelY)
        );

        // ----- Ground ambient fades OUT as player goes UP -----
        float targetGroundVolume = isRelaxingMusicMode ? 0f : Mathf.Lerp(
            groundAmbientMaxVolume,
            groundAmbientMinVolume,
            heightT
        );

        foreach (var audio in groundAmbientAudios)
        {
            if (audio == null) continue;

            if (!audio.isPlaying && !isRelaxingMusicMode)
                audio.Play();

            audio.volume = Mathf.Lerp(
                audio.volume,
                targetGroundVolume,
                Time.deltaTime * volumeChangeSpeed
            );

            // Stop audio if volume reaches zero in music mode
            if (isRelaxingMusicMode && audio.volume < 0.01f && audio.isPlaying)
                audio.Pause();
        }

        // ----- Bridge wind fades IN as player goes UP -----
        float targetBridgeVolume = isRelaxingMusicMode ? 0f : Mathf.Lerp(
            bridgeWindMinVolume,
            bridgeWindMaxVolume,
            heightT
        );

        foreach (var audio in bridgeWindAudios)
        {
            if (audio == null) continue;

            if (!audio.isPlaying && !isRelaxingMusicMode)
                audio.Play();

            audio.volume = Mathf.Lerp(
                audio.volume,
                targetBridgeVolume,
                Time.deltaTime * volumeChangeSpeed
            );

            // Optional realism
            if (!isRelaxingMusicMode)
                audio.pitch = 1f + heightT * 0.2f;

            // Stop audio if volume reaches zero in music mode
            if (isRelaxingMusicMode && audio.volume < 0.01f && audio.isPlaying)
                audio.Pause();
        }

        // Handle relaxing music fade
        HandleRelaxingMusic();
    }

    void HandleMusicToggle()
    {
        bool isButtonDown = toggleMusicAction.action?.ReadValue<float>() > 0.5f;

        // Detect button press (rising edge)
        if (isButtonDown && !wasButtonPressed)
        {
            isRelaxingMusicMode = !isRelaxingMusicMode;
        }

        wasButtonPressed = isButtonDown;
    }

    void HandleRelaxingMusic()
    {
        if (relaxingMusicSource == null) return;

        float targetMusicVolume = isRelaxingMusicMode ? relaxingMusicVolume : 0f;

        // Start playing if entering music mode
        if (isRelaxingMusicMode && !relaxingMusicSource.isPlaying)
        {
            relaxingMusicSource.Play();
        }

        // Fade volume
        relaxingMusicSource.volume = Mathf.Lerp(
            relaxingMusicSource.volume,
            targetMusicVolume,
            Time.deltaTime * musicFadeSpeed
        );

        // Stop if fully faded out
        if (!isRelaxingMusicMode && relaxingMusicSource.volume < 0.01f && relaxingMusicSource.isPlaying)
        {
            relaxingMusicSource.Stop();
        }
    }

    void OnDestroy()
    {
        toggleMusicAction.action?.Disable();
    }
}