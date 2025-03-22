using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s that have organic flesh.
    /// </summary>
    public class Organic : GasFilter
    {
        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            return pawn.RaceProps?.IsFlesh == true;
        }
    }
}