using UnityEngine;

/// <summary>
/// Editor helper: auto-assigns prerequisites for free build mode.
/// 
/// Default behavior: each part depends on the part before it in
/// assemblyOrder. This gives you a simple linear chain that you can
/// then customize in the Inspector for parts that can be placed
/// independently.
/// 
/// Usage: put this on the same GameObject as FreeBuildManager,
/// right-click → "Auto-Setup Prerequisites", then tweak as needed.
/// </summary>
public class AutoSetupPrerequisites : MonoBehaviour
{
    public FreeBuildManager freeBuildManager;

    [Tooltip("If true, each part depends on the previous part only. " +
             "If false, each part depends on ALL prior parts.")]
    public bool singleChainMode = true;

    [ContextMenu("Auto-Setup Prerequisites (Linear Chain)")]
    void SetupLinearChain()
    {
        GunPart[] parts = freeBuildManager.partsInOrder;
        if (parts == null || parts.Length == 0)
        {
            Debug.LogWarning("No parts found in FreeBuildManager.");
            return;
        }

        // First part has no prerequisites
        parts[0].prerequisites = new GunPart[0];

        for (int i = 1; i < parts.Length; i++)
        {
            if (singleChainMode)
            {
                // Each part only needs the one before it
                parts[i].prerequisites = new GunPart[] { parts[i - 1] };
            }
            else
            {
                // Each part needs ALL prior parts
                GunPart[] deps = new GunPart[i];
                for (int j = 0; j < i; j++)
                    deps[j] = parts[j];
                parts[i].prerequisites = deps;
            }
        }

        // Mark dirty for serialization
#if UNITY_EDITOR
        foreach (GunPart part in parts)
            UnityEditor.EditorUtility.SetDirty(part);
        Debug.Log($"Prerequisites set for {parts.Length} parts " +
                  $"({(singleChainMode ? "single chain" : "cumulative")}).");
#endif
    }

    [ContextMenu("Clear All Prerequisites")]
    void ClearAll()
    {
        GunPart[] parts = freeBuildManager.partsInOrder;
        foreach (GunPart part in parts)
            part.prerequisites = new GunPart[0];

#if UNITY_EDITOR
        foreach (GunPart part in parts)
            UnityEditor.EditorUtility.SetDirty(part);
        Debug.Log("All prerequisites cleared.");
#endif
    }
}
