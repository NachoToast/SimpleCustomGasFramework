using System.Collections.Generic;
using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// A helper class that chooses a <see cref="BodyPartRecord"/> on a given <see cref="Pawn"/>.
    /// </summary>
    public abstract class BodyPartChooser
    {
        /// <summary>
        /// Chooses a <see cref="BodyPartRecord"/> on the given <paramref name="pawn"/>.
        /// </summary>
        /// <remarks>
        /// A <see langword="null"/> return value indicates the whole body.
        /// </remarks>
        public abstract BodyPartRecord Choose(Pawn pawn);

        /// <summary>
        /// Instantiates a <see cref="BodyPartChooser"/> that is most optimal for the given <paramref name="bodyPartDefs"/>.
        /// </summary>
        public static BodyPartChooser Create(List<BodyPartDef> bodyPartDefs)
        {
            if (bodyPartDefs.Count == 0)
            {
                return new BodyPartChooser_None();
            }

            return new BodyPartChooser_Random(bodyPartDefs);
        }
    }
}