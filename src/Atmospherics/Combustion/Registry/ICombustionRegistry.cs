using System.Collections.Generic;
using Com.DipoleCat.ExtensionLib;
using Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;

namespace Com.Dipolecat.ExtensionLib.Atmospherics.Combustion.Registry
{
    public interface ICombustionRegistry: IRegistry<ICombustionProperties>
    {
        /*
        private static readonly Dictionary<(NamespacedId, NamespacedId), List<ICombustionProperties>> ByReagents = new();
        public static List<ICombustionProperties>? GetCombustions(
            NamespacedId oxidizer,
            NamespacedId fuel
        ){
            ByReagents.TryGetValue(
                (oxidizer, fuel),
                out var combustions
            );
            return combustions;
        }
        */
        
        public IEnumerable<NamespacedId>? GetComplementaryIds(NamespacedId id);

        public ICombustionProperties? GetData(NamespacedId oxidizerId, NamespacedId fuelId);
        

        /// <exception cref="KeyNotFoundException"><paramref name="id"/> has not been registered</exception>
        public ICombustionProperties this[NamespacedId oxidizerId, NamespacedId fuelId]{
            get {
                var value = GetData(oxidizerId, fuelId);
                return value ?? throw new KeyNotFoundException($"Combustion with oxidizer {oxidizerId} and fuel {fuelId} is not registered");
            }
        }
    }
}