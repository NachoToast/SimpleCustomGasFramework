using System.Collections.Generic;
using Verse;

namespace SCGF
{
    public static class Extensions
    {
        /// <summary>
        /// Returns <see langword="true"/> if <b>any</b> of the <paramref name="filters"/> should apply to the given <paramref name="pawn"/>.
        /// </summary>
        /// <remarks>
        /// See Also: <seealso cref="AllFiltersPass"/>
        /// </remarks>
        public static bool AnyFiltersPass(this IEnumerable<GasFilter> filters, Pawn pawn, byte gasDensity)
        {
            foreach (GasFilter filter in filters)
            {
                if (filter.ShouldApplyTo(pawn, gasDensity))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <b>all</b> of the <paramref name="filters"/> should apply to the given <paramref name="pawn"/>.
        /// </summary>
        /// <remarks>
        /// See Also: <seealso cref="AnyFiltersPass"/>
        /// </remarks>
        public static bool AllFiltersPass(this IEnumerable<GasFilter> filters, Pawn pawn, byte gasDensity)
        {
            foreach (GasFilter filter in filters)
            {
                if (!filter.ShouldApplyTo(pawn, gasDensity))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Like <see cref="HediffSet.GetFirstHediffOfDef"/> but also filters by <paramref name="part"/>.
        /// </summary>
        public static Hediff GetFirstHediffOfDefOnPart(this HediffSet hediffSet, HediffDef def, BodyPartRecord part)
        {
            foreach (Hediff hediff in hediffSet.hediffs)
            {
                if (hediff.def == def && hediff.Part == part)
                {
                    return hediff;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <b>every</b> element in the <paramref name="target"/> list is also in the <paramref name="source"/> set.
        /// </summary>
        public static bool ContainsEveryItemIn<T>(this HashSet<T> source, List<T> target)
        {
            if (target.Count > source.Count)
            {
                return false;
            }

            foreach (T item in target)
            {
                if (!source.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the given <paramref name="def"/> has the  <see cref="DefModExtension"/> <typeparamref name="T"/>
        /// </summary>
        public static bool HasModExtension<T>(this Def def, out T extension) where T : DefModExtension
        {
            extension = def.GetModExtension<T>();

            return (extension != null);
        }
    }
}