using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s wearing at least one item in each of the given <see cref="ApparelTierDef"/>s.
    /// </summary>
    public class ApparelTiers : GasFilter
    {
        private readonly List<ApparelTierDef> tierDefs = new List<ApparelTierDef>();

        [Unsaved]
        private HashSet<ApparelTierDef> tierDefsSet;

        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            List<Apparel> wornApparel = pawn.apparel?.WornApparel;

            if (wornApparel == null)
            {
                return false;
            }

            HashSet<ApparelTierDef> remainingTiers = new HashSet<ApparelTierDef>(tierDefsSet);

            foreach (Apparel apparel in wornApparel)
            {
                bool removedAnyTiers = false;

                foreach (ApparelTierDef tier in remainingTiers.ToList())
                {
                    if (tier.apparelSet.Contains(apparel.def))
                    {
                        remainingTiers.Remove(tier);
                        removedAnyTiers = true;
                    }
                }

                if (removedAnyTiers && remainingTiers.Count == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            tierDefsSet = new HashSet<ApparelTierDef>(tierDefs);
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (tierDefs.Count == 0)
            {
                yield return $"please specify at least one {nameof(ApparelTierDef)}";
            }
        }
    }
}