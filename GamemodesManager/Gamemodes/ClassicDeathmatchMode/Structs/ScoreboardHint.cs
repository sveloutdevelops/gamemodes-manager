using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Enums;
using System.ComponentModel;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Structs
{
    public class ScoreboardHint
    {
        public ScoreboardHint() : this(default, default) {}
        public ScoreboardHint(int hintSize, HintLocation hintLocation)
        {
            HintSize = hintSize;
            HintLocation = hintLocation;
        }

        [Description("Size of hint (min value = 1) (max value = 100)")]
        public int HintSize { get; set; }

        [Description("Hint Location (Right or Left)")]
        public HintLocation HintLocation { get; set; }       
    }
}
