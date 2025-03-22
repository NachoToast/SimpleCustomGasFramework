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

            if (explosion.projectile?.HasModExtension(out CustomGasExtension projExtension) == true)
            {
                // Mortar shells, thrown grenades, shells from launchers, etc...
                ext = projExtension;
            }
            else if (explosion.weapon?.HasModExtension(out CustomGasExtension wepExtension) == true)
            {
                // Mortars, item form of grenades/launchers, etc...
                ext = wepExtension;
            }
            else if (explosion.instigator?.def?.HasModExtension(out CustomGasExtension insExtension) == true)
            {
                // Mortar shells (in item form), IED traps, etc...
                // This could also be a pawn that fired a launcher or mortar, but that is covered by the projectile check.
                ext = insExtension;
            }
            else
            {
                ext = null;
            }

            lastSeen = new Tuple<Explosion, CustomGasExtension>(explosion, ext);
            return ext != null;
        }
    }
}