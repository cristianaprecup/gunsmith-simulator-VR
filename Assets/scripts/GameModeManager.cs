using UnityEngine;

/// <summary>
/// Defines the available game modes and holds the current selection.
/// ShootingManager reads this to know how to run the round.
/// </summary>
public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    public enum GameMode
    {
        TimeAttack,   // Hit as many targets as possible before time runs out
        Accuracy      // Limited ammo, scored on hit percentage
    }

    public GameMode selectedMode = GameMode.TimeAttack;

    void Awake() => Instance = this;
}
