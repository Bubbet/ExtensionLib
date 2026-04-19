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

    public IReadOnlyDictionary<NamespacedId, MoleQuantity> Results {get;}

    MoleEnergy IReactionProperties.MolarEnthalpy(TemperatureKelvin temperature, PressurekPa pressure){
        throw new System.NotImplementedException();
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
        //MoleEnergy molarEnthalpy, TODO im not sure what to be doing with this
        TemperatureKelvin autoIgnitionTemperature,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        Id = id;
        FuelSpecies = fuelSpecies;
        FuelQuantity = fuelQuantity;
        OxidizerSpecies = oxidizerSpecies;
        OxidizerQuantity = oxidizerQuantity;
        AutoIgnitionTemperature = autoIgnitionTemperature;
        Results = new ReadOnlyDictionary<NamespacedId, MoleQuantity>(results);
    }

    public VanillaCombustionProperties(
        string @namespace,
        NamespacedId oxidizerSpecies,
        MoleQuantity oxidizerQuantity,
        NamespacedId fuelSpecies,
        MoleQuantity fuelQuantity,
        TemperatureKelvin autoIgnitionTemperature,
        //MoleEnergy molarEnthalpy,
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
        autoIgnitionTemperature,
        //molarEnthalpy,
        results
    ){}

    public static VanillaCombustionProperties MakeHypergolic(
        NamespacedId id,
        NamespacedId hypergolicSpecies,
        TemperatureKelvin autoIgnitionTemperature,
        //MoleEnergy molarEnthalpy,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return new VanillaCombustionProperties(
            id,
            hypergolicSpecies,
            MoleQuantity.One,
            hypergolicSpecies,
            MoleQuantity.One,
            autoIgnitionTemperature,
            //molarEnthalpy,
            results
        );
    }

    public static VanillaCombustionProperties MakeHypergolic(
        string @namespace,
        NamespacedId hypergolicSpecies,
        TemperatureKelvin autoIgnitionTemperature,
        //MoleEnergy gasCombustionMolarEnthalpy,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return MakeHypergolic(
            new NamespacedId(@namespace, hypergolicSpecies.Name + "_" + hypergolicSpecies.Name),
            hypergolicSpecies,
            autoIgnitionTemperature,
            //gasCombustionMolarEnthalpy,
            results
        );
    }

    public static IEnumerable<ICombustionProperties> MakeHypergolicForLiquidAndGas(
        string @namespace,
        NamespacedId hypergolicGasId,
        NamespacedId hypergolicLiquidId,
        //MoleEnergy hypergolicLatentHeat,
        //MoleEnergy gasCombustionMolarEnthalpy,
        TemperatureKelvin autoIgnitionTemperature,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return MakeForLiquidAndGas(
            @namespace,
            hypergolicGasId,
            hypergolicLiquidId,
            //hypergolicLatentHeat,
            MoleQuantity.One,
            hypergolicGasId,
            hypergolicLiquidId,
            //hypergolicLatentHeat,
            MoleQuantity.One,
            autoIgnitionTemperature,
            //gasCombustionMolarEnthalpy,
            results
        );
    }

    public static IEnumerable<ICombustionProperties> MakeForLiquidAndGas(
        string @namespace,
        NamespacedId oxidizerGasId,
        NamespacedId oxidizerLiquidId,
        MoleQuantity oxidizerQuantity,
        NamespacedId fuelGasId,
        NamespacedId fuelLiquidId,
        MoleQuantity fuelQuantity,
        TemperatureKelvin autoIgnitionTemperature,
        //MoleEnergy gasCombustionMolarEnthalpy,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        yield return new VanillaCombustionProperties(
            new NamespacedId(@namespace, oxidizerGasId.Name + "_" + fuelGasId.Name),
            oxidizerGasId,
            oxidizerQuantity,
            fuelGasId,
            fuelQuantity,
            autoIgnitionTemperature,
            //gasCombustionMolarEnthalpy,
            results);
        
        yield return new VanillaCombustionProperties(
            new NamespacedId(@namespace, oxidizerLiquidId.Name + "_" + fuelGasId.Name),
            oxidizerLiquidId,
            oxidizerQuantity,
            fuelGasId,
            fuelQuantity,
            autoIgnitionTemperature,
            //gasCombustionMolarEnthalpy - oxidizerLatentHeat,
            results);
        
        yield return new VanillaCombustionProperties(
            new NamespacedId(@namespace, oxidizerGasId.Name + "_" + fuelLiquidId.Name),
            oxidizerGasId,
            oxidizerQuantity,
            fuelLiquidId,
            fuelQuantity,
            autoIgnitionTemperature,
            //gasCombustionMolarEnthalpy - fuelLatentHeat,
            results);
        
        yield return new VanillaCombustionProperties(
            new NamespacedId(@namespace, oxidizerLiquidId.Name + "_" + fuelLiquidId.Name),
            oxidizerLiquidId,
            oxidizerQuantity,
            fuelLiquidId,
            fuelQuantity,
            autoIgnitionTemperature,
            //gasCombustionMolarEnthalpy - oxidizerLatentHeat - fuelLatentHeat,
            results);
    }
}