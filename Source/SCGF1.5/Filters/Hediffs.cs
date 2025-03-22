using System.Collections.Generic;
using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s with any of the given <see cref="HediffDef"/>s
    /// </summary>
    public class Hediffs : GasFilter
    {
        private readonly List<HediffDef> hediffs = new List<HediffDef>();

        [Unsaved]
        private HashSet<HediffDef> hediffsSet;

        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            List<Hediff> hediffs = pawn.health?.hediffSet?.hediffs;

            if (hediffs == null)
            {
                return false;
            }

            foreach (Hediff hediff in hediffs)
            {
                if (hediffsSet.Contains(hediff.def))
                {
                    return true;
                }
            }

            return false;
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            hediffsSet = new HashSet<HediffDef>(hediffs);
        }
    }
}