using HarmonyLib;
using Verse;

namespace SCGF
{
    [StaticConstructorOnStartup]
    public static class SCGF
    {
        static SCGF()
        {
            GasLibrary.LoadCustomGasDefs();

            Harmony harmony = new Harmony(id: "rimworld.nachotoast.scgf.main");

            Patches_GasGrid.PatchAll(harmony);
            Patches_GasUtility.PatchAll(harmony);
            Patches_Map.PatchAll(harmony);
            Patches_Misc.PatchAll(harmony);
        }

    }
}
