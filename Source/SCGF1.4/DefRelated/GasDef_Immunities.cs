using RimWorld;
using System.Collections.Generic;
using System.Linq;
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

        public List<ApparelImmunityPreset> apparelPresets = new List<ApparelImmunityPreset>();

        public List<List<ThingDef>> apparelSets = new List<List<ThingDef>>();

        public List<HediffDef> hediffs = new List<HediffDef>();

        public List<GeneDef> genes = new List<GeneDef>();

        public List<TraitDef> traits = new List<TraitDef>();

        public List<Gender> genders = new List<Gender>();

        public List<LifeStageDef> lifestages = new List<LifeStageDef>();

        [Unsaved(false)]
        private HashSet<PawnKindDef> _pawnKinds;

        [Unsaved(false)]
        private HashSet<ThingDef> _apparel;

        [Unsaved(false)]
        private HashSet<HashSet<ThingDef>> _apparelPresets;

        [Unsaved(false)]
        private HashSet<HashSet<ThingDef>> _apparelSets;

        [Unsaved(false)]
        private HashSet<HediffDef> _hediffs;

        [Unsaved(false)]
        private HashSet<GeneDef> _genes;

        [Unsaved(false)]
        private HashSet<TraitDef> _traits;

        [Unsaved(false)]
        private HashSet<Gender> _genders;

        [Unsaved(false)]
        private HashSet<LifeStageDef> _lifestages;

        private void TransformFields()
        {
            _pawnKinds = pawnKinds.ToHashSet();
            _apparel = apparel.ToHashSet();

            _apparelPresets = new HashSet<HashSet<ThingDef>>(apparelPresets.Count);
            foreach (ApparelImmunityPreset apparelSetPrefix in apparelPresets)
            {
                _apparelPresets.Add(apparelSetPrefix.GetAllApparelDefs());
            }

            _apparelSets = new HashSet<HashSet<ThingDef>>(apparelSets.Count);
            foreach (List<ThingDef> apparelSet in apparelSets)
            {
                _apparelSets.Add(apparelSet.ToHashSet());
            }

            _hediffs = hediffs.ToHashSet();
            _genes = genes.ToHashSet();
            _traits = traits.ToHashSet();
            _genders = genders.ToHashSet();
            _lifestages = lifestages.ToHashSet();
        }

        public void AddImmunityChecks(List<PawnImmunityCheck> pawnImmunityChecks)
        {
            TransformFields();

            // pawn kinds
            if (_pawnKinds.Count > 0) pawnImmunityChecks.Add(IsImmune_PawnKind);

            // apparel
            if (_apparel.Count > 0) pawnImmunityChecks.Add(IsImmune_Apparel);

            // apparel sets and presets
            if (_apparelSets.Count > 0 || _apparelPresets.Count > 0) pawnImmunityChecks.Add(IsImmune_ApparelSetsOrPresets);

            // hediffs
            if (_hediffs.Count > 0) pawnImmunityChecks.Add(IsImmune_Hediffs);

            // genes
            if (_genes.Count > 0) pawnImmunityChecks.Add(IsImmune_Genes);

            // traits
            if (_traits.Count > 0) pawnImmunityChecks.Add(IsImmune_Traits);

            // genders
            if (_genders.Count > 0) pawnImmunityChecks.Add(IsImmune_Genders);

            // life stages
            if (_lifestages.Count > 0) pawnImmunityChecks.Add(IsImmune_Lifestages);
        }

        private bool IsImmune_PawnKind(Pawn pawn)
        {
            return _pawnKinds.Contains(pawn.kindDef);
        }

        private bool IsImmune_Apparel(Pawn pawn)
        {
            if (pawn.apparel == null) return false;

            foreach (Apparel item in pawn.apparel.WornApparel)
            {
                if (_apparel.Contains(item.def))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsImmune_ApparelSetsOrPresets(Pawn pawn)
        {
            if (pawn.apparel == null) return false;

            HashSet<ThingDef> wornApparelDefs = new HashSet<ThingDef>(pawn.apparel.WornApparelCount);
            foreach (Apparel apparel in pawn.apparel.WornApparel)
            {
                wornApparelDefs.Add(apparel.def);
            }

            // sets
            foreach (HashSet<ThingDef> apparelSet in _apparelSets)
            {
                if (IsWearingEveryItemInSet(apparelSet)) return true;
            }

            // presets
            foreach (HashSet<ThingDef> apparelPreset in _apparelPresets)
            {
                if (!IsWearingAnyItemInSet(apparelPreset)) return false;
            }

            return true;

            bool IsWearingEveryItemInSet(HashSet<ThingDef> apparelSet)
            {
                // cannot possibly be wearing every item in set if set has more items than pawn is wearing
                if (apparelSet.Count > wornApparelDefs.Count) return false;

                foreach (ThingDef immuneApparel in apparelSet)
                {
                    if (!wornApparelDefs.Contains(immuneApparel)) return false;
                }

                return true;
            }

            bool IsWearingAnyItemInSet(HashSet<ThingDef> apparelSet)
            {
                foreach(ThingDef apparelDef in wornApparelDefs)
                {
                    if (apparelSet.Contains(apparelDef)) return true;
                }

                return false;
            }
        }

        private bool IsImmune_Hediffs(Pawn pawn)
        {
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (_hediffs.Contains(hediff.def)) return true;
            }

            return false;
        }

        private bool IsImmune_Genes(Pawn pawn)
        {
            if (pawn.genes == null) return false;

            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (_genes.Contains(gene.def)) return true;
            }

            return false;
        }

        private bool IsImmune_Traits(Pawn pawn)
        {
            if (pawn.story?.traits == null) return false;

            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                if (_traits.Contains(trait.def)) return true;
            }

            return false;
        }

        private bool IsImmune_Genders(Pawn pawn)
        {
            return _genders.Contains(pawn.gender);
        }

        private bool IsImmune_Lifestages(Pawn pawn)
        {
            if (pawn.ageTracker?.CurLifeStage == null) return false;

            return _lifestages.Contains(pawn.ageTracker.CurLifeStage);
        }
    }

}
