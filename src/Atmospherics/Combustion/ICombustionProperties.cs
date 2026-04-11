using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

public interface ICombustionProperties : IReactionProperties
{
	public NamespacedId Id { get; }

	public NamespacedId Oxidizer { get; }
	public MoleQuantity OxidizerQuantity { get; }
	public NamespacedId Fuel { get; }
	public MoleQuantity FuelQuantity { get; }

	IReadOnlyDictionary<NamespacedId, MoleQuantity> IReactionProperties.Reactants =>
		new ReadOnlyDictionary<NamespacedId, MoleQuantity>(new Dictionary<NamespacedId, MoleQuantity>
		{
			{ Oxidizer, OxidizerQuantity },
			{ Fuel, FuelQuantity },
		});
}