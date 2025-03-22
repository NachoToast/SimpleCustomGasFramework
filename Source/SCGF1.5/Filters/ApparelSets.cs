using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s wearing any of the provided sets of apparel.
    /// </summary>
    public class ApparelSets : GasFilter
    {
        private readonly List<List<ThingDef>> sets = new List<List<ThingDef>>();

        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            HashSet<ThingDef> wornApparel = pawn.apparel?.WornApparel?.Select(apparel => apparel.def).ToHashSet();

            if (wornApparel == null)
            {
                return false;
            }

            foreach (List<ThingDef> apparelSet in sets)
            {
                if (wornApparel.ContainsEveryItemIn(apparelSet))
                {
                    return true;
                }
            }

            return false;
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (sets.Count == 0)
            {
                yield return "must have at least one set of apparel defined";
            }
        }
    }
}