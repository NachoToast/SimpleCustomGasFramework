using SCGF.ActionHelpers;
using System.Collections.Generic;
using Verse;

namespace SCGF.Actions
{
    public class TakeDamage : GasAction
    {
        private readonly DamageDef damageDef = null;

        private readonly float amount = 5f;

        private readonly float armorPenetration = 0f;

        private readonly int tickInterval = GenTicks.TickRareInterval;

        private readonly List<BodyPartDef> partsToAffect = new List<BodyPartDef>();

        [Unsaved]
        private BodyPartChooser partChooser;

        public override void DoEffects(Pawn pawn, byte gasDensity)
        {
            if (!pawn.IsHashIntervalTick(tickInterval))
            {
                return;
            }

            BodyPartRecord hitPart = partChooser.Choose(pawn);

            pawn.TakeDamage(new DamageInfo(
                def: damageDef,
                amount: amount,
                armorPenetration: armorPenetration,
                hitPart: hitPart));
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            partChooser = BodyPartChooser.Create(partsToAffect);
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (damageDef == null)
            {
                yield return $"{nameof(damageDef)} cannot be null";
            }

            if (amount <= 0f)
            {
                yield return $"{nameof(amount)} cannot be less than or equal to 0 (got {amount})";
            }

            if (armorPenetration < 0f)
            {
                yield return $"{nameof(armorPenetration)} cannot be less than 0 (got {armorPenetration})";
            }

            if (tickInterval < 0)
            {
                yield return $"{nameof(tickInterval)} cannot be less than 0 (got {tickInterval})";
            }
        }
    }
}