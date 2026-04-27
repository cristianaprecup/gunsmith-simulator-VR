using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FreeBuildUI : MonoBehaviour
{
    [Header("Progress")]
    public Slider progressBar;
    public Image progressFill;             // the fill image of the slider
    public TextMeshProUGUI progressText;   // "12 / 43"
    public TextMeshProUGUI percentText;    // "28%"

    [Header("Status")]
    public TextMeshProUGUI statusText;     // "5 parts available"
    public TextMeshProUGUI feedbackText;   // "Correct!" / "Need X first"
    public Image feedbackPanel;

    [Header("Colors")]
    public Color normalFillColor = new Color(0.2f, 0.6f, 1f, 1f);
    public Color completeFillColor = new Color(0.2f, 0.9f, 0.3f, 1f);
    public Color correctColor = new Color(0f, 1f, 0f, 0.3f);
    public Color wrongColor = new Color(1f, 0f, 0f, 0.3f);

    private Coroutine feedbackCoroutine;
    private int totalParts;

    public void Show(int total)
    {
        totalParts = total;
        gameObject.SetActive(true);
        progressBar.value = 0f;

        if (progressFill != null)
            progressFill.color = normalFillColor;

        progressText.text = $"0 / {total}";
        percentText.text = "0%";
        statusText.text = "Grab a highlighted part and place it!";
        feedbackText.text = "";

        if (feedbackPanel != null)
            feedbackPanel.color = Color.clear;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateProgress(int assembled, int total, int availableCount)
    {
        float progress = (float)assembled / total;

        progressBar.value = progress;
        progressText.text = $"{assembled} / {total}";
        percentText.text = $"{Mathf.RoundToInt(progress * 100)}%";

        if (availableCount > 0)
            statusText.text = $"{availableCount} part{(availableCount > 1 ? "s" : "")} available";
        else if (assembled < total)
            statusText.text = "Place current parts to unlock more";
        else
            statusText.text = "";
    }

    public void ShowFeedback(string message, bool isPositive)
    {
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        feedbackText.text = message;

        if (feedbackPanel != null)
            feedbackPanel.color = isPositive ? correctColor : wrongColor;

        feedbackCoroutine = StartCoroutine(ClearFeedback());
    }

    public void ShowComplete()
    {
        progressBar.value = 1f;
        progressText.text = $"{totalParts} / {totalParts}";
        percentText.text = "100%";
        statusText.text = "Rifle fully assembled!";

        if (progressFill != null)
            progressFill.color = completeFillColor;

        ShowFeedback("Complete! Great job!", true);
    }

    private IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(2f);
        feedbackText.text = "";

        if (feedbackPanel != null)
            feedbackPanel.color = Color.clear;
    }
}
