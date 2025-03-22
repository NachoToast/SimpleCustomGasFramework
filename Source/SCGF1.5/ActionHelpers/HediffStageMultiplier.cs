using System.Collections.Generic;
using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// A helper class that modifies a severity multiplier based on a <see cref="Hediff"/>'s stage.
    /// </summary>

    public class HediffStageMultiplier : ISeverityModifier
    {
        private readonly int lastStageIndex;

        private readonly float multiplier;

        protected HediffStageMultiplier(HediffDef def, float multiplier)
        {
            lastStageIndex = def.stages.Count - 1;

            this.multiplier = multiplier;
        }

        public void DoEffects(Pawn pawn, byte gasDensity, ref float severityMultiplier, Hediff existingHediff)
        {
            if (existingHediff?.CurStageIndex == lastStageIndex)
            {
                severityMultiplier *= multiplier;
            }
        }

        /// <summary>
        /// Conditionally instantiates a <see cref="HediffStageMultiplier"/> that is most optimal for the given <paramref name="hediffDef"/> and <paramref name="multiplier"/>.
        /// </summary>
        public static void TryCreate(HediffDef hediffDef, float multiplier, List<ISeverityModifier> list)
        {
            if (multiplier == 1f)
            {
                return;
            }

            list.Add(new HediffStageMultiplier(hediffDef, multiplier));
        }
    }
}