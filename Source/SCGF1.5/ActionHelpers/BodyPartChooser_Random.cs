using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// Chooses a random, not-missing <see cref="BodyPartRecord"/> based on an allowed list of <see cref="BodyPartDef"/>s and coverage.
    /// </summary>
    public class BodyPartChooser_Random : BodyPartChooser
    {
        private readonly HashSet<BodyPartDef> potentialBodyPartDefs;

        public BodyPartChooser_Random(List<BodyPartDef> bodyPartDefs)
        {
            potentialBodyPartDefs = new HashSet<BodyPartDef>(bodyPartDefs);
        }

        public override BodyPartRecord Choose(Pawn pawn)
        {
            return pawn.health?.hediffSet?
                .GetNotMissingParts()
                .Where(part => potentialBodyPartDefs.Contains(part.def))
                .RandomElementByWeightWithFallback(part => part.coverageAbs);
        }
    }
}