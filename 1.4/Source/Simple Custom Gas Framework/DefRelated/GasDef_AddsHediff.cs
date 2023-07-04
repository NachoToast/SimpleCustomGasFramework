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

        public float severityGasDensityFactor = 0.05f;

        public StatDef exposureStatFactor = null;

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
