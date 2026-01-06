using UnityEngine;
using Fusion;

public class AudioController : MonoBehaviour
{
    [Header("Ground Ambient Audio (Birds, Forest Wind)")]
    public AudioSource[] groundAmbientAudios;

    [Header("Bridge Wind Audio")]
    public AudioSource[] bridgeWindAudios;

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

    [Header("Smoothing")]
    public float volumeChangeSpeed = 2f;

    void Update()
    {
        // Find XR camera once (Fusion-safe)
        if (playerCameraTransform == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null) return;

            var cam = playerObj.GetComponentInChildren<Camera>();
            if (cam == null) return;

            playerCameraTransform = cam.transform;
        }

        float playerHeightY = playerCameraTransform.position.y;

        // Normalized height factor (0 = ground, 1 = bridge)
        float heightT = Mathf.Clamp01(
            (playerHeightY - groundLevelY) / (bridgeLevelY - groundLevelY)
        );

        // ----- Ground ambient fades OUT as player goes UP -----
        float targetGroundVolume = Mathf.Lerp(
            groundAmbientMaxVolume,
            groundAmbientMinVolume,
            heightT
        );

        foreach (var audio in groundAmbientAudios)
        {
            if (audio == null) continue;

            if (!audio.isPlaying)
                audio.Play();

            audio.volume = Mathf.Lerp(
                audio.volume,
                targetGroundVolume,
                Time.deltaTime * volumeChangeSpeed
            );
        }

        // ----- Bridge wind fades IN as player goes UP -----
        float targetBridgeVolume = Mathf.Lerp(
            bridgeWindMinVolume,
            bridgeWindMaxVolume,
            heightT
        );

        foreach (var audio in bridgeWindAudios)
        {
            if (audio == null) continue;

            if (!audio.isPlaying)
                audio.Play();

            audio.volume = Mathf.Lerp(
                audio.volume,
                targetBridgeVolume,
                Time.deltaTime * volumeChangeSpeed
            );

            // Optional realism
            audio.pitch = 1f + heightT * 0.2f;
        }
    }
}
