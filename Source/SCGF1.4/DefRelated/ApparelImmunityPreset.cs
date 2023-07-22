using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SCGF
{
    public class ApparelImmunityPreset : Def
    {
        public List<ThingDef> apparel = new List<ThingDef>();

        public ApparelImmunityPreset alsoIncludes = null;

        public HashSet<ThingDef> GetAllApparelDefs()
        {
            HashSet<ThingDef> apparelDefs = apparel.ToHashSet();

            if (alsoIncludes != null) apparelDefs.AddRange(alsoIncludes.GetAllApparelDefs());

            return apparelDefs;
        }
    }
}
