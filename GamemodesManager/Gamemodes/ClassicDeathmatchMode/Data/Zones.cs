using Exiled.API.Enums;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Data
{
    internal readonly struct Zones
    {
        internal static ZoneType[] zones = new ZoneType[3]
        {
            ZoneType.Entrance,
            ZoneType.HeavyContainment,
            ZoneType.LightContainment
        };
    }
}
