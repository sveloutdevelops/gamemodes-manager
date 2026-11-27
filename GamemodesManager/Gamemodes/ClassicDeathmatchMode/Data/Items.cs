using GamemodesManager.API.Extensions;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Interfaces;
using GamemodesManager.Gamemodes.Configs;
using System.Collections.Generic;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Data
{
    internal class Items : IData
    {
        internal static Dictionary<ItemType, double> randomPickups = new Dictionary<ItemType, double>();      
        public bool IsFilled { get; set; } = false;

        public void Init()
        {
            var config_list = ((ClassicDeathmatchConfig)ClassicDeathmatch.Instance.GetGamemodeConfig()).PickupItems;
            foreach (var obj in config_list)
                randomPickups.Add(obj.Key, obj.Value);
        }

        public void Clear() => randomPickups.Clear();
    }
}
