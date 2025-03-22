using RimWorld;
using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// Multiplies a severity multiplier by a <see cref="Pawn"/>'s <see cref="StatDef"/> value.
    /// </summary>
    public class StatFactorMultiplier_Simple : StatFactorMultiplier
    {
        public StatFactorMultiplier_Simple(StatDef statDef) : base(statDef)
        {
            //
        }

        public override void DoEffects(Pawn pawn, byte gasDensity, ref float severityMultiplier, Hediff existingHediff)
        {
            severityMultiplier *= 1f - pawn.GetStatValue(statDef);
        }
    }
}