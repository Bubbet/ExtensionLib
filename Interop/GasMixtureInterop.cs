using System;
using System.Collections.Generic;
using Assets.Scripts.Atmospherics;

namespace Interop
{
    public static class GasMixtureInterop
    {
        public static Dictionary<uint, Mole> GetModdedPhases(ref GasMixture mixture)
            => mixture._extensionlib_phases;
    }
}