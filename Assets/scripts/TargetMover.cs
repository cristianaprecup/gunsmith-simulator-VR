using UnityEngine;

/// <summary>
/// Attach to any target to make it move.
/// Moves back and forth between two points relative to its starting position.
/// 
/// Setup in Inspector:
///   - moveAxis: which direction it moves (X = left/right, Y = up/down, Z = forward/back)
///   - moveDistance: how far it travels each way from its start position
///   - moveSpeed: how fast it moves
///   - pauseDuration: how long it waits at each end before turning around
/// </summary>
public class TargetMover : MonoBehaviour
{
    public enum MoveAxis { X, Y, Z }

    [Header("Movement")]
    public MoveAxis moveAxis = MoveAxis.X;
    public float moveDistance = 2f;
    public float moveSpeed = 1.5f;

    [Header("Pause at ends")]
    public float pauseDuration = 0f;     // 0 = no pause, smooth back and forth

    [Header("Randomise")]
    public bool randomiseSpeed = false;  // Adds slight variation so targets don't all sync
    public float speedVariance = 0.3f;

    private Vector3 startPosition;
    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 targetPoint;
    private float pauseTimer = 0f;
    private bool pausing = false;

    void Start()
    {
        startPosition = transform.position;

        if (randomiseSpeed)
            moveSpeed += Random.Range(-speedVariance, speedVariance);

        Vector3 direction = moveAxis == MoveAxis.X ? Vector3.right
                          : moveAxis == MoveAxis.Y ? Vector3.up
                          : Vector3.forward;

        pointA = startPosition - direction * moveDistance;
        pointB = startPosition + direction * moveDistance;
        targetPoint = pointB;
    }

    void Update()
    {
        if (pausing)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
                pausing = false;
            return;
        }

        // Move toward target point
        transform.position = Vector3.MoveTowards(
            transform.position, targetPoint, moveSpeed * Time.deltaTime);

        // Reached the target point — flip direction
        if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
        {
            targetPoint = targetPoint == pointB ? pointA : pointB;

            if (pauseDuration > 0f)
            {
                pausing = true;
                pauseTimer = pauseDuration;
            }
        }
    }
}
