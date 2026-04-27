using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameState { Idle, Tutorial, Challenge, FreeBuild }
    public GameState currentState = GameState.Idle;

    public TutorialController tutorialController;
    public AssemblyManager assemblyManager;
    public FreeBuildManager freeBuildManager;

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

    public void StartFreeBuild()
    {
        currentState = GameState.FreeBuild;
        freeBuildManager.BeginFreeBuild();
    }

    public void ReturnToIdle()
    {
        if (currentState == GameState.FreeBuild)
            freeBuildManager.StopFreeBuild();

        currentState = GameState.Idle;
        assemblyManager.ResetAll();
    }
}