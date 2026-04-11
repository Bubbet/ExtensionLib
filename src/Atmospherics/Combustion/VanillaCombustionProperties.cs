using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

public readonly struct VanillaCombustionProperties : ICombustionProperties
{
	private static readonly Dictionary<(NamespacedId, NamespacedId), List<ICombustionProperties>> ByReagents = new();
	public static List<ICombustionProperties>? GetCombustions(NamespacedId oxidizer, NamespacedId fuel)
	{
		ByReagents.TryGetValue((oxidizer, fuel), out var combustions);
		return combustions;
	}
	
	public NamespacedId Id { get; }
	public NamespacedId Oxidizer { get; }
	public MoleQuantity OxidizerQuantity { get; }
	public NamespacedId Fuel { get; }
	public MoleQuantity FuelQuantity { get; }

	public IReadOnlyDictionary<NamespacedId, MoleQuantity> Results { get; }
	public MoleEnergy MolarEnthalpy { get; }

	public float ReactionRate(GasMixture mixture)
	{
		throw new System.NotImplementedException();
	}

	/// <summary>
	/// </summary>
	/// <param name="id"></param>
	/// <param name="oxidizer">Expected to include the phase in the id</param>
	/// <param name="oxidizerQuantity"></param>
	/// <param name="fuel">Expected to include the phase in the id</param>
	/// <param name="fuelQuantity"></param>
	/// <param name="results">Expected to include the phase in the id</param>
	public VanillaCombustionProperties(NamespacedId id, NamespacedId oxidizer, MoleQuantity oxidizerQuantity,
		NamespacedId fuel, MoleQuantity fuelQuantity, IDictionary<NamespacedId, MoleQuantity> results)
	{
		Id = id;
		Fuel = fuel;
		FuelQuantity = fuelQuantity;
		Oxidizer = oxidizer;
		OxidizerQuantity = oxidizerQuantity;
		if (!ByReagents.TryAdd((oxidizer, fuel), [this]))
			ByReagents[(oxidizer, fuel)].Add(this);
		Results = new ReadOnlyDictionary<NamespacedId, MoleQuantity>(results);
	}

	/// <summary>
	/// Auto-Generates the ID using the namespace passed.
	/// </summary>
	/// <param name="@namespace"></param>
	/// <param name="oxidizer">Expected to include the phase in the id</param>
	/// <param name="oxidizerQuantity"></param>
	/// <param name="fuel">Expected to include the phase in the id</param>
	/// <param name="fuelQuantity"></param>
	/// <param name="results">Expected to include the phase in the id</param>
	public VanillaCombustionProperties(string @namespace, NamespacedId oxidizer, MoleQuantity oxidizerQuantity,
		NamespacedId fuel, MoleQuantity fuelQuantity, IDictionary<NamespacedId, MoleQuantity> results) : this(
		new NamespacedId(@namespace, oxidizer.Name + "_" + fuel.Name), oxidizer, oxidizerQuantity, fuel, fuelQuantity,
		results)
	{
	}
	
	public static VanillaCombustionProperties Hypergolic(NamespacedId id, NamespacedId hypergolic,
		IDictionary<NamespacedId, MoleQuantity> results)
	{
		return new VanillaCombustionProperties(id, hypergolic, MoleQuantity.One, hypergolic, MoleQuantity.One, results);
	}

	public static VanillaCombustionProperties Hypergolic(string @namespace, NamespacedId hypergolic,
		IDictionary<NamespacedId, MoleQuantity> results)
	{
		return Hypergolic(new NamespacedId(@namespace, hypergolic.Name + "_" + hypergolic.Name), hypergolic, results);
	}

	public static IEnumerable<ICombustionProperties> AllStatesHypergolic(string @namespace, NamespacedId hypergolic,
		IDictionary<NamespacedId, MoleQuantity> results)
	{
		return AllStates(@namespace, hypergolic, MoleQuantity.One, hypergolic, MoleQuantity.One, results);
	}

	/// <summary>
	/// Generates each variant (gas-gas, gas-liquid, liquid-liquid, liquid-gas)
	/// </summary>
	/// <param name="namespace"></param>
	/// <param name="oxidizer">Expected to NOT include the phase</param>
	/// <param name="oxidizerQuantity"></param>
	/// <param name="fuel">Expected to NOT include the phase</param>
	/// <param name="fuelQuantity"></param>
	/// <param name="results">Expected to include the phase</param>
	/// <returns></returns>
	public static IEnumerable<ICombustionProperties> AllStates(string @namespace, NamespacedId oxidizer,
		MoleQuantity oxidizerQuantity,
		NamespacedId fuel, MoleQuantity fuelQuantity, IDictionary<NamespacedId, MoleQuantity> results)
	{
		var properties = new[]
		{
			new VanillaCombustionProperties(new NamespacedId(@namespace, oxidizer.Name + "/gas_" + fuel.Name + "/gas"),
				oxidizer / "gas", oxidizerQuantity, fuel / "gas", fuelQuantity, results),
			new VanillaCombustionProperties(
				new NamespacedId(@namespace, oxidizer.Name + "/gas_" + fuel.Name + "/liquid"), oxidizer / "gas",
				oxidizerQuantity, fuel / "liquid", fuelQuantity, results),
			new VanillaCombustionProperties(
				new NamespacedId(@namespace, oxidizer.Name + "/liquid_" + fuel.Name + "/liquid"), oxidizer / "liquid",
				oxidizerQuantity, fuel / "liquid", fuelQuantity, results),
			new VanillaCombustionProperties(
				new NamespacedId(@namespace, oxidizer.Name + "/liquid_" + fuel.Name + "/gas"), oxidizer / "liquid",
				oxidizerQuantity, fuel / "gas", fuelQuantity, results),
		};
		return properties.Cast<ICombustionProperties>();
	}
}