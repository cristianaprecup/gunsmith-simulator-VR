using UnityEngine;
using System.Collections.Generic;

public class RenameGunParts : MonoBehaviour
{
    [ContextMenu("Rename All Parts")]
    void RenameAll()
    {
        Dictionary<string, string> names = new Dictionary<string, string>
        {
            {"mag_low", "Magazine"},
            {"patr_low", "Cartridge"},
            {"prikl_low", "Stock"},
            {"ruch_low", "Pistol Grip"},
            {"dulo_low", "Barrel"},
            {"vtulk_low", "Barrel Bushing"},
            {"pruj_low", "Recoil Spring"},
            {"spusk_low", "Trigger Group"},
            {"spusk_low.001", "Trigger Group"},
            {"crishk_low", "Dust Cover"},
            {"crishk_low.001", "Dust Cover"},
            {"kr_low", "Upper Handguard"},
            {"kr2_low", "Lower Handguard"},
            {"per_low", "Gas Tube Pin"},
            {"pricel_low", "Rear Sight"},
            {"pricelw_low", "Front Sight"},
            {"prik_low", "Sight Rail"},
            {"prik_low.001", "Sight Rail B"},
            {"shompol_low", "Cleaning Rod"},
            {"knife_low", "Bayonet"},
            {"met_low", "Serial Plate"},
            {"f1_low", "Fire Selector A"},
            {"f2_low", "Fire Selector B"},
            {"f3_low", "Fire Selector C"},
            {"f4_low", "Fire Selector D"},
            {"d1_low", "Bolt Carrier"},
            {"d2_low", "Bolt"},
            {"d3_low", "Firing Pin"},
            {"d4_low", "Gas Piston"},
            {"d5_low", "Return Spring"},
            {"d6_low", "Spring Guide"},
            {"d7_low", "Buffer"},
            {"pd1_low", "Pin A"},
            {"pd2_low", "Pin B"},
            {"pd3_low", "Pin C"},
            {"pd3_low.001", "Pin C"},
            {"2_low", "Receiver Pin A"},
            {"2_low.001", "Receiver Pin B"},
            {"3_low.001", "Grip Screw"},
            {"4_low", "Trigger Pin"},
            {"5_low", "Hammer Pin"},
            {"6_low", "Safety Lever"},
            {"Dop1_low", "Muzzle Nut"},
            {"Dop1_low.001", "Muzzle Nut"},
            {"Dop2_low", "Flash Hider"},
            {"Dop2_low.001", "Flash Hider"},
            {"Dop3_low", "Handguard Cap"},
            {"Dop5_low", "Sling Mount"},
            {"Dop6_low", "Buttplate"},
        };

        MeshRenderer[] parts = GetComponentsInChildren<MeshRenderer>();
        int count = 0;
        foreach (MeshRenderer r in parts)
        {
            string original = r.gameObject.name;
            if (names.ContainsKey(original))
            {
                r.gameObject.name = names[original];
                GunPart gp = r.gameObject.GetComponent<GunPart>();
                if (gp != null) gp.partName = names[original];
                count++;
            }
        }
    }
}