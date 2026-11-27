using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Extensions
{
    internal static class InternalExtensions
    {
        internal static T RandomChoice<T>(this T[] array)
        {
            var index = Random.Range(0, array.Length-1);
            return array[index];
        }

        internal static T RandomChoice<T>(this List<T> array)
        {
            var index = Random.Range(0, array.Count-1);
            return array[index];
        }
    }
}
