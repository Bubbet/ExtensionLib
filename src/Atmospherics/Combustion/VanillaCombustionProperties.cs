using System.Collections.Generic;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

public readonly struct VanillaCombustionProperties : ICombustionProperties
{
	public static VanillaCombustionProperties Hypergolic(string @namespace, NamespacedId hypergolic)
	{
		return new VanillaCombustionProperties(@namespace, hypergolic, MoleQuantity.One, hypergolic, MoleQuantity.One);
	}
	
	public VanillaCombustionProperties(string @namespace, NamespacedId oxidizer, MoleQuantity oxidizerQuantity,
		NamespacedId fuel, MoleQuantity fuelQuantity, IDictionary<NamespacedId, MoleQuantity> results)
	{
		var id = new NamespacedId(@namespace, oxidizer.Name + "_" + fuel.Name);
		Id = id;
		Fuel = fuel;
		FuelQuantity = fuelQuantity;
		Oxidizer = oxidizer;
		OxidizerQuantity = oxidizerQuantity;
		Results = results;
	}

	public VanillaCombustionProperties(string @namespace, NamespacedId oxidizer, MoleQuantity oxidizerQuantity,
		NamespacedId fuel, MoleQuantity fuelQuantity)
		: this(@namespace, oxidizer, oxidizerQuantity, fuel, fuelQuantity, new Dictionary<NamespacedId, MoleQuantity>())
	{
		Results = new Dictionary<NamespacedId, MoleQuantity>();
	}

	public VanillaCombustionProperties WithResult(NamespacedId result, MoleQuantity resultQuantity)
	{
		Results.Add(result, resultQuantity);
		return this;
	}

	public NamespacedId Id { get; }
	public NamespacedId Oxidizer { get; }
	public MoleQuantity OxidizerQuantity { get; }
	public NamespacedId Fuel { get; }
	public MoleQuantity FuelQuantity { get; }

	public IDictionary<NamespacedId, MoleQuantity> Results { get; }
	public MoleEnergy MolarEnthalpy { get; }

	public float ReactionRate(GasMixture mixture)
	{
		throw new System.NotImplementedException();
	}
}