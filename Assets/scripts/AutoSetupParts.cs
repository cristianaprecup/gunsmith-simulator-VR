using UnityEngine;

public class AutoSetupParts : MonoBehaviour
{
    public GameObject ak47Root;

    [ContextMenu("Setup All Parts")]
    void Setup()
    {
        MeshRenderer[] renderers = ak47Root
            .GetComponentsInChildren<MeshRenderer>();

        AssemblyManager manager = GetComponent<AssemblyManager>();
        manager.partsInOrder = new GunPart[renderers.Length];

        int index = 0;
        foreach (MeshRenderer r in renderers)
        {
            GunPart part = r.gameObject.GetComponent<GunPart>();
            if (part == null)
                part = r.gameObject.AddComponent<GunPart>();

            part.partName = r.gameObject.name;
            part.assemblyOrder = index;
            manager.partsInOrder[index] = part;
            index++;
        }
    }
}