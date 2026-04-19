using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

public readonly struct VanillaCombustionProperties : ICombustionProperties
{
    public NamespacedId Id {get;}
    public NamespacedId OxidizerSpecies {get;}
    public MoleQuantity OxidizerQuantity {get;}
    public NamespacedId FuelSpecies {get;}
    public MoleQuantity FuelQuantity {get;}
    public TemperatureKelvin AutoIgnitionTemperature {get;}
    public MoleEnergy CombustionEnergy {get;}

    public IReadOnlyDictionary<NamespacedId, MoleQuantity> Results {get;}

    MoleEnergy IReactionProperties.MolarEnthalpy(TemperatureKelvin temperature, PressurekPa pressure){
        return CombustionEnergy;
    }
    public float ReactionRate(GasMixture mixture){
        throw new System.NotImplementedException();
    }

    public VanillaCombustionProperties(
        NamespacedId id,
        NamespacedId oxidizerSpecies,
        MoleQuantity oxidizerQuantity,
        NamespacedId fuelSpecies,
        MoleQuantity fuelQuantity,
        MoleEnergy combustionEnergy,
        TemperatureKelvin autoIgnitionTemperature,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        Id = id;
        FuelSpecies = fuelSpecies;
        FuelQuantity = fuelQuantity;
        OxidizerSpecies = oxidizerSpecies;
        OxidizerQuantity = oxidizerQuantity;
        CombustionEnergy = combustionEnergy;
        AutoIgnitionTemperature = autoIgnitionTemperature;
        Results = new ReadOnlyDictionary<NamespacedId, MoleQuantity>(results);
    }

    public VanillaCombustionProperties(
        string @namespace,
        NamespacedId oxidizerSpecies,
        MoleQuantity oxidizerQuantity,
        NamespacedId fuelSpecies,
        MoleQuantity fuelQuantity,
        MoleEnergy combustionEnergy,
        TemperatureKelvin autoIgnitionTemperature,
        IDictionary<NamespacedId, MoleQuantity> results
    ) : this(
        new NamespacedId(
            @namespace,
            oxidizerSpecies.Name + "_" + fuelSpecies.Name
        ),
        oxidizerSpecies,
        oxidizerQuantity,
        fuelSpecies,
        fuelQuantity,
        combustionEnergy,
        autoIgnitionTemperature,
        results
    ){}

    public static VanillaCombustionProperties MakeHypergolic(
        NamespacedId id,
        NamespacedId hypergolicSpecies,
        MoleEnergy combustionEnergy,
        TemperatureKelvin autoIgnitionTemperature,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return new VanillaCombustionProperties(
            id,
            hypergolicSpecies,
            MoleQuantity.One,
            hypergolicSpecies,
            MoleQuantity.One,
            combustionEnergy,
            autoIgnitionTemperature,
            results
        );
    }

    public static VanillaCombustionProperties MakeHypergolic(
        string @namespace,
        NamespacedId hypergolicSpecies,
        MoleEnergy combustionEnergy,
        TemperatureKelvin autoIgnitionTemperature,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return MakeHypergolic(
            new NamespacedId(@namespace, hypergolicSpecies.Name + "_" + hypergolicSpecies.Name),
            hypergolicSpecies,
            combustionEnergy,
            autoIgnitionTemperature,
            results
        );
    }

    public static IEnumerable<ICombustionProperties> MakeHypergolicForLiquidAndGas(
        string @namespace,
        NamespacedId hypergolicGasId,
        NamespacedId hypergolicLiquidId,
        MoleEnergy hypergolicLatentHeat,
        MoleEnergy combustionEnergy,
        TemperatureKelvin autoIgnitionTemperature,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return MakeForLiquidAndGas(
            @namespace,
            hypergolicGasId,
            hypergolicLiquidId,
            MoleQuantity.One,
            hypergolicLatentHeat,
            hypergolicGasId,
            hypergolicLiquidId,
            MoleQuantity.One,
            hypergolicLatentHeat,
            combustionEnergy,
            autoIgnitionTemperature,
            results
        );
    }

    public static IEnumerable<ICombustionProperties> MakeForLiquidAndGas(
        string @namespace,
        NamespacedId oxidizerGasId,
        NamespacedId oxidizerLiquidId,
        MoleQuantity oxidizerQuantity,
        MoleEnergy oxidizerLatentHeat,
        NamespacedId fuelGasId,
        NamespacedId fuelLiquidId,
        MoleQuantity fuelQuantity,
        MoleEnergy fuelLatentHeat,
        MoleEnergy combustionEnergy,
        TemperatureKelvin autoIgnitionTemperature,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        yield return new VanillaCombustionProperties(
            new NamespacedId(@namespace, oxidizerGasId.Name + "_" + fuelGasId.Name),
            oxidizerGasId,
            oxidizerQuantity,
            fuelGasId,
            fuelQuantity,
            combustionEnergy,
            autoIgnitionTemperature,
            results);

        yield return new VanillaCombustionProperties(
            new NamespacedId(@namespace, oxidizerLiquidId.Name + "_" + fuelGasId.Name),
            oxidizerLiquidId,
            oxidizerQuantity,
            fuelGasId,
            fuelQuantity,
            combustionEnergy - oxidizerLatentHeat,
            autoIgnitionTemperature,
            results);

        yield return new VanillaCombustionProperties(
            new NamespacedId(@namespace, oxidizerGasId.Name + "_" + fuelLiquidId.Name),
            oxidizerGasId,
            oxidizerQuantity,
            fuelLiquidId,
            fuelQuantity,
            combustionEnergy - fuelLatentHeat,
            autoIgnitionTemperature,
            results);

        yield return new VanillaCombustionProperties(
            new NamespacedId(@namespace, oxidizerLiquidId.Name + "_" + fuelLiquidId.Name),
            oxidizerLiquidId,
            oxidizerQuantity,
            fuelLiquidId,
            fuelQuantity,
            combustionEnergy - oxidizerLatentHeat - fuelLatentHeat,
            autoIgnitionTemperature,
            results);
    }

    public static IEnumerable<ICombustionProperties> MakeForLiquidAndGas(
        string @namespace,
        VanillaMaterialProperties oxidizerMaterial,
        MoleQuantity oxidizerQuantity,
        VanillaMaterialProperties fuelMaterial,
        MoleQuantity fuelQuantity,
        MoleEnergy combustionEnergy,
        TemperatureKelvin autoIgnitionTemperature,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return MakeForLiquidAndGas(@namespace,
            oxidizerMaterial.GasPhaseId,
            oxidizerMaterial.LiquidPhaseId,
            oxidizerQuantity, 
            new MoleEnergy(oxidizerMaterial.SpecificLatentHeatOfVaporization.ToDouble()),
            fuelMaterial.GasPhaseId, 
            fuelMaterial.LiquidPhaseId, 
            fuelQuantity,
            new MoleEnergy(fuelMaterial.SpecificLatentHeatOfVaporization.ToDouble()),
            combustionEnergy,
            autoIgnitionTemperature,
            results);
    }
}