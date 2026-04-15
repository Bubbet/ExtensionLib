using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.Scripts.Atmospherics;

namespace Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

public readonly struct VanillaCombustionProperties : ICombustionProperties
{
    public NamespacedId Id {get;}
    public NamespacedId OxidizerSpecies {get;}
    public MoleQuantity OxidizerQuantity {get;}
    public NamespacedId FuelSpecies {get;}
    public MoleQuantity FuelQuantity {get;}

    public IReadOnlyDictionary<NamespacedId, MoleQuantity> Results {get;}
    public MoleEnergy MolarEnthalpy {get;}

    public float ReactionRate(GasMixture mixture){
        throw new System.NotImplementedException();
    }

    public VanillaCombustionProperties(
        NamespacedId id,
        NamespacedId oxidizerSpecies,
        MoleQuantity oxidizerQuantity,
        NamespacedId fuelSpecies,
        MoleQuantity fuelQuantity,
        MoleEnergy molarEnthalpy,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        Id = id;
        FuelSpecies = fuelSpecies;
        FuelQuantity = fuelQuantity;
        OxidizerSpecies = oxidizerSpecies;
        OxidizerQuantity = oxidizerQuantity;
        Results = new ReadOnlyDictionary<NamespacedId, MoleQuantity>(results);
        MolarEnthalpy = molarEnthalpy;
    }

    public VanillaCombustionProperties(
        string @namespace,
        NamespacedId oxidizerSpecies,
        MoleQuantity oxidizerQuantity,
        NamespacedId fuelSpecies,
        MoleQuantity fuelQuantity,
        MoleEnergy molarEnthalpy,
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
        molarEnthalpy,
        results
    ){}

    public static VanillaCombustionProperties MakeHypergolic(
        NamespacedId id,
        NamespacedId hypergolicSpecies,
        MoleEnergy molarEnthalpy,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return new VanillaCombustionProperties(
            id,
            hypergolicSpecies,
            MoleQuantity.One,
            hypergolicSpecies,
            MoleQuantity.One,
            molarEnthalpy,
            results
        );
    }

    public static VanillaCombustionProperties MakeHypergolic(
        string @namespace,
        NamespacedId hypergolicSpecies,
        MoleEnergy molarEnthalpy,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return MakeHypergolic(
            new NamespacedId(@namespace, hypergolicSpecies.Name + "_" + hypergolicSpecies.Name),
            hypergolicSpecies,
            molarEnthalpy,
            results
        );
    }

    public static IEnumerable<ICombustionProperties> MakeHypergolicForLiquidAndGas(
        string @namespace,
        NamespacedId hypergolicMaterial,
        MoleEnergy molarEnthalpy,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        return MakeForLiquidAndGas(
            @namespace,
            hypergolicMaterial,
            MoleQuantity.One,
            hypergolicMaterial,
            MoleQuantity.One,
            molarEnthalpy,
            results
        );
    }
    
    public static IEnumerable<ICombustionProperties> MakeForLiquidAndGas(
        string @namespace,
        NamespacedId oxidizerMaterial,
        MoleQuantity oxidizerQuantity,
        NamespacedId fuelMaterial,
        MoleQuantity fuelQuantity,
        MoleEnergy molarEnthalpy,
        IDictionary<NamespacedId, MoleQuantity> results
    ){
        // TODO solve actual molarEnthalpy for liquid variants
        var properties = new[]{
            new VanillaCombustionProperties(
                new NamespacedId(@namespace,
                    oxidizerMaterial.Name + "/gas_" + fuelMaterial.Name + "/gas"),
                oxidizerMaterial / "gas",
                oxidizerQuantity,
                fuelMaterial / "gas",
                fuelQuantity,
                molarEnthalpy,
                results
            ),
            new VanillaCombustionProperties(
                new NamespacedId(@namespace,
                    oxidizerMaterial.Name + "/gas_" + fuelMaterial.Name + "/liquid"),
                oxidizerMaterial / "gas",
                oxidizerQuantity,
                fuelMaterial / "liquid",
                fuelQuantity,
                molarEnthalpy,
                results
            ),
            new VanillaCombustionProperties(
                new NamespacedId(@namespace,
                    oxidizerMaterial.Name + "/liquid_" + fuelMaterial.Name + "/liquid"),
                oxidizerMaterial / "liquid",
                oxidizerQuantity,
                fuelMaterial / "liquid",
                fuelQuantity,
                molarEnthalpy,
                results
            ),
            new VanillaCombustionProperties(
                new NamespacedId(@namespace,
                    oxidizerMaterial.Name + "/liquid_" + fuelMaterial.Name + "/gas"),
                oxidizerMaterial / "liquid",
                oxidizerQuantity,
                fuelMaterial / "gas",
                fuelQuantity,
                molarEnthalpy,
                results
            ),
        };
        return properties.Cast<ICombustionProperties>();
    }
}