using RimWorld;
using System.Collections.Generic;
using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// A helper class that multiplies a severity multiplier based on a <see cref="StatDef"/>.
    /// </summary>
    public abstract class StatFactorMultiplier : ISeverityModifier
    {
        protected readonly StatDef statDef;

        protected StatFactorMultiplier(StatDef statDef)
        {
            this.statDef = statDef;
        }

        public abstract void DoEffects(Pawn pawn, byte gasDensity, ref float severityMultiplier, Hediff existingHediff);

        /// <summary>
        /// Conditionally instantiates a <see cref="StatFactorMultiplier"/> that is most optimal for the given <paramref name="statDef"/> and <paramref name="multiplier"/>.
        /// </summary>
        public static void TryCreate(StatDef statDef, float multiplier, List<ISeverityModifier> list)
        {
            if (statDef == null || multiplier == 0f)
            {
                return;
            }

            if (multiplier == 1f)
            {
                list.Add(new StatFactorMultiplier_Simple(statDef));
            }
            else
            {
                list.Add(new StatFactorMultiplier_Complex(statDef, multiplier));
            }
        }
    }
}