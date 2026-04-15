using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Networking;
using Com.DipoleCat.ExtensionLib;
using Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;
using Com.DipoleCat.ExtensionLib.Networking;

namespace Com.Dipolecat.ExtensionLib.Atmospherics.Combustion.Registry
{
    internal sealed class FrozenCombustionRegistry : ICombustionRegistry
    {
        private readonly Dictionary<NamespacedId, uint> id_map = new();
        private readonly List<NamespacedId> id_list = new();
        private readonly List<ICombustionProperties> data_list = new();
        private readonly INetworkCodec<ICombustionProperties> codec;
        private readonly HashSet<NamespacedId> oxidizers = new();
        private readonly HashSet<NamespacedId> fuels = new();
        private readonly Dictionary<(NamespacedId, NamespacedId), uint> byReagents = new();
        private readonly Dictionary<NamespacedId, HashSet<NamespacedId>> compliments = new();
        public int Count => id_list.Count;

        public IEnumerable<ICombustionProperties> OrderedData => data_list.AsReadOnly();

        public IEnumerable<NamespacedId> OrderedIds => id_list.AsReadOnly();
        internal FrozenCombustionRegistry(
            Dictionary<NamespacedId, uint> id_map,
            List<NamespacedId> id_list,
            List<ICombustionProperties> data_list,
            INetworkCodec<ICombustionProperties> codec,
            HashSet<NamespacedId> oxidizers,
            HashSet<NamespacedId> fuels,
            Dictionary<(NamespacedId, NamespacedId), uint> byReagents,
            Dictionary<NamespacedId, HashSet<NamespacedId>> compliments
        ){
            //no reference types in these, so shallow copy is sufficient
            this.id_map = new(id_map);
            this.id_list = new(id_list);
            this.data_list = new(data_list);
            this.codec = codec;
            this.oxidizers = new(oxidizers);
            this.fuels = new(fuels);
            this.byReagents = new (byReagents);
            this.compliments = new(compliments);
        }


         public ICombustionProperties? GetData(uint index){
             return index >= Count ? null : data_list[(int)index];
         }

        public uint? GetIndex(NamespacedId id)
        {
            if(!id_map.TryGetValue(id, out uint index)) return null;
            return index;
        }

        public NamespacedId? GetId(uint index){
            return index >= Count ? throw new KeyNotFoundException($"index {index} is not registered") : id_list[(int)index];
        }

        public bool IsPresent(uint index)
        {
            return index < Count;
        }

        public void Serialize(RocketBinaryWriter writer)
        {
            Serialization.WriteArray(
                new List<NetworkEntry<ICombustionProperties>>(
                    from index in Enumerable.Range(0,Count)
                    let id = GetId((uint)index)
                    let data = GetData((uint)index)
                    orderby index
                    select new NetworkEntry<ICombustionProperties>(id.Value,data)
                ),
                new NetworkEntryCodec<ICombustionProperties>(codec),
                writer
            );
        }
        public IEnumerable<NamespacedId>? GetComplementaryIds(NamespacedId id){
            return !compliments.TryGetValue(id, out var list) ? null : list.AsEnumerable();
        }
        public ICombustionProperties? GetData(NamespacedId oxidizerId, NamespacedId fuelId){
            var key = (oxidizerId, fuelId);
            return !byReagents.TryGetValue(key, out uint value) ? null : data_list[(int)value];
        }
        
        public IEnumerable<NamespacedId> GetOxidizers()
            => oxidizers.AsEnumerable();
        public IEnumerable<NamespacedId> GetFuels()
            => fuels.AsEnumerable();
    }
}