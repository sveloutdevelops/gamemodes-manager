using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Enums;
using System.ComponentModel;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Structs
{
    public class WinMessage
    {
        public WinMessage() : this(default, default, default) { }
        public WinMessage(WinMessageType type, string content, float duration)
        {
            Type = type;
            Content = content;
            Duration = duration;
        }

        [Description("Type of message (Broadcast or Hint)")]
        public WinMessageType Type { get; set; }

        [Description("Message content(%winner% is nickname of player which win game)")]
        public string Content { get; set; }

        [Description("Message duration (float)")]
        public float Duration { get; set; }
    }
}
