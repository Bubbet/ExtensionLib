using System.Collections.Generic;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

public readonly struct VanillaCombustionProperties : ICombustionProperties
{
	public VanillaCombustionProperties(NamespacedId id, KeyValuePair<IMaterialProperties, MoleQuantity> fuel, KeyValuePair<IMaterialProperties, MoleQuantity> oxidizer, IEnumerable<KeyValuePair<IMaterialProperties, MoleQuantity>> results)
	{
		Id = id;
		Fuel = fuel;
		Oxidizer = oxidizer;
		Results = results;
	}

	public NamespacedId Id { get; }
	public KeyValuePair<IMaterialProperties, MoleQuantity> Oxidizer { get; }
	public KeyValuePair<IMaterialProperties, MoleQuantity> Fuel { get; }
	public IEnumerable<KeyValuePair<IMaterialProperties, MoleQuantity>> Results { get; }
}