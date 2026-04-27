using UnityEngine;


public class GunPart : MonoBehaviour
{
    [Header("Identity")]
    public string partName;
    public int assemblyOrder;
    public bool isAssembled = true;

    [Header("Free Build")]
    [Tooltip("Parts that MUST be assembled before this one can be placed")]
    public GunPart[] prerequisites;

    [HideInInspector] public Vector3 targetPosition;
    [HideInInspector] public Quaternion targetRotation;

    [Tooltip("How close the player must bring the part to its target to snap")]
    public float snapRadius = 0.08f;

    private Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void SaveTargetTransform()
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    public bool AreDependenciesMet()
    {
        if (prerequisites == null || prerequisites.Length == 0)
            return true;

        foreach (GunPart dep in prerequisites)
        {
            if (!dep.isAssembled)
                return false;
        }
        return true;
    }

    public bool IsNearTarget()
    {
        return Vector3.Distance(transform.position, targetPosition) <= snapRadius;
    }

    public void SnapToTarget()
    {
        transform.position = targetPosition;
        transform.rotation = targetRotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null && grab.isSelected)
        {
            grab.enabled = false;   
            grab.enabled = true;   
        }

        isAssembled = true;
    }

    public void SetOutline(string color)
    {
        if (renderers == null) return;

        foreach (Renderer r in renderers)
        {
            Color emissionColor = Color.black;
            if (color == "green") emissionColor = Color.green * 0.5f;
            else if (color == "red") emissionColor = Color.red * 0.5f;
            else if (color == "yellow") emissionColor = Color.yellow * 0.4f;

            r.material.SetColor("_EmissionColor", emissionColor);
            r.material.EnableKeyword("_EMISSION");
        }
    }

    public void ClearOutline() => SetOutline("none");
}