using UnityEngine;

public class TableUI : MonoBehaviour
{
    public void OnTutorialPressed()
        => GameManager.Instance.StartTutorial();

    public void OnChallengePressed()
        => GameManager.Instance.StartChallenge();

    public void OnFreeBuildPressed()
        => GameManager.Instance.StartFreeBuild();

    public void OnResetPressed()
        => GameManager.Instance.ReturnToIdle();
}