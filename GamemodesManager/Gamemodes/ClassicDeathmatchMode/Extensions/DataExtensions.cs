using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Interfaces;
using System.Linq;
using System.Reflection;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Extensions
{
    internal static class DataExtensions
    {
        public static void Init()
        {
            var data = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "GamemodesManager.Gamemodes.ClassicDeathmatchMode.Data" && t.GetInterface(nameof(IData)) != null);
            foreach (var type in data)
            {
                if (type.IsAssignableFrom(typeof(IData)))
                {
                    if (!((IData)type).IsFilled)
                    {
                        ((IData)type).Init();
                        ((IData)type).IsFilled = true;
                    }
                }
            }              
        }

        public static void Clear()
        {
            var data = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "GamemodesManager.Gamemodes.ClassicDeathmatchMode.Data" && t.GetInterface(nameof(IData)) != null);
            foreach (var type in data)
            {
                if (type.IsAssignableFrom(typeof(IData)))
                {
                    if (((IData)type).IsFilled)
                    {
                        ((IData)type).Clear();
                        ((IData)type).IsFilled = false;
                    }
                }
            }
        }
    }
}
