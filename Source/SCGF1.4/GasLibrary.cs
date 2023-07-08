using System;
using System.Linq;
using Verse;

namespace SCGF
{
    /// <summary>
    /// Keeps track of all custom gases so they can be referenced throughout the framework when needed.
    /// </summary>
    public static class GasLibrary
    {
        public static readonly int firstCustomGasIndex = (int)Enum.GetValues(typeof(GasType)).Cast<GasType>().Max() + 1;

        public static GasDef[] customGassesArray;

        public static int numCustomGasses = 0;

        public static bool anyToxicCustomGasses = false;

        public static void LoadCustomGasDefs()
        {

            customGassesArray = DefDatabase<GasDef>.AllDefsListForReading.Where(gasDef => gasDef.realGasType == null).ToArray();
            numCustomGasses = customGassesArray.Length;
            anyToxicCustomGasses = false;

            for (int i = 0; i < numCustomGasses; i++)
            {
                GasDef gasDef = customGassesArray[i];

                gasDef.customIndex = i;

                if (gasDef.isToxic)
                {
                    anyToxicCustomGasses = true;
                }
            }

            if (numCustomGasses == 0)
            {
                Log.Warning("[Simple Custom Gas Framework] No custom gases found");
            }
            else if (numCustomGasses == 1)
            {
                Log.Message("[Simple Custom Gas Framework] 1 Custom gas loaded");
            }
            else
            {
                Log.Message(String.Format("[Simple Custom Gas Framework] {0} Custom gases loaded", numCustomGasses));
            }

        }

        public static GasDef GetCustomGasFromIndex(GasType customIndex)
        {
            return customGassesArray[(int)customIndex - firstCustomGasIndex];
        }
    }
}
