using System.Collections.Generic;
using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s that are any of the given <see cref="PawnKindDef"/>s.
    /// </summary>
    public class PawnKinds : GasFilter
    {
        private readonly List<PawnKindDef> pawnKinds = new List<PawnKindDef>();

        [Unsaved]
        private HashSet<PawnKindDef> pawnKindsSet;

        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            if (pawn.kindDef == null)
            {
                return false;
            }

            return pawnKindsSet.Contains(pawn.kindDef);
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            pawnKindsSet = new HashSet<PawnKindDef>(pawnKinds);
        }
    }
}