using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s wearing at least one item in each of the given <see cref="ProtectiveApparelDef"/>s.
    /// </summary>
    public class ProtectiveApparel : GasFilter
    {
        private readonly List<ProtectiveApparelDef> types = new List<ProtectiveApparelDef>();

        [Unsaved]
        private HashSet<ProtectiveApparelDef> typesSet;

        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            List<Apparel> wornApparel = pawn.apparel?.WornApparel;

            if (wornApparel == null)
            {
                return false;
            }

            HashSet<ProtectiveApparelDef> remainingTypes = new HashSet<ProtectiveApparelDef>(typesSet);

            foreach (Apparel apparel in wornApparel)
            {
                bool removedAnyTypes = false;

                foreach (ProtectiveApparelDef type in remainingTypes.ToList())
                {
                    if (type.apparelSet.Contains(apparel.def))
                    {
                        remainingTypes.Remove(type);
                        removedAnyTypes = true;
                    }
                }

                if (removedAnyTypes && remainingTypes.Count == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            typesSet = new HashSet<ProtectiveApparelDef>(types);
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (types.Count == 0)
            {
                yield return $"please specify at least one {nameof(ProtectiveApparelDef)}";
            }
        }
    }
}