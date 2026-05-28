using UnityEngine;
using TMPro;

/// <summary>
/// World Space canvas shown at the start of the shooting phase.
/// Player picks a mode, then the round begins.
/// 
/// Setup:
///   - Create a World Space Canvas, parent it to Main Camera
///   - Pos Z: 0.6, Scale: 0.001
///   - Add two UI Buttons: TimeAttackButton and AccuracyButton
///   - Add two TextMeshPro texts for the button labels and one for the header
///   - Assign this script to the Canvas root
/// </summary>
public class ModeSelectUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI timeAttackDescText;
    public TextMeshProUGUI accuracyDescText;

    [Header("References")]
    public ShootingManager shootingManager;

    void Awake() => Hide();

    public void Show()
    {
        gameObject.SetActive(true);

        if (headerText != null)
            headerText.text = "Choose Your Mode";

        if (timeAttackDescText != null)
            timeAttackDescText.text = "Time Attack\nHit as many targets\nas possible in 60s";

        if (accuracyDescText != null)
            accuracyDescText.text = "Accuracy\n20 bullets only\nscore = hit %";
    }

    public void Hide() => gameObject.SetActive(false);

    // Called by the Time Attack button's OnClick
    public void SelectTimeAttack()
    {
        GameModeManager.Instance.selectedMode = GameModeManager.GameMode.TimeAttack;
        Hide();
        shootingManager.BeginRound();
    }

    // Called by the Accuracy button's OnClick
    public void SelectAccuracy()
    {
        GameModeManager.Instance.selectedMode = GameModeManager.GameMode.Accuracy;
        Hide();
        shootingManager.BeginRound();
    }
}
