using RimWorld;
using System.Collections.Generic;
using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s that are at any of the given <see cref="LifeStageDef"/>s.
    /// </summary>
    public class LifeStages : GasFilter
    {
        private readonly List<LifeStageDef> lifeStages = new List<LifeStageDef>();

        [Unsaved]
        private HashSet<LifeStageDef> lifeStagesSet;

        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            LifeStageDef currentLifeStage = pawn.ageTracker?.CurLifeStage;

            if (currentLifeStage == null)
            {
                return false;
            }

            return lifeStagesSet.Contains(currentLifeStage);
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            lifeStagesSet = new HashSet<LifeStageDef>(lifeStages);
        }
    }
}