using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Data;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Extensions
{
    internal static class ZoneExtensions
    {
        internal static Vector3 RandomSpawnPosition(this ZoneType players_zone)
        {
            Door door = FindDoor(players_zone);
            while (door == null)
                door = FindDoor(players_zone);
            var door_pos = door.Position;
            var help_pos = new Vector3(0.5f, 0f, 0.5f);
            var result = door_pos + help_pos;
            result.y += 0.3f;
            result.y -= help_pos.y;
            return result;
        }

        private static Door FindDoor(ZoneType zone)
        {
            var rooms = Room.Get(zone).ToList();
            foreach (var room in Rooms.IgnoreSpawnRooms)
                if (rooms.Contains(Room.Get(room))) rooms.Remove(Room.Get(room));
            int room_value = Random.Range(0, rooms.Count);
            var doors = rooms.ToList().ElementAt(room_value).Doors;
            if (doors.Count == 0) return null;
            int door_id = Random.Range(0, doors.Count());
            return doors.ToList().ElementAt(door_id);
        }
    }
}
