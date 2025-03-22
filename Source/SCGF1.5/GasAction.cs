using Verse;

namespace SCGF
{
    /// <summary>
    /// A class that runs on a given <see cref="Pawn"/> when their current cell's gas density is > 0
    /// </summary>
    public abstract class GasAction : Editable
    {
        public abstract void DoEffects(Pawn pawn, byte gasDensity);
    }
}