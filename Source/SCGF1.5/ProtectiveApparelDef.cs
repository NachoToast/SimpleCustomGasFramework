using System.Collections.Generic;
using Verse;

namespace SCGF
{
    /// <summary>
    /// List of apparel <see cref="ThingDef"/>s that signify a certain level of immunity to gas.
    /// </summary>
    public class ProtectiveApparelDef : Def
    {
        private readonly List<ThingDef> apparel = new List<ThingDef>();

        [Unsaved]
        public HashSet<ThingDef> apparelSet = new HashSet<ThingDef>();

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            apparelSet = new HashSet<ThingDef>(apparel);
        }
    }
}