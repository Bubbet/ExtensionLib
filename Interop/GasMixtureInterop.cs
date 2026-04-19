using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts.Atmospherics;


[assembly:InternalsVisibleTo("ExtensionLib")]

namespace Interop
{
    internal static class GasMixtureInterop
    {
        public static Dictionary<uint, Mole> GetModdedPhases(ref GasMixture mixture)
            => mixture._extensionlib_phases;
    }
}