using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Attach this to a prefab with a TextMeshPro component.
/// ShootingTarget spawns one of these on hit.
/// 
/// Setup:
///   - Create an empty GameObject, add TextMeshPro (3D), add this script
///   - Set font size to around 0.3, color white or yellow
///   - Save as a prefab in your Prefabs folder
///   - Drag the prefab into the HitPopupPrefab slot on each ShootingTarget
/// </summary>
public class HitPopup : MonoBehaviour
{
    [Header("Animation")]
    public float floatSpeed = 1.5f;      // How fast it rises
    public float lifetime = 0.8f;        // Seconds before it disappears
    public float scaleUpTime = 0.1f;     // Quick scale-up punch at start

    private TextMeshPro tmp;
    private float elapsed = 0f;
    private Vector3 startScale;

    void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        startScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Scale punch at start
        if (elapsed < scaleUpTime)
        {
            float t = elapsed / scaleUpTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, startScale, t);
        }
        else
        {
            transform.localScale = startScale;
        }

        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Face the camera
        Camera cam = Camera.main;
        if (cam != null)
            transform.forward = cam.transform.forward;

        // Fade out in the last half of lifetime
        float fadeStart = lifetime * 0.5f;
        if (elapsed > fadeStart && tmp != null)
        {
            float alpha = 1f - ((elapsed - fadeStart) / (lifetime - fadeStart));
            Color c = tmp.color;
            c.a = Mathf.Clamp01(alpha);
            tmp.color = c;
        }

        if (elapsed >= lifetime)
            Destroy(gameObject);
    }

    public void SetText(string text, Color color)
    {
        tmp = GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = text;
            tmp.color = color;
        }
    }
}
