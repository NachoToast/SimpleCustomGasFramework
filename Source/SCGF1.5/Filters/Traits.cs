using RimWorld;
using System.Collections.Generic;
using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s with any of the given <see cref="TraitDef"/>s.
    /// </summary>
    public class Traits : GasFilter
    {
        private readonly List<TraitDef> traits = new List<TraitDef>();

        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            TraitSet pawnTraits = pawn.story?.traits;

            if (pawnTraits == null)
            {
                return false;
            }

            foreach (TraitDef trait in traits)
            {
                if (pawnTraits.HasTrait(trait))
                {
                    return true;
                }
            }

            return false;
        }
    }
}