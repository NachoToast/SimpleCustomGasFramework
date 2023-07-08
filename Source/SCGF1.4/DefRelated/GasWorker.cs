using Verse;

namespace SCGF
{
    /// <summary>
    /// This is the default gas worker that gases use to decide whether their effects should be applied to a specific pawn,
    /// and then applying those effects.
    /// </summary>
    public class GasWorker
    {
        public GasDef gasDef;

        public virtual void ApplyGasEffects(Pawn pawn, byte gasDensity)
        {
            for (int i = 0; i < gasDef.addsHediffs.Count; i++)
            {
                GasDef_AddsHediff hediffAdder = gasDef.addsHediffs[i];
                hediffAdder.ApplyHediffToPawn(pawn, gasDensity);
            }
        }

        public virtual bool IsAffectedByThisGas(Pawn pawn, byte gasDensity)
        {
            if (gasDef.addsHediffs == null)
            {
                // don't bother with checks if there's no outcome
                return false;
            }

            if (!pawn.Spawned)
            {
                // can't apply a gas to a pawn that hasn't spawned yet, silly
                return false;
            }

            if (pawn.RaceProps.Animal && !gasDef.targetAnimals)
            {
                return false;
            }

            if (pawn.RaceProps.Humanlike && !gasDef.targetHumans)
            {
                return false;
            }

            if (pawn.RaceProps.IsMechanoid && !gasDef.targetMechs)
            {
                return false;
            }

            if (gasDef.immunities != null && gasDef.immunities.IsImmune_Any(pawn))
            {
                return false;
            }

            return true;
        }
    }
}
