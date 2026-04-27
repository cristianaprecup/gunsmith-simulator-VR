using UnityEngine;
using System.Collections;

public class AssemblyManager : MonoBehaviour
{
    public GunPart[] partsInOrder; // drag all 43 parts here in correct order
    public InstructionUI instructionUI;
    private int currentStep = 0;

    public void BeginChallenge()
    {
        currentStep = 0;
        DisassembleAll();
        instructionUI.ShowStep(partsInOrder[0].partName, 0, partsInOrder.Length);
        HighlightNextPart();
    }

    void DisassembleAll()
    {
        foreach (GunPart part in partsInOrder)
        {
            part.isAssembled = false;
            // move parts to table positions (set beforehand)
            part.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void TrySnapPart(GunPart part)
    {
        if (part == partsInOrder[currentStep])
        {
            // Correct!
            part.SetOutline("green");
            part.GetComponent<Rigidbody>().isKinematic = true;
            part.isAssembled = true;
            instructionUI.ShowCorrect();
            StartCoroutine(AdvanceStep());
        }
        else
        {
            // Wrong!
            part.SetOutline("red");
            instructionUI.ShowWrong(partsInOrder[currentStep].partName);
            StartCoroutine(ClearWrongOutline(part));
        }
    }

    IEnumerator AdvanceStep()
    {
        yield return new WaitForSeconds(0.8f);
        partsInOrder[currentStep].ClearOutline();
        currentStep++;

        if (currentStep >= partsInOrder.Length)
        {
            instructionUI.ShowComplete();
        }
        else
        {
            instructionUI.ShowStep(partsInOrder[currentStep].partName,
                                   currentStep, partsInOrder.Length);
            HighlightNextPart();
        }
    }

    IEnumerator ClearWrongOutline(GunPart part)
    {
        yield return new WaitForSeconds(1f);
        part.ClearOutline();
    }

    void HighlightNextPart()
    {
        partsInOrder[currentStep].SetOutline("green");
    }

    public void ResetAll()
    {
        currentStep = 0;
        foreach (GunPart part in partsInOrder)
            part.ClearOutline();
    }
}