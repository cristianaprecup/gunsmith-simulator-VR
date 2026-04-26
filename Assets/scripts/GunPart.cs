using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunPart : MonoBehaviour
{
    public string partName;
    public int assemblyOrder;
    public bool isAssembled = true;

    private Renderer[] renderers;
    private Material[] originalMaterials;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetOutline(string color)
    {
        foreach (Renderer r in renderers)
        {
            if (color == "green")
                r.material.SetColor("_EmissionColor", Color.green * 0.5f);
            else if (color == "red")
                r.material.SetColor("_EmissionColor", Color.red * 0.5f);
            else
                r.material.SetColor("_EmissionColor", Color.black);

            r.material.EnableKeyword("_EMISSION");
        }
    }

    public void ClearOutline() => SetOutline("none");
}