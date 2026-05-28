using UnityEngine;
using System.Collections;

/// <summary>
/// Attach to every target. Flashes red on hit, spawns a floating score popup,
/// and reports points to ShootingManager.
/// </summary>
public class ShootingTarget : MonoBehaviour
{
    [Header("Points")]
    public int pointValue = 1;           // Override per target (small=3, medium=2, large=1)

    [Header("Feedback")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.15f;
    public float resetDelay = 1.0f;

    [Header("Popup")]
    public GameObject hitPopupPrefab;    // Drag your HitPopup prefab here
    public Color popupColor = Color.yellow;

    [Header("Optional Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    private Renderer[] renderers;
    private Color[] originalColors;
    private bool isHit = false;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;
    }

    public void OnHit()
    {
        if (isHit) return;
        isHit = true;

        // Report hit with point value
        ShootingManager sm = FindFirstObjectByType<ShootingManager>();
        if (sm != null) sm.RegisterHit(pointValue);

        // Spawn floating popup above the target
        if (hitPopupPrefab != null)
        {
            Vector3 popupPos = transform.position + Vector3.up * 0.5f;
            GameObject popup = Instantiate(hitPopupPrefab, popupPos, Quaternion.identity);
            HitPopup hp = popup.GetComponent<HitPopup>();
            if (hp != null)
            {
                string text = pointValue > 1 ? $"+{pointValue}!" : $"+{pointValue}";
                hp.SetText(text, popupColor);
            }
        }

        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        SetColor(flashColor);
        yield return new WaitForSeconds(flashDuration);
        RestoreColors();
        yield return new WaitForSeconds(resetDelay - flashDuration);
        isHit = false;
    }

    void SetColor(Color c)
    {
        foreach (Renderer r in renderers) r.material.color = c;
    }

    void RestoreColors()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }
}
