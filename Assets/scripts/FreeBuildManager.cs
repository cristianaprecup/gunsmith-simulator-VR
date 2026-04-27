using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using TMPro;

public class FreeBuildManager : MonoBehaviour
{
    [Header("References")]
    public GunPart[] partsInOrder;
    public GameObject instructionsPanel;
    public TextMeshProUGUI stepText;
    public Transform tableCenter;

    [Header("Settings")]
    public float snapCheckInterval = 0.1f;

    private int currentStep = 0;
    private bool isActive = false;
    private GunPart currentlyGrabbedPart = null;
    private GameObject currentGhost = null;
    
    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;

    void Start()
    {
        originalPositions = new Vector3[partsInOrder.Length];
        originalRotations = new Quaternion[partsInOrder.Length];
        
        for (int i = 0; i < partsInOrder.Length; i++)
        {
            originalPositions[i] = partsInOrder[i].transform.position;
            originalRotations[i] = partsInOrder[i].transform.rotation;
        }

        if (instructionsPanel != null) instructionsPanel.SetActive(false);
    }

    void Update()
    {
        if (isActive && currentlyGrabbedPart != null && currentStep > 0)
        {
            if (currentlyGrabbedPart.IsNearTarget())
            {
                currentlyGrabbedPart.SetOutline("green");
            }
            else
            {
                currentlyGrabbedPart.SetOutline("yellow");
            }
        }
    }

    public void BeginFreeBuild()
    {
        DestroyGhost();
        currentStep = 0;
        currentlyGrabbedPart = null;

        if (instructionsPanel != null) instructionsPanel.SetActive(true);

        for (int i = 0; i < partsInOrder.Length; i++)
        {
            partsInOrder[i].transform.position = originalPositions[i];
            partsInOrder[i].transform.rotation = originalRotations[i];
            partsInOrder[i].SaveTargetTransform();
        }

        for (int i = 0; i < partsInOrder.Length; i++)
        {
            GunPart part = partsInOrder[i];
            part.isAssembled = false;
            part.ClearOutline();

            part.transform.position = GetTablePosition(i);
            part.transform.rotation = part.targetRotation;

            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            Collider col = part.GetComponent<Collider>();
            if (col != null) col.isTrigger = false;

            var grab = part.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab != null)
            {
                grab.enabled = true;
                grab.selectEntered.RemoveListener(OnPartGrabbed);
                grab.selectEntered.AddListener(OnPartGrabbed);
                grab.selectExited.RemoveListener(OnPartReleased);
                grab.selectExited.AddListener(OnPartReleased);
            }
        }

        UpdateStepUI();
        HighlightNextPart();

