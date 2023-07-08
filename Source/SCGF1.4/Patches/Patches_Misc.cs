using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SCGF
{
    /// <summary>
    /// Miscellaneous patches (I can't believe I spelt that right first try), such as handling gas spawns from explosions, adding
    /// custom gases to the UI inspector (bottom left text), and listing custom gases in the debug menu.
    /// </summary>
    public class Patches_Misc
    {
        private static readonly Type patchType = typeof(Patches_Misc);

        public static void PatchAll(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(Explosion), nameof(Explosion.StartExplosion)),
                postfix: new HarmonyMethod(patchType, nameof(StartExplosion_Postfix)));

           harmony.Patch(AccessTools.Method(typeof(MouseoverReadout), "DrawGas"), // private method
                postfix: new HarmonyMethod(patchType, nameof(DrawGas_Postfix)));


            harmony.Patch(AccessTools.Method(typeof(DebugToolsGeneral), "PushGas"), // private method
                postfix: new HarmonyMethod(patchType, nameof(PushGas_Postfix)));

        }

        /// <summary>
        /// Before doing an explosion first check if it uses the GasType 'Unused' (value 24), if it does then
        /// modify its GasType value to be the custom gas value (which we retrieve from the XML mod extension)
        /// so we know what it is when it later gets added to the gas grid.
        /// </summary>
        public static void StartExplosion_Postfix(Explosion __instance)
        {
            // skip all this logic if not using 'Unused' gas type, or if the explosion doesn't define a gas
            if (__instance.postExplosionGasType == null || __instance.postExplosionGasType != GasType.Unused)
            {
                return;
            }

            GasDef customGasToApply = null;

            if (__instance.projectile != null && __instance.projectile.HasModExtension<CustomGasExtension>())
            {
                // the projectile can be a mortar shell, thrown grenade, shell from a launcher, etc...
                customGasToApply = __instance.projectile.GetModExtension<CustomGasExtension>().gasDef;
            }
            else if (__instance.weapon != null && __instance.weapon.HasModExtension<CustomGasExtension>())
            {
                // the weapon can be a mortar, grenade, launcher, etc...
                customGasToApply = __instance.weapon.GetModExtension<CustomGasExtension>().gasDef;
            }
            else if (__instance.instigator != null && __instance.instigator.def.HasModExtension<CustomGasExtension>())
            {
                // the instigator can be a mortar shell (in item form), IED trap, etc...
                // it can also be the pawn (if they fired a launcher or mortar) but we don't care about that
                // since it would've been picked up by the first 'if' statement(for projectile)
                customGasToApply = __instance.instigator.def.GetModExtension<CustomGasExtension>().gasDef;
            }

            if (customGasToApply == null)
            {
                return;
            }

            if (customGasToApply.realGasType != null)
            {
                __instance.postExplosionGasType = customGasToApply.realGasType;
            }
            else
            {
                __instance.postExplosionGasType = (GasType)(GasLibrary.firstCustomGasIndex + customGasToApply.customIndex);
            }

        }

        /// <summary>
        /// The base 'DrawGas' method is used to add the inspector labels of each gas and its density to the bottom left of the screen.
        /// The method that calls DrawGas - MouseoverReadout.MouseoverReadoutOnGUI, is hard-coded to only draw the 3 vanilla gas types, so
        /// this detects the last call to DrawGas (which just happens to be drawing rot strink) and adds our own gases to the widget.
        /// </summary>
        public static void DrawGas_Postfix(GasType gasType, ref float curYOffset, Vector2 ___BotLeft)
        {
            if (gasType != GasType.RotStink)
            {
                return;
            }

            IntVec3 intVec = UI.MouseCell();

            int cellIndex = Find.CurrentMap.cellIndices.CellToIndex(intVec);

            ExtendedGasGrid gasGrid = (ExtendedGasGrid)Find.CurrentMap.gasGrid;

            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                GasDef gasDef = GasLibrary.customGassesArray[i];

                byte density = gasGrid.DensityAt(cellIndex, gasDef);
                if (density > 0)
                {
                    Widgets.Label(new Rect(___BotLeft.x, (float)UI.screenHeight - ___BotLeft.y - curYOffset, 999f, 999f), gasDef.label.CapitalizeFirst() + " " + ((float)(int)density / 255f).ToStringPercent("F0"));

                    curYOffset += 19f;
                }
            }
        }

        /// <summary>
        /// For ease of testing and developing, custom gases should be added to the 'Add Gas' debug menu list.
        /// </summary>
        public static void PushGas_Postfix(ref List<DebugActionNode> __result)
        {
            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                GasDef gasDef = GasLibrary.customGassesArray[i];

                __result.Add(new DebugActionNode(gasDef.label.CapitalizeFirst(), DebugActionType.ToolMap, delegate
                {
                    GasUtility.AddGas(UI.MouseCell(), Find.CurrentMap, (GasType)(GasLibrary.firstCustomGasIndex + gasDef.customIndex), 5f);
                }));
            }
        }
    }
}
