using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s that are Anomaly entities.
    /// </summary>
    public class Anomalies : GasFilter
    {
        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            return pawn.RaceProps?.IsAnomalyEntity == true;
        }
    }
}