using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SCGF
{
    /// <summary>
    /// A predefined list of apparels that grant immunity to a gas.
    /// </summary>
    public class ApparelImmunityPreset : Def
    {
        public List<ThingDef> apparel = new List<ThingDef>();

        public ApparelImmunityPreset alsoIncludes = null;

        /// <summary>
        /// Recursively creates a hashset out of all apparel defs.
        /// </summary>
        /// <returns></returns>
        public HashSet<ThingDef> GetAllApparelDefs()
        {
            HashSet<ThingDef> apparelDefs = apparel.ToHashSet();

            if (alsoIncludes != null) apparelDefs.AddRange(alsoIncludes.GetAllApparelDefs());

            return apparelDefs;
        }
    }
}
