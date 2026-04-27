using UnityEngine;

public class AssignMaterials : MonoBehaviour
{
    public Material material1;
    public Material material2;

    [ContextMenu("Assign To All Children")]
    void AssignToAll()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            if (r.materials.Length == 2)
                r.materials = new Material[] { material1, material2 };
            else
                r.materials = new Material[] { material1 };
        }
    }
}