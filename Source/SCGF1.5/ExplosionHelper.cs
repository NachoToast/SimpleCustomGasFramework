using PerformanceFish;
using System;
using Verse;

namespace SCGF
{
    public static class ExplosionHelper
    {
        private static Tuple<Explosion, CustomGasExtension> lastSeen = new Tuple<Explosion, CustomGasExtension>(null, null);

        public static void TryAddGasFor(Explosion explosion, IntVec3 cell)
        {
            if (HasCustomGas(explosion, out CustomGasExtension ext))
            {
                explosion.Map.gasGrid.AddGas(cell, ext.gasDef, 255);
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the given <paramref name="explosion"/> should add a custom gas.
        /// </summary>
        private static bool HasCustomGas(Explosion explosion, out CustomGasExtension ext)
        {
            if (explosion == lastSeen.Item1)
            {
                ext = lastSeen.Item2;
                return ext != null;
            }

            ext = explosion.damType?.GetModExtension<CustomGasExtension>();

            lastSeen = new Tuple<Explosion, CustomGasExtension>(explosion, ext);
            return ext != null;
        }
    }
}