using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstructionUI : MonoBehaviour
{
    public TextMeshProUGUI stepText;
    public TextMeshProUGUI instructionText;
    public Slider progressBar;
    public Image feedbackPanel;

    public void ShowStep(string partName, int step, int total)
    {
        stepText.text = $"Step {step + 1} of {total}";
        instructionText.text = $"Attach: {partName}";
        progressBar.value = (float)step / total;
        feedbackPanel.color = Color.clear;
    }

    public void ShowCorrect()
    {
        feedbackPanel.color = new Color(0, 1, 0, 0.3f);
        instructionText.text = "Correct!";
    }

    public void ShowWrong(string correctPart)
    {
        feedbackPanel.color = new Color(1, 0, 0, 0.3f);
        instructionText.text = $"Wrong! Attach: {correctPart} first";
    }

    public void ShowComplete()
    {
        stepText.text = "Complete!";
        instructionText.text = "Rifle assembled successfully";
        progressBar.value = 1f;
        feedbackPanel.color = new Color(0, 1, 0, 0.4f);
    }
}