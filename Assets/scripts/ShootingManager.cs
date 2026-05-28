using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class ShootingManager : MonoBehaviour
{
    [Header("Gun")]
    public GameObject gunRoot;
    public GunShooter gunShooter;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable gunInteractable;
    public Transform gunSpawnPoint;

    [Header("Targets & UI")]
    public TargetSpawner targetSpawner;
    public ShootingUI shootingUI;
    public ModeSelectUI modeSelectUI;   // Assign the mode select canvas here

    [Header("Time Attack Settings")]
    public float timeAttackDuration = 60f;

    [Header("Accuracy Settings")]
    public int accuracyAmmo = 20;       // Total bullets in accuracy mode

    // Runtime state
    private int score = 0;
    private int shotsFired = 0;
    private int shotsHit = 0;
    private float timeRemaining = 0f;
    private bool roundActive = false;
    private Coroutine timerCoroutine;
    private Rigidbody gunRigidbody;

    // Called by GameManager when phase begins — shows mode select first
    public void BeginShootingPhase()
    {
        SetupGun();
        targetSpawner.SpawnAll();

        if (modeSelectUI != null)
            modeSelectUI.Show();
        else
            BeginRound(); // No mode select assigned, just start
    }

    // Called by ModeSelectUI once player picks a mode
    public void BeginRound()
    {
        score = 0;
        shotsFired = 0;
        shotsHit = 0;
        roundActive = true;

        GameModeManager.GameMode mode = GameModeManager.Instance != null
            ? GameModeManager.Instance.selectedMode
            : GameModeManager.GameMode.TimeAttack;

        if (mode == GameModeManager.GameMode.TimeAttack)
        {
            timeRemaining = timeAttackDuration;
            shootingUI.Show(score, timeRemaining);
            shootingUI.SetAmmoVisible(false);
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            timerCoroutine = StartCoroutine(RunTimer());
        }
        else // Accuracy
        {
            timeRemaining = 0;
            shootingUI.Show(score, -1f); // -1 = hide timer
            shootingUI.SetAmmoVisible(true);
            shootingUI.UpdateAmmo(accuracyAmmo);
        }

        if (gunShooter != null) gunShooter.Enable();
    }

    void SetupGun()
    {
        if (gunRoot == null) return;

        gunRoot.SetActive(true);
        gunRoot.transform.SetParent(null);

        if (gunSpawnPoint != null)
        {
            gunRoot.transform.position = gunSpawnPoint.position;
            gunRoot.transform.rotation = gunSpawnPoint.rotation;
        }

        gunRigidbody = gunRoot.GetComponent<Rigidbody>();
        if (gunRigidbody == null) gunRigidbody = gunRoot.AddComponent<Rigidbody>();
        gunRigidbody.isKinematic = true;

        if (gunInteractable != null)
        {
            gunInteractable.enabled = true;
            gunInteractable.selectEntered.AddListener(OnGunGrabbed);
            gunInteractable.selectExited.AddListener(OnGunReleased);
        }
    }

    private void OnGunGrabbed(SelectEnterEventArgs args)
    {
        if (gunRigidbody != null) gunRigidbody.isKinematic = false;
    }

    private void OnGunReleased(SelectExitEventArgs args)
    {
        if (gunRigidbody != null) gunRigidbody.isKinematic = false;
    }

    public void RegisterHit(int points = 1)
    {
        if (!roundActive) return;

        shotsHit++;
        score += points;
        shootingUI.UpdateScore(score);

        // Accuracy mode: check if all ammo used
        GameModeManager.GameMode mode = GameModeManager.Instance != null
            ? GameModeManager.Instance.selectedMode
            : GameModeManager.GameMode.TimeAttack;

        if (mode == GameModeManager.GameMode.Accuracy)
        {
            int ammoLeft = accuracyAmmo - shotsFired;
            shootingUI.UpdateAmmo(ammoLeft);
        }
    }

    // Called by GunShooter every time trigger is pulled
    public void RegisterShot()
    {
        if (!roundActive) return;

        GameModeManager.GameMode mode = GameModeManager.Instance != null
            ? GameModeManager.Instance.selectedMode
            : GameModeManager.GameMode.TimeAttack;

        if (mode != GameModeManager.GameMode.Accuracy) return;

        shotsFired++;
        int ammoLeft = accuracyAmmo - shotsFired;
        shootingUI.UpdateAmmo(ammoLeft);

        if (ammoLeft <= 0)
        {
            roundActive = false;
            if (gunShooter != null) gunShooter.Disable();
            targetSpawner.DespawnAll();

            int percent = shotsFired > 0 ? Mathf.RoundToInt((shotsHit / (float)shotsFired) * 100) : 0;
            shootingUI.ShowAccuracyResults(shotsHit, shotsFired, percent);
        }
    }

    public void EndShootingPhase()
    {
        roundActive = false;
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (gunShooter != null) gunShooter.Disable();

        if (gunInteractable != null)
        {
            gunInteractable.selectEntered.RemoveListener(OnGunGrabbed);
            gunInteractable.selectExited.RemoveListener(OnGunReleased);
            gunInteractable.enabled = false;
        }

        targetSpawner.DespawnAll();
        shootingUI.Hide();
        if (modeSelectUI != null) modeSelectUI.Hide();
    }

    IEnumerator RunTimer()
    {
        while (timeRemaining > 0f)
        {
            yield return null;
            timeRemaining -= Time.deltaTime;
            shootingUI.UpdateTimer(Mathf.Max(0f, timeRemaining));
        }

        roundActive = false;
        if (gunShooter != null) gunShooter.Disable();
        targetSpawner.DespawnAll();
        shootingUI.ShowResults(score, timeAttackDuration);
    }
}
