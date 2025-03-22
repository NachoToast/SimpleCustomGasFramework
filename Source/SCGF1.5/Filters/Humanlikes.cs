using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s that are humanlike.
    /// </summary>
    public class Humanlikes : GasFilter
    {
        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            return pawn.RaceProps?.Humanlike == true;
        }
    }
}