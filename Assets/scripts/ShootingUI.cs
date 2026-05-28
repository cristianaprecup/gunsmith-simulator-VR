using UnityEngine;
using TMPro;

public class ShootingUI : MonoBehaviour
{
    [Header("HUD Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI ammoText;     // New: shown in Accuracy mode
    public TextMeshProUGUI resultsText;

    void Awake() => Hide();

    public void Show(int score, float timeRemaining)
    {
        gameObject.SetActive(true);
        if (resultsText != null) resultsText.gameObject.SetActive(false);
        UpdateScore(score);

        if (timeRemaining < 0f)
        {
            // Accuracy mode — hide timer
            if (timerText != null) timerText.gameObject.SetActive(false);
        }
        else
        {
            if (timerText != null) timerText.gameObject.SetActive(true);
            UpdateTimer(timeRemaining);
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }

    public void UpdateTimer(float seconds)
    {
        if (timerText == null) return;
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        timerText.text = $"{m:00}:{s:00}";

        // Turn timer red when under 10 seconds
        timerText.color = seconds < 10f ? Color.red : Color.white;
    }

    public void SetAmmoVisible(bool visible)
    {
        if (ammoText != null) ammoText.gameObject.SetActive(visible);
    }

    public void UpdateAmmo(int ammoLeft)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {ammoLeft}";
            ammoText.color = ammoLeft <= 5 ? Color.red : Color.white;
        }
    }

    public void ShowResults(int score, float totalTime)
    {
        if (timerText != null) timerText.text = "00:00";
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (resultsText != null)
        {
            resultsText.gameObject.SetActive(true);
            resultsText.text = $"Time's Up!\nScore: {score}";
        }
    }

    public void ShowAccuracyResults(int hits, int shots, int percent)
    {
        if (scoreText != null) scoreText.text = $"Score: {hits}";
        if (ammoText != null) ammoText.text = "Ammo: 0";
        if (resultsText != null)
        {
            resultsText.gameObject.SetActive(true);
            resultsText.text = $"Out of Ammo!\nHits: {hits}/{shots}\nAccuracy: {percent}%";
        }
    }

    public void Hide() => gameObject.SetActive(false);
}
