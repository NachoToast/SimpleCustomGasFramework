using System.Collections.Generic;
using Verse;

namespace SCGF
{
    public class HediffGiver_RandomWithSeverity : HediffGiver
    {
        private readonly float mtbDays = 0f;

        private readonly FloatRange severityRange;

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            float chanceFactor = ChanceFactor(pawn);

            if (chanceFactor == 0f)
            {
                return;
            }

            List<Hediff> addedHediffs = new List<Hediff>();

            if (Rand.MTBEventOccurs(mtbDays / chanceFactor, 60000f, 60f) && TryApply(pawn, addedHediffs))
            {
                foreach (Hediff hediff in addedHediffs)
                {
                    hediff.Severity = severityRange.RandomInRange;
                }

                SendLetter(pawn, cause);
            }
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (mtbDays < 0)
            {
                yield return $"{nameof(mtbDays)} cannot be less than 0";
            }

            if (severityRange == null)
            {
                yield return $"{nameof(severityRange)} cannot be null";
            }
            else
            {
                if (severityRange.max <= 0)
                {
                    yield return $"{nameof(severityRange)} maximum cannot be less than or equal to 0 (got {severityRange.max})";
                }
                if (severityRange.min <= 0)
                {
                    yield return $"{nameof(severityRange)} minimum cannot be less than or equal to 0 (got {severityRange.min})";
                }
            }
        }
    }
}