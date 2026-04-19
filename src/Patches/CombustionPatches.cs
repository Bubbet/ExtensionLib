using System;
using Assets.Scripts.Atmospherics;
using HarmonyLib;
using Interop;

namespace Com.DipoleCat.ExtensionLib.Patches
{
    [HarmonyPatch]
    public static class CombustionPatches
    {
        [HarmonyPostfix, HarmonyPatch(typeof(GasMixture), MethodType.Constructor)]
        public static void GasMixture_Constructor(ref GasMixture __instance){
            var phases = GasMixtureInterop.GetModdedPhases(ref __instance);
            using var data = Registries.Phases.OrderedData.GetEnumerator();
            uint index = 0;
            do{
                var mole = new Mole(0, new MoleQuantity(0), new MoleEnergy(0));
                MoleInterop.SetModdedPhase(ref mole, index);
                phases[index] = mole;
                index++;
            } while (data.MoveNext());
        }
    }
}