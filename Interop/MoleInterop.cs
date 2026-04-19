using Assets.Scripts.Atmospherics;

namespace Interop
{
    internal static class MoleInterop
    {
        public static uint GetModdedPhase(Mole mole)
            => mole._extensionlib_phaseid;
        public static void SetModdedPhase(ref Mole mole, uint id)
            => mole._extensionlib_phaseid = id;
    }
}