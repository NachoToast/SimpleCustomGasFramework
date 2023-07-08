using RimWorld;
using System.Collections.Generic;
using Verse;

namespace SCGF
{
    /// <summary>
    /// This defines the 'immunities' part of a GasDef.
    /// </summary>
    public class GasDef_Immunities
    {
        public List<PawnKindDef> pawnKinds = new List<PawnKindDef>();

        public List<ThingDef> apparel = new List<ThingDef>();

        public List<HediffDef> hediffs = new List<HediffDef>();

        public List<GeneDef> genes = new List<GeneDef>();

        public List<TraitDef> traits = new List<TraitDef>();

        public bool IsImmune_PawnKind(Pawn pawn)
        {
            return pawnKinds.Contains(pawn.kindDef);
        }

        public bool IsImmune_Apparel(Pawn pawn)
        {
            if (apparel.Count > 0 && pawn.apparel != null)
            {
                foreach (Apparel item in pawn.apparel.WornApparel)
                {
                    if (apparel.Contains(item.def))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsImmune_Hediffs(Pawn pawn)
        {
            if (hediffs.Count > 0)
            {
                foreach (HediffDef hediffDef in hediffs)
                {
                    if (pawn.health.hediffSet.HasHediff(hediffDef))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsImmune_Genes(Pawn pawn)
        {
            if (genes.Count > 0 && pawn.genes != null)
            {
                foreach (GeneDef gene in genes)
                {
                    if (pawn.genes.HasGene(gene))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsImmune_Traits(Pawn pawn)
        {
            if (traits.Count > 0 && pawn.story?.traits != null)
            {
                foreach (TraitDef trait in traits)
                {
                    if (pawn.story.traits.HasTrait(trait))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsImmune_Any(Pawn pawn)
        {
            return IsImmune_PawnKind(pawn) || IsImmune_Apparel(pawn) || IsImmune_Hediffs(pawn) || IsImmune_Genes(pawn) || IsImmune_Traits(pawn);
        }
    }

}
