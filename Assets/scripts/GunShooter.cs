using UnityEngine;
using UnityEngine.InputSystem;

public class GunShooter : MonoBehaviour
{
    [Header("References")]
    public Transform muzzlePoint;

    [Header("Shooting Settings")]
    public float shootRange = 100f;
    public float fireRate = 0.15f;
    public LayerMask shootableLayers = ~0;

    [Header("Feedback")]
    public GameObject muzzleFlashPrefab;
    public float muzzleFlashDuration = 0.05f;
    public AudioSource gunAudioSource;
    public AudioClip shootSound;

    private bool shootingEnabled = false;
    private float nextFireTime = 0f;

    private InputAction triggerAction;

    void Awake()
    {
        triggerAction = new InputAction(
            "Shoot",
            binding: "<XRController>{RightHand}/trigger"
        );
        triggerAction.Enable();
    }

    void OnDestroy()
    {
        triggerAction?.Disable();
        triggerAction?.Dispose();
    }

    public void Enable()
    {
        shootingEnabled = true;
        Debug.Log("GunShooter: Enabled.");
    }

    public void Disable()
    {
        shootingEnabled = false;
    }

    void Update()
    {
        if (!shootingEnabled) return;
        if (Time.time < nextFireTime) return;

        float triggerValue = triggerAction.ReadValue<float>();

        if (Time.frameCount % 60 == 0)
            Debug.Log($"GunShooter: trigger = {triggerValue:F2}");

        if (triggerValue > 0.5f)
        {
            nextFireTime = Time.time + fireRate;
            Fire();
        }
    }

    private void Fire()
    {
        if (muzzlePoint == null)
        {
            Debug.LogWarning("GunShooter: muzzlePoint not assigned!");
            return;
        }

        Ray ray = new Ray(muzzlePoint.position, muzzlePoint.forward);
        Debug.Log("GunShooter: FIRED!");

        if (Physics.Raycast(ray, out RaycastHit hit, shootRange, shootableLayers))
        {
            Debug.Log($"GunShooter: Hit {hit.collider.gameObject.name}");
            ShootingTarget target = hit.collider.GetComponent<ShootingTarget>();
            if (target == null) target = hit.collider.GetComponentInParent<ShootingTarget>();
            if (target != null) target.OnHit();
            else Debug.Log("GunShooter: No ShootingTarget on hit object.");
        }
        else
        {
            Debug.Log("GunShooter: Raycast hit nothing.");
        }

        PlayMuzzleFlash();
        PlaySound();
        Debug.DrawRay(ray.origin, ray.direction * shootRange, Color.yellow, 0.5f);
    }

    private void PlayMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzlePoint == null) return;
        GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
        Destroy(flash, muzzleFlashDuration);
    }

    private void PlaySound()
    {
        if (gunAudioSource != null && shootSound != null)
            gunAudioSource.PlayOneShot(shootSound);
    }
}
