using System.Collections.Generic;
using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s with one of the given <see cref="Gender"/>s.
    /// </summary>
    public class Genders : GasFilter
    {
        private readonly List<Gender> genders = new List<Gender>();

        [Unsaved]
        private HashSet<Gender> gendersSet;

        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            return gendersSet.Contains(pawn.gender);
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            gendersSet = new HashSet<Gender>(genders);
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (genders.Count == 0)
            {
                yield return "must have at least one gender defined";
            }
        }
    }
}