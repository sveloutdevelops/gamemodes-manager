using GamemodesManager.API.Extensions;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Enums;
using GamemodesManager.Gamemodes.Configs;
using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Extensions;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode
{
    internal sealed class Scoreboard
    {
        public Scoreboard() 
            : this(new List<Stats>()) {}
        public Scoreboard(List<Stats> stats)
        {
            Stats = stats;
        }

        internal string StringScoreboard => CreateScoreboard();

        private static readonly ClassicDeathmatchConfig _config = (ClassicDeathmatchConfig)ClassicDeathmatch.Instance.GetGamemodeConfig();
        internal List<Stats> Stats { get; private set; }

        private static string ScoreboardHintLocation
        {
            get
            {
                switch (_config.ScoreboardMessage.HintLocation)
                {
                    case HintLocation.Right:
                        return "right";
                    case HintLocation.Left:
                        return "left";
                    default:
                        return "right";
                }
            }
        }
        private static int ScoreboardHintSize
        {
            get
            {
                if (_config.ScoreboardMessage.HintSize > 50 | _config.ScoreboardMessage.HintSize < 1) return 50;
                return _config.ScoreboardMessage.HintSize;
            }
        }

        private const string _scoreboardTitle = "Таблица очков   \t\n" + "Игрок                  \tУбийства\n\n";
        private const string _statsFormat = "{0}{1}{2}\n\n";
        private const ushort _playerNicknameMaxChars = 8;

        internal void DeleteScoreboard() => Stats = null;

        internal void UpdateScoreboard()
        {
            if (Stats == null) return;
            List<Stats> new_list = new List<Stats>();
            List<int> kills = new List<int>();
            for (int i = 0; i < Stats.Count; i++)
                kills.Add(Stats.ElementAt(i).Kills);
            for (int i = 0; i < Stats.Count; i++)
            {
                var score = kills.Max();
                var player = Stats.Find(obj => obj.Kills == score);
                new_list.Add(player);
                kills.Remove(score);
            }
            Stats = new_list;
        }

        private string CreateScoreboard()
        {
            if (Stats == null) return string.Empty;
            int scoreboard_lines = 0;
            int remove_lines = 0;
            StringBuilder scoreboardBuilder = new StringBuilder();
            scoreboardBuilder.Append($"<align={ScoreboardHintLocation}><size={ScoreboardHintSize}>");
            scoreboardBuilder.Append(_scoreboardTitle);
            foreach ((Player player, int kills) in Stats.Where(pl => pl.Player != null))
            {
                if (scoreboard_lines == 5 && Player.List.Count() > 20) break;
                if (kills >= 10) remove_lines++;
                object[] format_objs = new object[3] { 
                    (player.DisplayNickname.Truncate(_playerNicknameMaxChars).Length < 5 ? player.DisplayNickname.Truncate(_playerNicknameMaxChars) + CreateExtraSpace((5 - player.DisplayNickname.Truncate(_playerNicknameMaxChars).Length) * 2) : player.DisplayNickname.Truncate(_playerNicknameMaxChars)) + '\t',
                    CreateExtraSpace(30),
                    kills };
                string format = string.Format(_statsFormat, format_objs);
                scoreboardBuilder.Append((remove_lines > 0 ? format.Remove(format.Length - remove_lines * 2) : format) + '\t');
                scoreboard_lines++;
            }
            scoreboardBuilder.Append("</size></align>");
            return scoreboardBuilder.ToString();
        }

        private static string CreateExtraSpace(int count)
        {
            if (count == 0) return "";
            int length = count;
            string space = " ";
            string result = "";
            for (int i = 0; i <= length; i++)
                result += space;
            return result;
        }        
    }
}
