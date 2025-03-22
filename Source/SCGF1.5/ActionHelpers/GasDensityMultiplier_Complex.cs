using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// Multiplies a severity multiplier by the gas density and a secondary multiplier.
    /// </summary>
    public class GasDensityMultiplier_Complex : GasDensityMultiplier
    {
        private readonly float multiplier;

        public GasDensityMultiplier_Complex(float multiplier)
        {
            this.multiplier = multiplier;
        }

        public override void DoEffects(Pawn pawn, byte gasDensity, ref float severityMultiplier, Hediff existingHediff)
        {
            severityMultiplier *= multiplier * (gasDensity / 255f);
        }
    }
}