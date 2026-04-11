using System.Collections.Generic;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics;

public interface IReactionProperties
{
	public IDictionary<NamespacedId, MoleQuantity> Reactants { get; }
	public IDictionary<NamespacedId, MoleQuantity> Results { get; }
	public MoleEnergy MolarEnthalpy { get; }
	public float ReactionRate(GasMixture mixture);
}