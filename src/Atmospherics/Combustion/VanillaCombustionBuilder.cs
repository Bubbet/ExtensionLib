using System;
using System.Collections.Generic;
using Assets.Scripts.Atmospherics;
using Com.DipoleCat.ExtensionLib;
using Com.DipoleCat.ExtensionLib.Atmospherics;
using Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

namespace Com.Dipolecat.ExtensionLib.Atmospherics.Combustion;

public class VanillaCombustionBuilder
{
	private KeyValuePair<NamespacedId, MoleQuantity>? _fuel;
	private KeyValuePair<NamespacedId, MoleQuantity>? _oxidizer;
	private Dictionary<NamespacedId, MoleQuantity> _results = new();
	private readonly string _namespace;

	public VanillaCombustionBuilder(string @namespace)
	{
		_namespace = @namespace;
	}

	public VanillaCombustionBuilder Fuel(
		NamespacedId material,
		MoleQuantity amountNeeded
	)
	{
		_fuel = new KeyValuePair<NamespacedId, MoleQuantity>(material, amountNeeded);
		return this;
	}

	public VanillaCombustionBuilder Oxidizer(
		NamespacedId material,
		MoleQuantity amountNeeded
	)
	{
		_oxidizer = new KeyValuePair<NamespacedId, MoleQuantity>(material, amountNeeded);
		return this;
	}

	public VanillaCombustionBuilder Hypergolic(
		NamespacedId material,
		MoleQuantity amountNeeded
	)
	{
		Fuel(material, amountNeeded);
		Oxidizer(material, amountNeeded);
		return this;
	}

	public VanillaCombustionBuilder Result(NamespacedId material, MoleQuantity amountAdded)
	{
		_results[material] = amountAdded;
		return this;
	}

	public VanillaCombustionProperties Build()
	{
		if (!_fuel.HasValue) throw new ArgumentException("No fuel present in combustion");
		if (!_oxidizer.HasValue) throw new ArgumentException("No oxidizer present in combustion");
		var id = new NamespacedId(_namespace, _fuel.Value.Key.Name + "_" + _oxidizer.Value.Key.Name);

		var materials = Registries.GetRegistry<IMaterialProperties>(Registries.MaterialRegistryId)!;
		var fuel = materials[_fuel.Value.Key];
		var oxidizer = materials[_fuel.Value.Key];

		var results = new Dictionary<IMaterialProperties, MoleQuantity>();
		foreach (var result in _results)
		{
			results[materials[result.Key]] = result.Value;
		}

		return new VanillaCombustionProperties(id,
			new KeyValuePair<IMaterialProperties, MoleQuantity>(fuel, _fuel.Value.Value),
			new KeyValuePair<IMaterialProperties, MoleQuantity>(oxidizer, _oxidizer.Value.Value), results);
	}
}