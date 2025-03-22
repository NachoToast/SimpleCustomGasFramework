using RimWorld;
using SCGF.ActionHelpers;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SCGF.Actions
{
    public class ApplyHediff : GasAction
    {
        private readonly HediffDef hediffDef = null;

        private readonly float severityPerTick = 0.05f;

        private readonly float densityFactor = 1f;

        private readonly StatDef impactedByStat = null;

        private readonly float statImpactFactor = 1f;

        private readonly float finalStageFactor = 1f;

        private readonly bool ignoreInfecterCompIKnowWhatImDoing = false;

        private readonly List<BodyPartDef> partsToAffect = new List<BodyPartDef>();

        [Unsaved]
        private readonly List<ISeverityModifier> modifiers = new List<ISeverityModifier>();

        [Unsaved]
        private BodyPartChooser partChooser;

        public override void DoEffects(Pawn pawn, byte gasDensity)
        {
            BodyPartRecord targetBodyPart = partChooser.Choose(pawn);

            Hediff existingHediff;

            if (targetBodyPart == null)
            {
                existingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
            }
            else
            {
                existingHediff = pawn.health.hediffSet.GetFirstHediffOfDefOnPart(hediffDef, targetBodyPart);
            }

            float severityMultiplier = 1f;

            foreach (ISeverityModifier modifier in modifiers)
            {
                modifier.DoEffects(pawn, gasDensity, ref severityMultiplier, existingHediff);
            }

            float severityOffset = severityPerTick * severityMultiplier;

            if (Mathf.Approximately(severityOffset, 0f))
            {
                // No meaningful change in severity.
                return;
            }

            if (existingHediff != null)
            {
                existingHediff.Severity += severityOffset;
                return;
            }

            if (severityOffset < 0f)
            {
                // Only add a new hediff for positive severity values.
                return;
            }

            existingHediff = HediffMaker.MakeHediff(hediffDef, pawn);
            existingHediff.Severity = severityOffset;

            pawn.health.AddHediff(existingHediff, targetBodyPart);
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            GasDensityMultiplier.TryCreate(densityFactor, modifiers);
            StatFactorMultiplier.TryCreate(impactedByStat, statImpactFactor, modifiers);
            HediffStageMultiplier.TryCreate(hediffDef, finalStageFactor, modifiers);

            partChooser = BodyPartChooser.Create(partsToAffect);
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (hediffDef == null)
            {
                yield return $"{nameof(hediffDef)} cannot be null";
            }
            else
            {
                if (hediffDef.HasComp(typeof(HediffComp_Infecter)))
                {
                    if (!ignoreInfecterCompIKnowWhatImDoing)
                    {
                        yield return $"{nameof(hediffDef)} {hediffDef.defName} has comp {nameof(HediffComp_Infecter)}, infection hediffs (chemical burns, scratches, etc...) should not be applied by gases due to them requiring a specific body part to be added to; even if you specify {nameof(partsToAffect)}, it's not guaranteed a pawn will have any of them at the time the hediff is applied. Please specify another {nameof(hediffDef)}. If you know what you're doing and can GUARANTEE a body part will always be available, you can suppress this error message by setting {nameof(ignoreInfecterCompIKnowWhatImDoing)} to true";
                    }
                }
                else if (ignoreInfecterCompIKnowWhatImDoing)
                {
                    yield return $"{nameof(ignoreInfecterCompIKnowWhatImDoing)} is true but {nameof(hediffDef)} {hediffDef.defName} does not have {nameof(HediffComp_Infecter)}";
                }

                if (finalStageFactor != 1f && hediffDef.stages.Count < 2)
                {
                    yield return $"{nameof(finalStageFactor)} is {finalStageFactor.ToStringDecimalIfSmall()} but the {nameof(hediffDef)} ({hediffDef}) {(hediffDef.stages.Count == 0 ? "has no stages" : "only has 1 stage")}";
                }
            }

            if (severityPerTick == 0f)
            {
                yield return $"{nameof(severityPerTick)} cannot be 0 (got {severityPerTick})";
            }

            if (impactedByStat != null)
            {
                if (statImpactFactor == 0f)
                {
                    yield return $"{nameof(impactedByStat)} is defined but {nameof(statImpactFactor)} is 0, this means the stat has no impact whatsoever";
                }
            }
            else if (statImpactFactor != 1f)
            {
                yield return $"{nameof(statImpactFactor)} is defined but {nameof(impactedByStat)} is not";
            }
        }
    }
}