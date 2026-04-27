using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    public InstructionUI instructionUI;
    public float stepDelay = 2.5f;
    public Transform tableCenter;

    private AssemblyManager assemblyManager;
    private GunPart[] parts;
    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;

    void Start()
    {
        assemblyManager = GetComponent<AssemblyManager>();
    }

    public void BeginTutorial()
    {
        parts = assemblyManager.partsInOrder;

        originalPositions = new Vector3[parts.Length];
        originalRotations = new Quaternion[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            originalPositions[i] = parts[i].transform.position;
            originalRotations[i] = parts[i].transform.rotation;
        }

        StartCoroutine(RunTutorial());
    }

    IEnumerator RunTutorial()
    {
        instructionUI.ShowStep("Disassembly, watch carefully", 0, parts.Length);
        yield return new WaitForSeconds(1.5f);

        for (int i = parts.Length - 1; i >= 0; i--)
        {
            GunPart part = parts[i];
            part.SetOutline("red");
            instructionUI.ShowStep(
                $"Removing: {part.partName}",
                parts.Length - i,
                parts.Length);

            Vector3 tablePos = GetTablePosition(i);

            Quaternion flatRotation = Quaternion.Euler(
                0,
                originalRotations[i].eulerAngles.y,
                0);

            yield return StartCoroutine(
                AnimatePart(part.transform, tablePos, flatRotation, 0.8f));

            part.ClearOutline();
            yield return new WaitForSeconds(stepDelay - 0.8f);
        }

        yield return new WaitForSeconds(1.5f);
        // Phase 2 — Reassemble step by step
        instructionUI.ShowStep("Assembly, watch carefully", 0, parts.Length);
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < parts.Length; i++)
        {
            GunPart part = parts[i];
            part.SetOutline("green");
            instructionUI.ShowStep(
                $"Attach: {part.partName}",
                i,
                parts.Length);

            yield return StartCoroutine(
                AnimatePart(part.transform,
                originalPositions[i],
                originalRotations[i], 0.8f));

            part.ClearOutline();
            yield return new WaitForSeconds(stepDelay - 0.8f);
        }

        instructionUI.ShowComplete();
        yield return new WaitForSeconds(2.5f);
        GameManager.Instance.StartShooting();
    }

    IEnumerator AnimatePart(Transform part, Vector3 targetPos,
                             Quaternion targetRot, float duration)
    {
        Vector3 startPos = part.position;
        Quaternion startRot = part.rotation;

        Rigidbody rb = part.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Vector3 arcMid = (startPos + targetPos) / 2f + Vector3.up * 0.25f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float smoothT = t * t * (3f - 2f * t);

            Vector3 p1 = Vector3.Lerp(startPos, arcMid, smoothT);
            Vector3 p2 = Vector3.Lerp(arcMid, targetPos, smoothT);
            part.position = Vector3.Lerp(p1, p2, smoothT);
            part.rotation = Quaternion.Slerp(startRot, targetRot, smoothT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        part.position = targetPos;
        part.rotation = targetRot;
    }

    Vector3 GetTablePosition(int index)
    {
        float spacing = 0.15f;
        int cols = 8;

        float x = (index % cols) * spacing - (cols * spacing / 2f);
        float z = (index / cols) * spacing;

        Vector3 center = tableCenter != null ?
            tableCenter.position : transform.position;

        return new Vector3(
            center.x + x,
            center.y + 0.05f,
            center.z + z);
    }
}