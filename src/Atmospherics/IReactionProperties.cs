using System.Collections.Generic;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics
{
    public interface IReactionProperties
    {
        public IReadOnlyDictionary<NamespacedId, MoleQuantity> Reactants { get; }
        public IReadOnlyDictionary<NamespacedId, MoleQuantity> Results { get; }
        public MoleEnergy MolarEnthalpy(TemperatureKelvin temperature, PressurekPa pressure);
        public float ReactionRate(GasMixture mixture);
    }
}