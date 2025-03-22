using System.Collections.Generic;
using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// A helper class that multiplies a severity multiplier based on gas density.
    /// </summary>
    public abstract class GasDensityMultiplier : ISeverityModifier
    {
        public abstract void DoEffects(Pawn pawn, byte gasDensity, ref float severityMultiplier, Hediff existingHediff);

        /// <summary>
        /// Conditionally instantiates a <see cref="GasDensityMultiplier"/> that is most optimal for the given <paramref name="multiplier"/>.
        /// </summary>
        public static void TryCreate(float multiplier, List<ISeverityModifier> list)
        {
            if (multiplier == 0f)
            {
                return;
            }

            if (multiplier == 1f)
            {
                list.Add(new GasDensityMultiplier_Simple());
            }
            else
            {
                list.Add(new GasDensityMultiplier_Complex(multiplier));
            }
        }
    }
}