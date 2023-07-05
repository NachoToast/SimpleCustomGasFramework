using HarmonyLib;
using System;
using Verse;

namespace SCGF
{
    /// <summary>
    /// Patches to the vanilla Map class to handle generating and loading the custom gas grid.
    /// </summary>
    public static class Patches_Map
    {
        private static readonly Type patchType = typeof(Patches_Map);

        private static readonly Type map = typeof(Map);

        public static void PatchAll(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(map, nameof(Map.ConstructComponents)),
                postfix: new HarmonyMethod(patchType, nameof(ConstructComponents_Postfix)));

            harmony.Patch(AccessTools.Method(map, "ExposeComponents"), // private method
                postfix: new HarmonyMethod(patchType, nameof(ExposeComponents_Postfix)));

        }

        /// <summary>
        /// When a map is being generated for the first time (notably NOT from loading a save), we want to replace its gas grid with
        /// our own custom gas grid class.
        /// </summary>
        public static void ConstructComponents_Postfix(Map __instance)
        {
            // although it won't directly use any of the methods on the 'ExtendedGasGrid' class (which is why GasGrid patches are still needed)
            // this allows us to define custom fields that can be used in patches (i.e. customGasDensity)
            __instance.gasGrid = new ExtendedGasGrid(__instance);
        }

        /// <summary>
        /// When a map is being loaded from a save, we want to ensure its gas grid is the modded gas grid.
        /// 
        /// We also want to make sure that it can handle the current number of custom gases, since its possible that the user has
        /// added or removed gases.
        /// </summary>
        public static void ExposeComponents_Postfix(Map __instance)
        {
            if (__instance.gasGrid.GetType() != typeof(ExtendedGasGrid))
            {
                __instance.gasGrid = new ExtendedGasGrid(__instance);
                return;
            }

            ExtendedGasGrid loadedGasGrid = (ExtendedGasGrid) __instance.gasGrid;
            int mapCellCount = __instance.cellIndices.NumGridCells;

            int numLoadedGasses = loadedGasGrid.customGasDensity.Length / mapCellCount;
            
            if (numLoadedGasses == GasLibrary.numCustomGasses)
            {
                return;
            }

            string noun = numLoadedGasses == 1 ? "type" : "types";
            Log.Warning(String.Format("[Simple Custom Gas Framework] {0} has {1} custom gas {3} on its grid, but there should be {2}, fixing...", __instance.ToString(), numLoadedGasses, GasLibrary.numCustomGasses, noun));

            loadedGasGrid.customGasDensity = new byte[mapCellCount * GasLibrary.numCustomGasses];
        }

    }
}
