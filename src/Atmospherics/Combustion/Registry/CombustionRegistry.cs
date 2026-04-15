using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Networking;
using Com.DipoleCat.ExtensionLib;
using Com.DipoleCat.ExtensionLib.Atmospherics.Combustion;
using Com.DipoleCat.ExtensionLib.Networking;

namespace Com.Dipolecat.ExtensionLib.Atmospherics.Combustion.Registry
{
    public class CombustionRegistry : ICombustionRegistry, IMutableRegistry<ICombustionProperties>
    {
        internal readonly Dictionary<NamespacedId, uint> id_map = new();
        internal readonly List<NamespacedId> id_list = new();
        internal readonly List<ICombustionProperties> data_list = new();
        internal readonly INetworkCodec<ICombustionProperties> codec;
        internal readonly HashSet<NamespacedId> oxidizers = new();
        internal readonly HashSet<NamespacedId> fuels = new();
        internal readonly Dictionary<(NamespacedId, NamespacedId), uint> byReagents = new();
        internal readonly Dictionary<NamespacedId, HashSet<NamespacedId>> compliments = new();

        public IEnumerable<ICombustionProperties> OrderedData => data_list.AsReadOnly();
        public IEnumerable<NamespacedId> OrderedIds => id_list.AsReadOnly();
        public int Count => id_list.Count;

        public CombustionRegistry(INetworkCodec<ICombustionProperties> networkCodec){
            codec = networkCodec;
        }

        public uint? GetIndex(NamespacedId id){
            if(!id_map.TryGetValue(id, out uint index)) return null;
            return index;
        }
        public NamespacedId? GetId(uint index){
            return index >= Count ? throw new KeyNotFoundException($"index {index} is not registered") : id_list[(int)index];
        }
        public bool IsPresent(uint index)
            => index < Count;
        public bool IsPresent(NamespacedId id)
            => id_map.ContainsKey(id);
        public void Serialize(RocketBinaryWriter writer){
            Serialization.WriteArray(
                new List<NetworkEntry<ICombustionProperties>>(
                    from index in Enumerable.Range(0, Count)
                    let id = GetId((uint)index)
                    let data = GetData((uint)index)
                    orderby index
                    select new NetworkEntry<ICombustionProperties>(id.Value, data)
                ),
                new NetworkEntryCodec<ICombustionProperties>(codec),
                writer
            );
        }
        public ICombustionProperties? GetData(uint index){
            return index >= Count ? null : data_list[(int)index];
        }
        public ICombustionProperties? GetData(NamespacedId id){
            uint? integer_id = GetIndex(id);
            return !integer_id.HasValue ? null : GetData(integer_id.Value);
        }

        public void Deregister(NamespacedId id){
            throw new NotSupportedException();
            // TODO maybe change hashsets to dict counting references
        }
        public void SetData(uint index, ICombustionProperties data){
            if(index >= Count) throw new KeyNotFoundException($"index {index} is not registered");
            data_list[(int)index] = data;
        }

        internal void AddCompliment(NamespacedId key, IEnumerable<NamespacedId> values){
            if(!compliments.TryGetValue(key, out var list)){ list = []; }
            foreach (var namespacedId in values){ list.Add(namespacedId); }
        }
        public uint Register(NamespacedId id, ICombustionProperties data){
            //enforce uniqueness
            uint? existing_index = GetIndex(id);
            if(existing_index.HasValue) throw new ArgumentException($"ID {id} is already registered");

            uint index = (uint)Count;
            id_list.Add(id);
            data_list.Add(data);
            id_map.Add(id, index);

            foreach (var reactant in data.Reactants.Keys){
                var rest = data.Reactants.Keys.Where(x => x != reactant);
                AddCompliment(reactant, rest);
            }

            oxidizers.Add(data.OxidizerSpecies);
            fuels.Add(data.FuelSpecies);

            byReagents.Add((data.OxidizerSpecies, data.FuelSpecies), index);

            return index;
        }
        public IRegistry FrozenCopy(){
            return new FrozenCombustionRegistry(
                id_map,
                id_list,
                data_list,
                codec, 
                oxidizers, 
                fuels, 
                byReagents, 
                compliments);
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