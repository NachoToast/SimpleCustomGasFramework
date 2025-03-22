using RimWorld;
using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// Multiplies a severity multiplier by a <see cref="Pawn"/>'s <see cref="StatDef"/> value and a secondary multiplier.
    /// </summary>
    public class StatFactorMultiplier_Complex : StatFactorMultiplier
    {
        private readonly float multiplier;

        public StatFactorMultiplier_Complex(StatDef statDef, float multiplier) : base(statDef)
        {
            this.multiplier = multiplier;
        }

        public override void DoEffects(Pawn pawn, byte gasDensity, ref float severityMultiplier, Hediff existingHediff)
        {
            severityMultiplier *= multiplier - (multiplier * pawn.GetStatValue(statDef));
        }
    }
}