        isActive = true;
    }

    public void StopFreeBuild()
    {
        isActive = false;
        currentlyGrabbedPart = null;
        DestroyGhost();

        if (instructionsPanel != null) instructionsPanel.SetActive(false);

        foreach (GunPart part in partsInOrder)
        {
            var grab = part.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab != null)
            {
                grab.selectExited.RemoveListener(OnPartReleased);
                grab.selectEntered.RemoveListener(OnPartGrabbed);
            }
            part.ClearOutline();
        }

        if (stepText != null) stepText.text = "";
    }

    public void ResetFreeBuild()
    {
        StopFreeBuild();
        BeginFreeBuild();
    }

    private void OnPartGrabbed(SelectEnterEventArgs args)
    {
        GunPart part = args.interactableObject.transform.GetComponent<GunPart>();
        if (part == null || part.isAssembled) return;

        Rigidbody rb = part.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        if (part == partsInOrder[currentStep])
        {
            currentlyGrabbedPart = part;
            if (currentStep > 0)
            {
                CreateGhost(part);
            }
        }
    }

    private void OnPartReleased(SelectExitEventArgs args)
    {
        DestroyGhost();

        if (!isActive) return;

        GunPart part = args.interactableObject.transform.GetComponent<GunPart>();
        if (part == null || part.isAssembled) return;

        if (part == currentlyGrabbedPart)
        {
            currentlyGrabbedPart = null;
        }

        if (part == partsInOrder[currentStep])
        {
            if (currentStep == 0)
            {
                part.isAssembled = true;
                part.SetOutline("green");
                StartCoroutine(ClearOutlineAfterDelay(part, 0.6f));

                Rigidbody firstRb = part.GetComponent<Rigidbody>();
                if (firstRb != null)
                {
                    firstRb.isKinematic = false;
                    firstRb.useGravity = true;
                }

                StartCoroutine(SettleFirstPart(part));
            }
            else
            {
                if (part.IsNearTarget())
                {
                    part.SnapToTarget();
                    part.isAssembled = true;
                    
                    Collider col = part.GetComponent<Collider>();
                    if (col != null) col.isTrigger = true;

                    part.SetOutline("green");
                    StartCoroutine(ClearOutlineAfterDelay(part, 0.6f));

                    currentStep++;

                    if (currentStep >= partsInOrder.Length) OnComplete();
                    else
                    {
                        UpdateStepUI();
                        HighlightNextPart();
                    }
                }
                else
                {
                    part.SetOutline("yellow");
                }
            }
        }
        else
        {
            part.SetOutline("red");
            StartCoroutine(ClearOutlineAfterDelay(part, 1f));
        }

        if (!part.isAssembled)
        {
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }

    private void CreateGhost(GunPart part)
    {
        if (currentGhost != null) Destroy(currentGhost);

        currentGhost = Instantiate(part.gameObject, part.targetPosition, part.targetRotation);
        
        foreach (var col in currentGhost.GetComponentsInChildren<Collider>()) Destroy(col);
        
        Rigidbody rb = currentGhost.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);
        
        var grab = currentGhost.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null) Destroy(grab);
        
        GunPart gunPart = currentGhost.GetComponent<GunPart>();
        if (gunPart != null) Destroy(gunPart);

        Material ghostMat = new Material(Shader.Find("Unlit/Transparent"));
        
        ghostMat.color = new Color(0f, 1f, 0f, 0.15f); 

        foreach (Renderer r in currentGhost.GetComponentsInChildren<Renderer>())
        {
            r.material = ghostMat;
        }
    }

    private void DestroyGhost()
    {
        if (currentGhost != null)
        {
            Destroy(currentGhost);
            currentGhost = null;
        }
    }

    IEnumerator SettleFirstPart(GunPart part)
    {
        yield return new WaitForSeconds(1.5f);

        Rigidbody rb = part.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = part.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        Quaternion targetRotOffset = part.transform.rotation * Quaternion.Inverse(originalRotations[0]);
                
        for (int i = 1; i < partsInOrder.Length; i++)
        {
            Vector3 currentTablePos = partsInOrder[i].transform.position;
            Quaternion currentTableRot = partsInOrder[i].transform.rotation;

            Vector3 localOffset = originalPositions[i] - originalPositions[0];
            partsInOrder[i].transform.position = part.transform.position + targetRotOffset * localOffset;
            partsInOrder[i].transform.rotation = targetRotOffset * originalRotations[i];

            partsInOrder[i].SaveTargetTransform();

            partsInOrder[i].transform.position = currentTablePos;
            partsInOrder[i].transform.rotation = currentTableRot;
        }

        currentStep++;
        if (currentStep >= partsInOrder.Length) OnComplete();
        else
        {
            UpdateStepUI();
            HighlightNextPart();
        }
    }

    private void UpdateStepUI()
    {
        if (stepText != null)
            stepText.text = $"Step {currentStep + 1} of {partsInOrder.Length}\nAttach: {partsInOrder[currentStep].partName}";
    }

    private void HighlightNextPart()
    {
        partsInOrder[currentStep].SetOutline("yellow");
    }

    private void OnComplete()
    {
        isActive = false;
        if (stepText != null) stepText.text = "<color=green>Assembly Complete!</color>";

        foreach (GunPart part in partsInOrder)
            part.ClearOutline();
    }

    IEnumerator ClearOutlineAfterDelay(GunPart part, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!part.isAssembled && part == partsInOrder[currentStep])
            part.SetOutline("yellow");
        else if (!part.isAssembled)
            part.ClearOutline();
    }

    Vector3 GetTablePosition(int index)
    {
        float spacing = 0.18f;
        int cols = 8;
        float x = (index % cols) * spacing - (cols * spacing / 2f);
        float z = (index / cols) * spacing;

        float offsetRight = 1f;
        float offsetBack = -0.10f;

        Vector3 center = tableCenter != null ? tableCenter.position : transform.position;
        return new Vector3(center.x + x + offsetRight, center.y + 0.05f, center.z + z + offsetBack);
    }
}