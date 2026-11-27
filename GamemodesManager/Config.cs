using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using GamemodesManager.Gamemodes.Configs;

namespace GamemodesManager
{
    public class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Show debug messages in the server console?")]
        public bool Debug { get; set; } = false;

        public Hint GamemodeRoundMessage { get; set; } = new Hint("В этом раунде будет запущен режим %name%\n%rules%.");

        public ClassicDeathmatchConfig ClassicDeathmatchConfig { get; set;} = new ClassicDeathmatchConfig();
    }
}