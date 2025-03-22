using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// Multiplies a severity multiplier by just the gas density.
    /// </summary>
    public class GasDensityMultiplier_Simple : GasDensityMultiplier
    {
        public override void DoEffects(Pawn pawn, byte gasDensity, ref float severityMultiplier, Hediff existingHediff)
        {
            severityMultiplier *= gasDensity / 255f;
        }
    }
}