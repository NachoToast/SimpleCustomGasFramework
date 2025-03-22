using RimWorld;
using Verse;

namespace SCGF.Actions
{
    public class DoToxicDamage : GasAction
    {
        public override void DoEffects(Pawn pawn, byte gasDensity)
        {
            float toxicFactor = gasDensity / 255f;

            Hediff firstHediffOfDef = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf.ToxicBuildup);

            if (firstHediffOfDef != null && firstHediffOfDef.CurStageIndex == firstHediffOfDef.def.stages.Count)
            {
                toxicFactor *= 0.25f;
            }

            GameCondition_ToxicFallout.DoPawnToxicDamage(pawn, false, toxicFactor);
        }
    }
}