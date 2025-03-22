using Verse;

namespace SCGF.ActionHelpers
{
    /// <summary>
    /// Always chooses the whole body (i.e. a <see langword="null"/> <see cref="BodyPartRecord"/>).
    /// </summary>
    public class BodyPartChooser_None : BodyPartChooser
    {
        public override BodyPartRecord Choose(Pawn pawn)
        {
            return null;
        }
    }
}