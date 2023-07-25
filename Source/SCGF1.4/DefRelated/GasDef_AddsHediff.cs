using RimWorld;
using System.Collections.Generic;
using System.Linq;
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

        public List<BodyPartDef> partsToAffect = new List<BodyPartDef>();

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
            if (severityAdjustment == 0f) return;

            BodyPartRecord bodyPart = null;
            Hediff existingHediff = null;

            if (partsToAffect.Count > 0)
            {
                // choose a random part to effect
                IEnumerable<BodyPartRecord> potentialParts = pawn.health.hediffSet.GetNotMissingParts();
                potentialParts = potentialParts.Where((BodyPartRecord p) => partsToAffect.Contains(p.def));
                bodyPart = potentialParts.RandomElementByWeightWithFallback((BodyPartRecord x) => x.coverageAbs);

                existingHediff = pawn.health.hediffSet.GetFirstHediffMatchingPart<Hediff>(bodyPart);
            }
            else
            {
                existingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
            }

            if (existingHediff != null)
            {
                existingHediff.Severity += severityAdjustment;
                return;
            }

            if (severityAdjustment < 0f) return; // only add a new hediff for positive severity adjustments

            existingHediff = HediffMaker.MakeHediff(hediff, pawn);
            existingHediff.Severity = severityAdjustment;
            pawn.health.AddHediff(existingHediff, bodyPart);

        }
    }

}
