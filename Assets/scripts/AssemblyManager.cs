using UnityEngine;
using System.Collections;

public class AssemblyManager : MonoBehaviour
{
    public GunPart[] partsInOrder;
    public InstructionUI instructionUI;
    private int currentStep = 0;

    public float transitionDelay = 2.5f;

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
            part.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void TrySnapPart(GunPart part)
    {
        if (part == partsInOrder[currentStep])
        {
            part.SetOutline("green");
            part.GetComponent<Rigidbody>().isKinematic = true;
            part.isAssembled = true;
            instructionUI.ShowCorrect();
            StartCoroutine(AdvanceStep());
        }
        else
        {
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
            StartCoroutine(TransitionToShooting());
        }
        else
        {
            instructionUI.ShowStep(partsInOrder[currentStep].partName,
                                   currentStep, partsInOrder.Length);
            HighlightNextPart();
        }
    }

    IEnumerator TransitionToShooting()
    {
        yield return new WaitForSeconds(transitionDelay);
        GameManager.Instance.StartShooting();
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
