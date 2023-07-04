using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SCGF
{
    /// <summary>
    /// Patches to the vanilla GasGrid class to make it also use the custom gas grid - ExtendedGasGrid, for (most of) its methods.
    /// </summary>
    public static class Patches_GasGrid
    {
        private static readonly Type patchType = typeof(Patches_GasGrid);

        private static readonly Type gasGrid = typeof(GasGrid);

        public static void PatchAll(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.RecalculateEverHadGas)), postfix: new HarmonyMethod(patchType, nameof(RecalculateEverHadGas_Postfix)));

            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.AnyGasAt), new Type[] { typeof(int) }),
                postfix: new HarmonyMethod(patchType, nameof(AnyGasAt_Postfix)));

            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.DensityAt), new Type[] { typeof(int), typeof(GasType) }),
                prefix: new HarmonyMethod(patchType, nameof(DensityAt_Prefix)));

            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.AddGas)),
                prefix: new HarmonyMethod(patchType, nameof(AddGas_Prefix)));

            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.ColorAt)),
                prefix: new HarmonyMethod(patchType, nameof(ColorAt_Prefix)));

            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.Notify_ThingSpawned)),
                postfix: new HarmonyMethod(patchType, nameof(Notify_ThingSpawned_Postfix)));

            harmony.Patch(AccessTools.Method(gasGrid, "TryDissipateGasses"), // private method
                postfix: new HarmonyMethod(patchType, nameof(TryDissipateGasses_Postfix)));

            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.EqualizeGasThroughBuilding)),
                postfix: new HarmonyMethod(patchType, nameof(EqualizeGasThroughBuilding_Postfix)));

            harmony.Patch(AccessTools.Method(gasGrid, "TryDiffuseGasses"), // private method
                postfix: new HarmonyMethod(patchType, nameof(TryDiffuseGasses_Postfix)));

            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.Debug_FillAll)),
                postfix: new HarmonyMethod(patchType, nameof(Debug_FillAll_Postfix)));

            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.Debug_ClearAll)),
                postfix: new HarmonyMethod(patchType, nameof(Debug_ClearAll_Postfix)));

            harmony.Patch(AccessTools.Method(gasGrid, nameof(GasGrid.ExposeData)),
                postfix: new HarmonyMethod(patchType, nameof(ExposeData_Postfix)));
        }

        /// <summary>
        /// After checking for vanilla gasses, also check for custom ones if there were none found.
        /// </summary>
        public static void RecalculateEverHadGas_Postfix(ExtendedGasGrid __instance, ref bool ___anyGasEverAdded)
        {
            if (___anyGasEverAdded == false)
            {
                __instance.RecalculateEverHadGas(ref ___anyGasEverAdded);
            }
        }

        /// <summary>
        /// After checking for vanilla gasses at the cell, also check for custom ones if there were none found.
        /// </summary>
        public static void AnyGasAt_Postfix(ExtendedGasGrid __instance, int idx, ref bool __result)
        {
            if (__result == false)
            {
                __result = __instance.AnyGasAt(idx);
            }
        }

        /// <summary>
        /// If getting the density of a custom gas (indicated by gasType being greater than the max vanilla GasType value),
        /// only call the child method, since the parent method cannot use non-vanilla values.
        /// </summary>
        public static bool DensityAt_Prefix(ExtendedGasGrid __instance, int index, GasType gasType, ref byte __result)
        {
            if ((int)gasType < GasLibrary.firstCustomGasIndex)
            {
                return true;
            }

            __result = __instance.DensityAt(index, GasLibrary.GetCustomGasFromIndex(gasType));

            return false;
        }

        /// <summary>
        /// If adding a custom gas to a cell (indicated by gasType being greater than the max vanilla GasType value),
        /// only call the child method, since the parent will error if given an unrecognized value.
        /// </summary>
        public static bool AddGas_Prefix(ExtendedGasGrid __instance, IntVec3 cell, GasType gasType, int amount, ref bool ___anyGasEverAdded, bool canOverflow = true)
        {
            if ((int)gasType < GasLibrary.firstCustomGasIndex)
            {
                return true;
            }

            GasDef gasDef = GasLibrary.GetCustomGasFromIndex(gasType);

            __instance.AddGas(cell, gasDef, amount, ref ___anyGasEverAdded, canOverflow);

            return false;
        }

        /// <summary>
        /// Since calculating the colour of a cell requires knowledge of all the gasses present - both vanilla and custom,
        /// only call the child method, since the parent doesn't account for custom gasses.
        /// </summary>
        public static bool ColorAt_Prefix(ExtendedGasGrid __instance, IntVec3 cell, ref Color __result, ref FloatRange ___AlphaRange, ref Color ___SmokeColor, ref Color ___ToxColor, ref Color ___RotColor)
        {
            // since the child doesn't have access to the parents private class fields like the colour of each vanilla gas,
            // we need to access it using injections and pass them in manually
            __result = __instance.ColorAt(cell, ___AlphaRange, ___SmokeColor, ___ToxColor, ___RotColor);

            return false;
        }

        /// <summary>
        /// A thing spawning should affect custom gasses as well as vanilla ones.
        /// </summary>
        public static void Notify_ThingSpawned_Postfix(ExtendedGasGrid __instance, Thing thing)
        {
            __instance.Notify_ThingSpawned(thing);
        }

        /// <summary>
        /// A call to dissipate vanilla gasses should also try to dissipate custom gasses.
        /// </summary>
        public static void TryDissipateGasses_Postfix(ExtendedGasGrid __instance, int index)
        {
            __instance.TryDissipateGasses(index);
        }

        /// <summary>
        /// A call to equalize vanilla gasses should also try to equalize custom gasses.
        /// </summary>
        public static void EqualizeGasThroughBuilding_Postfix(ExtendedGasGrid __instance, Building b, bool twoWay)
        {
            __instance.EqualizeGasThroughBuilding(b, twoWay);
        }

        /// <summary>
        /// A call to diffuse vanilla gasses should also try to diffuse custom gasses.
        /// </summary>
        public static void TryDiffuseGasses_Postfix(ExtendedGasGrid __instance, IntVec3 cell, List<IntVec3> ___cardinalDirections)
        {
            __instance.TryDiffuseGasses(cell, ___cardinalDirections);
        }

        /// <summary>
        /// The debug 'Fill All Gas' option should also fill cells with custom gasses instead of just the vanilla ones.
        /// </summary>
        public static void Debug_FillAll_Postfix(ExtendedGasGrid __instance)
        {
            __instance.Debug_FillAll();
        }

        /// <summary>
        /// The debug 'Clear All Gas' option should also clear custom gasses instead of just the vanilla ones.
        /// </summary>
        public static void Debug_ClearAll_Postfix(ExtendedGasGrid __instance)
        {
            __instance.Debug_ClearAll();
        }

        /// <summary>
        /// When saving/loading the gas grid of a map, we want to make sure our custom gas grid properties (i.e. customGasDensity)
        /// are being saved/loaded as well.
        /// </summary>
        public static void ExposeData_Postfix(ExtendedGasGrid __instance)
        {
            // since this is called when loading a save, it's possible that the gas grid is the vanilla one
            // i.e. the map was generated and saved without this mod being present (aka the user is adding this mod to an existing save)
            // if that is the case then we don't need to do anything here, as there are other patches to handle
            // overwriting the vanilla gas grid with our extended one (Patches_Map.ExposeComponents_Postfix)
            if (__instance.GetType() == typeof(ExtendedGasGrid))
            {
                __instance.ExposeData();
            }
        }
    }
}
