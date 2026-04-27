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

    [Header("Round Settings")]
    public float shootingDuration = 60f;

    private int score = 0;
    private float timeRemaining = 0f;
    private bool roundActive = false;
    private Coroutine timerCoroutine;
    private Rigidbody gunRigidbody;

    public void BeginShootingPhase()
    {
        score = 0;
        timeRemaining = shootingDuration;
        roundActive = true;

        if (gunRoot != null)
        {
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

        if (gunShooter != null) gunShooter.Enable();

        targetSpawner.SpawnAll();
        shootingUI.Show(score, timeRemaining);

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(RunTimer());
    }

    private void OnGunGrabbed(SelectEnterEventArgs args)
    {
        if (gunRigidbody != null) gunRigidbody.isKinematic = false;
    }

    private void OnGunReleased(SelectExitEventArgs args)
    {
        if (gunRigidbody != null) gunRigidbody.isKinematic = false;
    }

    public void RegisterHit()
    {
        if (!roundActive) return;
        score++;
        shootingUI.UpdateScore(score);
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
        shootingUI.ShowResults(score, shootingDuration);
    }
}
