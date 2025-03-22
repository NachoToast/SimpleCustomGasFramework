using HarmonyLib;
using PerformanceFish;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace SCGF
{
    [StaticConstructorOnStartup]
    public static class Patches
    {
        static Patches()
        {
            Harmony harmony = new Harmony("Nachotoast.SCGF");

            harmony.Patch(
                original: AccessTools.Method(
                    type: typeof(GasUtility),
                    name: nameof(GasUtility.PawnGasEffectsTick)),
                postfix: new HarmonyMethod(
                    methodType: typeof(Patches),
                    methodName: nameof(GasUtility_PawnGasEffectsTick_Postfix)));

            harmony.Patch(
                original: AccessTools.Method(
                    type: typeof(GasGrid),
                    name: nameof(GasGrid.ExposeData)),
                postfix: new HarmonyMethod(
                    methodType: typeof(Patches),
                    methodName: nameof(GasGrid_ExposeData_Postfix)));

            harmony.Patch(
                original: AccessTools.Method(
                    type: typeof(Explosion),
                    name: "AffectCell"),
                transpiler: new HarmonyMethod(
                    methodType: typeof(Patches),
                    methodName: nameof(Explosion_AffectCell_Transpiler)));
        }

        /// <summary>
        /// Do custom <see cref="GasDef"/> effects for every <paramref name="pawn"/> every 50 ticks.
        /// </summary>
        private static void GasUtility_PawnGasEffectsTick_Postfix(Pawn pawn)
        {
            if (!pawn.Spawned || !pawn.IsHashIntervalTick(50))
            {
                return;
            }

            GasGrid gasGrid = pawn.Map.gasGrid;

            if (gasGrid == null)
            {
                return;
            }

            IntVec3 cell = pawn.Position;

            foreach (GasDef gasDef in DefDatabase<GasDef>.AllDefsListForReading)
            {
                byte density = gasGrid.DensityAt(cell, gasDef);

                if (density > 0)
                {
                    gasDef.DoEffects(pawn, density);
                }
            }
        }

        /// <summary>
        /// Scribe custom <see cref="GasDef"/> densities in <see cref="GasGrid"/> data.
        /// </summary>
        private static void GasGrid_ExposeData_Postfix(GasGrid __instance)
        {
            Scribe.EnterNode("scgf_customGasDefs");

            foreach (GasDef gasDef in DefDatabase<GasDef>.AllDefsListForReading)
            {
                __instance.ScribeForDef(gasDef, gasDef.defName);
            }

            Scribe.ExitNode();
        }

        /// <summary>
        /// Insert logic for adding custom gases in <see cref="Explosion"/>.AffectCell
        /// </summary>
        private static IEnumerable<CodeInstruction> Explosion_AffectCell_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool successfullyDidPatch = false;

            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode != OpCodes.Ldarg_0)
                {
                    continue;
                }

                if (codes[i + 1].opcode != OpCodes.Ldflda)
                {
                    continue;
                }

                if (!(codes[i + 1].operand is FieldInfo fieldInfo))
                {
                    continue;
                }

                if (fieldInfo.DeclaringType != typeof(Explosion))
                {
                    continue;
                }

                if (fieldInfo.Name != nameof(Explosion.postExplosionGasType))
                {
                    continue;
                }

                codes[i].opcode = OpCodes.Nop;

                codes.InsertRange(i + 1, new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExplosionHelper), nameof(ExplosionHelper.TryAddGasFor))),
                    new CodeInstruction(OpCodes.Ldarg_0)
                });

                successfullyDidPatch = true;

                break;
            }

            if (!successfullyDidPatch)
            {
                Verse.Log.Error("[Simple Custom Gas Framework] Failed to find correct code to patch");
            }

            return codes.AsEnumerable();
        }
    }
}