using Verse;

namespace SCGF
{
    /// <summary>
    /// Helper classes that modify a severity multiplier.
    /// </summary>
    public interface ISeverityModifier
    {
        /// <summary>
        /// Modifies the <paramref name="severityMultiplier"/> based on the given <paramref name="pawn"/>, <paramref name="gasDensity"/>, and <paramref name="existingHediff"/>.
        /// </summary>
        /// <remarks>
        /// Note that it is possible for <paramref name="existingHediff"/> to be <see langword="null"/>.
        /// </remarks>
        void DoEffects(Pawn pawn, byte gasDensity, ref float severityMultiplier, Hediff existingHediff);
    }
}