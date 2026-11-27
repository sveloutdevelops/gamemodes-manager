using Exiled.API.Enums;
using Exiled.API.Features;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Data 
{
    internal class Rooms : IData
    {
        internal static List<Room> rooms { get; set; } = new List<Room>();

        internal static readonly RoomType[] IgnoreSpawnRooms = new RoomType[] 
        { 
            RoomType.HczHid,
            RoomType.Hcz079,
        };

        public bool IsFilled { get; set; } = false; 

        public void Init()
        {
            var lcz_rooms = Room.Get(ZoneType.LightContainment);
            var hcz_rooms = Room.Get(ZoneType.HeavyContainment);
            var entrance = Room.Get(ZoneType.Entrance);
            AddRoomsToArray(lcz_rooms.ToList());
            AddRoomsToArray(hcz_rooms.ToList());
            AddRoomsToArray(entrance.ToList());
        }

        public void Clear() => rooms.Clear();

        private static void AddRoomsToArray(List<Room> _rooms)
        {
            foreach (var room in _rooms)
                rooms.Add(room);
        }
    }
}
