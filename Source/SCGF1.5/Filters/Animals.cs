using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s that are animals.
    /// </summary>
    public class Animals : GasFilter
    {
        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            return pawn.RaceProps?.Animal == true;
        }
    }
}