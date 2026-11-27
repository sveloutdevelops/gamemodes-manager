using GamemodesManager.API.Attributes;
using GamemodesManager.API.Interfaces;
using Exiled.API.Features;
using System;

namespace GamemodesManager.API
{
    /// <summary>
    /// Abstract class for creating your gamemodes
    /// </summary>
    /// <typeparam name="T">Config of your gamemode, config must be inherited by <see cref="IGamemodeConfig"/></typeparam>
    [IgnoreRegister]
    internal abstract class Gamemode<T> : GamemodeElement where T : IGamemodeConfig
    {
        /// <summary>
        /// Gets name of gamemode
        /// </summary>
        public override abstract string Name { get; }

        /// <summary>
        /// Gets rules of gamemode
        /// </summary>
        public override abstract string Rules { get; }

        /// <summary>
        /// Author of gamemode
        /// </summary>
        public virtual string Author { get; } = null;

        /// <summary>
        /// Version of gamemode
        /// </summary>
        public virtual Version Version { get; } = null;

        /// <summary>
        /// Gets or sets duration of gamemode, default = unlimited duration
        /// </summary>
        public virtual float GamemodeDuration { get; } = -1;

        /// <summary>
        /// Gets or sets count of <see cref="Player"/> to start gamemode, default = 0
        /// </summary>
        public virtual uint PlayerCountToStart { get; } = 0;

        /// <summary>
        /// After enabling gamemode to next round
        /// </summary>
        public override abstract void Load();

        /// <summary>
        /// After disabling gamemode
        /// </summary>
        public override abstract void Unload();

        /// <summary>
        /// After ending gamemode
        /// </summary>
        public override abstract void End();

        /// <summary>
        /// After registering gamemode
        /// </summary>
        public virtual void OnRegistered() {}

        /// <summary>
        /// After unregistering gamemode
        /// </summary>
        public virtual void OnUnregistered() {}

        
        internal sealed override string GamamemodeAuthor => Author;
        internal sealed override uint PlayersCountToStart => PlayerCountToStart;
        internal sealed override float Time => GamemodeDuration;
        internal sealed override void OnRegistering() => OnRegistered();
        internal sealed override void OnUnregistering() => OnUnregistered();

    }
}
