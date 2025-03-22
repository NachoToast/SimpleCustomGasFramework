using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s that are insects.
    /// </summary>
    public class Insects : GasFilter
    {
        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            return pawn.RaceProps?.Insect == true;
        }
    }
}