using Verse;

namespace SCGF
{
    /// <summary>
    /// A class that decides whether a given <see cref="Pawn"/> should be affected by a gas.
    /// </summary>
    /// <remarks>
    /// This class can be used for both <see cref="GasDef.appliesTo"/> and <see cref="GasDef.immunityWhen"/>.
    /// </remarks>
    public abstract class GasFilter : Editable
    {
        public abstract bool ShouldApplyTo(Pawn pawn, byte gasDensity);
    }
}