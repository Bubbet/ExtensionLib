using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

public interface ICombustionProperties : IReactionProperties
{
	public NamespacedId Id { get; }

	public NamespacedId OxidizerSpecies { get; }
	public MoleQuantity OxidizerQuantity { get; }
	public NamespacedId FuelSpecies { get; }
	public MoleQuantity FuelQuantity { get; }
    
    public TemperatureKelvin AutoIgnitionTemperature { get; }

    IReadOnlyDictionary<NamespacedId, MoleQuantity> IReactionProperties.Reactants => new Dictionary<NamespacedId, MoleQuantity>{
        {OxidizerSpecies, OxidizerQuantity},
        {FuelSpecies, FuelQuantity},
    };
}