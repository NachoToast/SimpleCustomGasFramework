using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SCGF
{
    public delegate bool PawnImmunityCheck(Pawn pawn);

    /// <summary>
    /// This is the default gas worker that gases use to decide whether their effects should be applied to a specific pawn,
    /// and then applying those effects.
    /// </summary>
    public class GasWorker
    {
        public GasDef gasDef;

        /// <summary>
        /// List of functions that should run when checking if this gas affects a pawn.
        /// If any of these return true, the pawn is not affected.
        /// </summary>
        private readonly List<PawnImmunityCheck> pawnImmunityChecks = new List<PawnImmunityCheck>();

        public virtual void Initialize()
        {
            if (gasDef.addsHediffs == null)
            {
                // don't bother adding further checks if there's no outcome
                pawnImmunityChecks.Add((Pawn pawn) => true);
                return;
            }

            // can't apply a gas to a pawn that hasn't spawned yet, silly
            pawnImmunityChecks.Add((Pawn pawn) => !pawn.Spawned);

            if (!gasDef.targetAnimals) // if this gas doesn't target animals
            {
                pawnImmunityChecks.Add((Pawn pawn) => pawn.RaceProps.Animal); // mark animals as immune
            };

            if (!gasDef.targetHumans) // if this gas doesn't target humans
            {
                pawnImmunityChecks.Add((Pawn pawn) => pawn.RaceProps.Humanlike); // mark humans as immune
            }

            if (!gasDef.targetMechs) // if this gas doesn't target mechs
            {
                pawnImmunityChecks.Add((Pawn pawn) => pawn.RaceProps.IsMechanoid); // mark mechs as immune
            }

            if (!gasDef.targetInsects) // if this gas doesn't target insects
            {
                pawnImmunityChecks.Add((Pawn pawn) => pawn.RaceProps.Insect); // mark insects as immune
            }

            gasDef.immunities?.AddImmunityChecks(pawnImmunityChecks); // add immunities if specified
        }

        public virtual void ApplyGasEffects(Pawn pawn, byte gasDensity)
        {
            foreach (GasDef_AddsHediff gasHediffAdder in gasDef.addsHediffs)
            {
                gasHediffAdder.ApplyHediffToPawn(pawn, gasDensity);
            }
        }

        public virtual bool IsAffectedByThisGas(Pawn pawn, byte gasDensity)
        {
            foreach (PawnImmunityCheck immunityCheck in pawnImmunityChecks)
            {
                if (immunityCheck(pawn)) return false;
            }

            return true;
        }
    }
}
