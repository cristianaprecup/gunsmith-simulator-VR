using UnityEngine;
using System.Collections;

public class ShootingTarget : MonoBehaviour
{
    [Header("Feedback")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.15f;
    public float resetDelay = 1.0f;

    [Header("Optional Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    // Internal state
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

        ShootingManager sm = FindObjectOfType<ShootingManager>();
        if (sm != null) sm.RegisterHit();

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
        foreach (Renderer r in renderers)
            r.material.color = c;
    }

    void RestoreColors()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }
}
