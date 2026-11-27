using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Extensions
{
    internal static class PlayerExtensions
    {
        internal static readonly Dictionary<Player, bool> DoubleJumps = new Dictionary<Player, bool>();
        
        private const float _speed = 10f;
        public static void ClearDeadModel(this Player player)
        {
            var playerModel = player.GameObject;
            Timing.CallDelayed(5f, delegate
            {
                Object.Destroy(playerModel);
                playerModel = null;
            });
            while (playerModel != null)
            {
                var pos = new Vector3(0, 0.01f, 0);
                playerModel.transform.position -= pos * (Time.deltaTime * _speed);
            }
        }

        public static bool IsDoubleJumpAvailable(this Player player)
        {
            if (!DoubleJumps[player]) return false;

            return true;
        }
    }
}
