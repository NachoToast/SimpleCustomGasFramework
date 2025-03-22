using System.Collections.Generic;
using Verse;

namespace SCGF.Filters
{
    /// <summary>
    /// Filter to only <see cref="Pawn"/>s with any of the given <see cref="GeneDef"/>s.
    /// </summary>
    public class Genes : GasFilter
    {
        private readonly List<GeneDef> genes = new List<GeneDef>();

        [Unsaved]
        private HashSet<GeneDef> genesSet;

        public override bool ShouldApplyTo(Pawn pawn, byte gasDensity)
        {
            List<Gene> genes = pawn.genes?.GenesListForReading;

            if (genes == null)
            {
                return false;
            }

            foreach (Gene gene in genes)
            {
                if (genesSet.Contains(gene.def))
                {
                    return true;
                }
            }

            return false;
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            genesSet = new HashSet<GeneDef>(genes);
        }
    }
}