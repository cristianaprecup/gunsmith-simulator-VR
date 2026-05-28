using UnityEngine;


public class GunStateManager : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable rootGrab;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable[] partGrabs;

    void Start()
    {
        rootGrab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        partGrabs = GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        SetAssembledMode();
    }

    // Call this when gun is assembled — whole gun grabbable
    public void SetAssembledMode()
    {
        // Enable root grab
        rootGrab.enabled = true;

        // Disable all individual part grabs
        foreach (UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab in partGrabs)
        {
            if (grab != rootGrab)
                grab.enabled = false;
        }
    }

    // Call this when challenge starts — parts individually grabbable
    public void SetDisassembledMode()
    {
        // Disable root grab
        rootGrab.enabled = false;

        // Enable all individual part grabs
        foreach (UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab in partGrabs)
        {
            if (grab != rootGrab)
                grab.enabled = true;
        }
    }
}