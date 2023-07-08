using RimWorld;

namespace SCGF
{
    [DefOf]
    public class GasDefOf
    {
        public static GasDef BlindSmoke;

        public static GasDef ToxGas;

        public static GasDef RotStink;

        static GasDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GasDefOf));
        }
    }
}
