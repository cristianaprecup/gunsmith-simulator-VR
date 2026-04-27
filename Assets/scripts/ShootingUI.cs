using UnityEngine;
using TMPro;

public class ShootingUI : MonoBehaviour
{
    [Header("HUD Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultsText;

    void Awake() => Hide();

    public void Show(int score, float timeRemaining)
    {
        gameObject.SetActive(true);
        if (resultsText != null) resultsText.gameObject.SetActive(false);
        UpdateScore(score);
        UpdateTimer(timeRemaining);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Hits: {score}";
    }

    public void UpdateTimer(float seconds)
    {
        if (timerText != null)
        {
            int m = Mathf.FloorToInt(seconds / 60f);
            int s = Mathf.FloorToInt(seconds % 60f);
            timerText.text = $"{m:00}:{s:00}";
        }
    }

    public void ShowResults(int score, float totalTime)
    {
        if (timerText != null) timerText.text = "00:00";
        if (scoreText != null) scoreText.text = $"Hits: {score}";
        if (resultsText != null)
        {
            resultsText.gameObject.SetActive(true);
            resultsText.text = $"Done!\n{score} targets hit";
        }
    }

    public void Hide() => gameObject.SetActive(false);
}
