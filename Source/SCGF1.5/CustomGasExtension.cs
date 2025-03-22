using System.Collections.Generic;
using Verse;

namespace SCGF
{
    /// <summary>
    /// The mod extension that developers can put on <see cref="ThingDef"/>s to make them produce custom gases.
    /// </summary>
    public class CustomGasExtension : DefModExtension
    {
        public readonly GasDef gasDef;

        public readonly bool overrideExplosionColors = true;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (gasDef == null)
            {
                yield return $"{nameof(gasDef)} cannot be null";
            }
        }
    }
}