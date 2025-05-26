using UnityEngine;

public class MinimapRotating : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform minimapRect;  // RawImage parent or container (do NOT rotate this)
    [SerializeField] private RectTransform iconPlayer;
    [SerializeField] private RectTransform iconOpponent;
    [SerializeField] private RectTransform iconFlag;

    [Header("World References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform opponent;
    [SerializeField] private Transform flag;

    [SerializeField] private Camera minimapCamera;

    [Header("Settings")]
    [SerializeField] private float maxDistance = 50f;

    private void LateUpdate()
    {
        if (player == null || minimapCamera == null) return;

        // Rotate the minimap camera to match the player's rotation (around Y-axis)
        Vector3 camEuler = minimapCamera.transform.eulerAngles;
        camEuler.y = player.eulerAngles.y;
        minimapCamera.transform.eulerAngles = camEuler;

        // The minimap UI (minimapRect) does NOT rotate now, stays fixed

        // Update icons position relative to player
        UpdateIcon(iconPlayer, player.position, true);
        UpdateIcon(iconOpponent, opponent.position);
        UpdateIcon(iconFlag, flag.position);

        // Now update icon rotations:

        // Optional: rotate player icon to face same direction as player
        if (iconPlayer != null)
        {
            float playerYRotation = player.eulerAngles.y - minimapCamera.transform.eulerAngles.y - 90f;
            iconPlayer.localRotation = Quaternion.Euler(0f, 0f, -playerYRotation);
        }

        // Rotate opponent icon based on their rotation relative to player and minimap camera
        if (iconOpponent != null && opponent != null)
        {
            float relativeRotation = opponent.eulerAngles.y - minimapCamera.transform.eulerAngles.y - 90f;
            iconOpponent.localRotation = Quaternion.Euler(0f, 0f, -relativeRotation);
        }

        // Usually flag icon does not rotate, so leave it identity
        if (iconFlag != null)
        {
            iconFlag.localRotation = Quaternion.identity;
        }
    }


    private void UpdateIcon(RectTransform icon, Vector3 worldPosition, bool center = false)
    {
        if (icon == null) return;

        // Calculate offset from player to target in world space
        Vector3 offset = worldPosition - player.position;
        offset.y = 0f;

        // Since minimapCamera rotates with the player, rotate offset by camera Y rotation
        Quaternion camRotation = Quaternion.Euler(0, minimapCamera.transform.eulerAngles.y, 0);
        Vector3 rotatedOffset = Quaternion.Inverse(camRotation) * offset;

        // Clamp distance if not centered
        if (!center && rotatedOffset.magnitude > maxDistance)
        {
            rotatedOffset = rotatedOffset.normalized * maxDistance;
        }

        // Convert world units to minimap UI units
        Vector2 minimapSize = minimapRect.sizeDelta;
        float pixelsPerUnit = minimapSize.x / (minimapCamera.orthographicSize * 2f);

        Vector2 localPos = new Vector2(rotatedOffset.x, rotatedOffset.z) * pixelsPerUnit;

        icon.anchoredPosition = localPos;

        // Keep icons upright (no rotation)
        icon.localRotation = Quaternion.identity;
    }
}
