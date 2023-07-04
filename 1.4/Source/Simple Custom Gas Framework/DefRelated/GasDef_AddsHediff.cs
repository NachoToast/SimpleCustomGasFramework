using RimWorld;
using Verse;

namespace SCGF
{
    /// <summary>
    /// This defines the 'addsHediffs' list items of a GasDef.
    /// </summary>
    public class GasDef_AddsHediff
    {
        public HediffDef hediff;

        // hediff severity increase is calculated by this value * gas density %
        // e.g. a value of 0.05f (default) means the severity of the hediff increases by 0.05 every second at 100% density, 0.025 at 50% density, etc...
        public float severityGasDensityFactor = 0.05f;

        // on top of the severityGasDensityFactor (see above), hediff severity increase also takes this stat into account
        // e.g. if its value is 75% for a pawn, their severity increase will only be 25% of what you would expect
        public StatDef exposureStatFactor = null;

        // if a pawns severity for this hediff is at its final stage, further severity increases get multiplied by this amount
        // in vanilla, Tox Gas uses this with a value of 0.25 (default)
        public float finalStageMultiplier = 0.25f;

        public float GetSeverityAdjustment(Pawn pawn, byte gasDensity)
        {
            float adjustment = gasDensity / 255f * severityGasDensityFactor;

            if (exposureStatFactor != null)
            {
                adjustment *= 1f - pawn.GetStatValue(exposureStatFactor);
            }

            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);

            if (firstHediffOfDef != null && firstHediffOfDef.CurStageIndex == firstHediffOfDef.def.stages.Count - 1)
            {
                adjustment *= 0.25f;
            }

            return adjustment;
        }

        public void ApplyHediffToPawn(Pawn pawn, byte gasDensity)
        {
            float severityAdjustment = GetSeverityAdjustment(pawn, gasDensity);

            HealthUtility.AdjustSeverity(pawn, hediff, severityAdjustment);
        }
    }

}
