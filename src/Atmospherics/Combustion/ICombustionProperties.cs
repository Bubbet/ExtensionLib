using System.Collections.Generic;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

public interface ICombustionProperties
{
	public NamespacedId Id { get; }

	public KeyValuePair<IMaterialProperties, MoleQuantity> Oxidizer { get; }
	public KeyValuePair<IMaterialProperties, MoleQuantity> Fuel { get; }
	public IEnumerable<KeyValuePair<IMaterialProperties, MoleQuantity>> Results { get; }
}
