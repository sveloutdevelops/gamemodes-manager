 using System;
using System.Diagnostics.Eventing.Reader;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using GamemodesManager.API.Features;
using Server = Exiled.Events.Handlers.Server;

namespace GamemodesManager
{
    internal sealed class GamemodesManager : Plugin<Config>
    {
        public override string Prefix => "gamemodesmanager";
        public override string Name => "GamemodesManager";
        public override string Author => "SveloutDevelops";
        
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(7, 0, 0);
        public static GamemodesManager Singleton = new GamemodesManager();

        public override void OnEnabled()
        {
            Singleton = this;
            Server.RestartingRound += OnRestartingRound;
            Server.RespawningTeam += OnRespawningTeam;
            
            RegisterGamemodes();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Singleton = null;
            Server.RestartingRound -= OnRestartingRound;
            Server.RespawningTeam -= OnRespawningTeam;

            UnregisterGamemodes();

            base.OnDisabled();
        }

        private void RegisterGamemodes() => API.Features.Gamemodes.RegisterGamemodes();

        private void UnregisterGamemodes() => API.Features.Gamemodes.UnregisterGamemodes();

        private void OnRestartingRound()
        {
            if (API.Features.Gamemodes.CurrentGamemode != null)
                API.Features.Gamemodes.StopGamemode(API.Features.Gamemodes.CurrentGamemode);
        }

        private void OnRespawningTeam(RespawningTeamEventArgs ev) => ev.IsAllowed = API.Features.Gamemodes.AutoSpawnStatus;
    }
}
