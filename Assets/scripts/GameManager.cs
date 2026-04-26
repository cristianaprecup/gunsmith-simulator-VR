using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameState { Idle, Tutorial, Challenge }
    public GameState currentState = GameState.Idle;

    public TutorialController tutorialController;
    public AssemblyManager assemblyManager;

    void Awake() => Instance = this;

    public void StartTutorial()
    {
        currentState = GameState.Tutorial;
        tutorialController.BeginTutorial();
    }

    public void StartChallenge()
    {
        currentState = GameState.Challenge;
        assemblyManager.BeginChallenge();
    }

    public void ReturnToIdle()
    {
        currentState = GameState.Idle;
        assemblyManager.ResetAll();
    }
}