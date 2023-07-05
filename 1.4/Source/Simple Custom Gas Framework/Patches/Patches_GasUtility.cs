using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace SCGF
{
    /// <summary>
    /// Patches to the vanilla GasUtility class to add functionality to custom gases, like applying hediffs and toxic buildup.
    /// </summary>
    public static class Patches_GasUtility
    {
        private static readonly Type patchType = typeof(Patches_GasUtility);

        private static readonly Type gasUtility = typeof(GasUtility);

        public static void PatchAll(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(GasUtility), nameof(GasUtility.PawnGasEffectsTick)),
                postfix: new HarmonyMethod(patchType, nameof(PawnGasEffectsTick_Postfix_Custom)));

            if (ModsConfig.BiotechActive && GasLibrary.anyToxicCustomGasses)
            {
                harmony.Patch(AccessTools.Method(typeof(GasUtility), nameof(GasUtility.PawnGasEffectsTick)),
                    postfix: new HarmonyMethod(patchType, nameof(PawnGasEffectsTick_Postfix_Toxic)));
            }
        }

        /// <summary>
        /// Gases with the 'isToxic' property should give pawns toxic buildup hediff just like how tox gas does.
        /// </summary>
        public static void PawnGasEffectsTick_Postfix_Toxic(Pawn pawn)
        {
            if (!pawn.Spawned || !pawn.IsHashIntervalTick(50))
            {
                return;
            }

            float toxicFactor = 0f;

            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                GasDef gasDef = GasLibrary.customGassesArray[i];
                if (!gasDef.isToxic)
                {
                    continue;
                }

                byte gasDensity = pawn.Position.GasDentity(pawn.Map, (GasType)(GasLibrary.firstCustomGasIndex + i));
                if (gasDensity == 0)
                {
                    continue;
                }

                toxicFactor += (float)(int)gasDensity / 255f;

            }
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ToxicBuildup);
                if (firstHediffOfDef != null && firstHediffOfDef.CurStageIndex == firstHediffOfDef.def.stages.Count - 1)
                {
                    toxicFactor *= 0.25f;
                }

                GameCondition_ToxicFallout.DoPawnToxicDamage(pawn, protectedByRoof: false, toxicFactor);
        }

        /// <summary>
        /// Every 60 seconds, pawns should check the density of all gases in their current cell, and call relevant methods to
        /// conditionally apply gas effects if needed.
        /// </summary>
        public static void PawnGasEffectsTick_Postfix_Custom(Pawn pawn)
        {
            if (!pawn.Spawned || !pawn.IsHashIntervalTick(60))
            {
                return;
            }

            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                GasDef gasDef = GasLibrary.customGassesArray[i];

                byte gasDensity = pawn.Position.GasDentity(pawn.Map, (GasType)(GasLibrary.firstCustomGasIndex + i));
                if (gasDensity == 0)
                {
                    continue;
                }

                if (gasDef.Worker.IsAffectedByThisGas(pawn, gasDensity))
                {
                    gasDef.Worker.ApplyGasEffects(pawn, gasDensity);
                }
            }
        }
    }
}
