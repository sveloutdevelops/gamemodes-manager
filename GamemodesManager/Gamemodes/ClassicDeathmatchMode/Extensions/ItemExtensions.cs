using Exiled.API.Features.Items;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Extensions
{
    internal static class ItemExtensions
    {
        internal static void CreateRandomPickup(this Item item) => item.CreatePickup(RandomPickupPosition());

        private static Vector3 RandomPickupPosition()
        {
            var room = Rooms.rooms.RandomChoice();
            var random_add_pos = new Vector3(Random.Range(1f, 1.5f), 0, Random.Range(1, 1.5f));
            var result = room.Position + random_add_pos;
            return result;
        }
    }
}
