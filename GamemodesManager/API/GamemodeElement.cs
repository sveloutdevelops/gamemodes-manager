using GamemodesManager.API.Attributes;
using System;

namespace GamemodesManager.API
{
    [IgnoreRegister]
    internal abstract class GamemodeElement
    {
        public virtual string Name { get; }
        public virtual string Rules { get; }
        internal virtual float Time { get; set; }
        internal virtual Version GamemodeVersion { get; }
        internal virtual string GamamemodeAuthor { get; }
        internal virtual uint PlayersCountToStart { get; }

        public virtual void Load() {}
        public virtual void Unload() {}
        public virtual void End() {}

        internal virtual void OnRegistering() {}
        internal virtual void OnUnregistering() {}
    }
}
