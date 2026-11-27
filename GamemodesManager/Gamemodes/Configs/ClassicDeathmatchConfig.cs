using System.Collections.Generic;
using System.ComponentModel;
using GamemodesManager.API.Interfaces;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Enums;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Structs;

namespace GamemodesManager.Gamemodes.Configs
{
    public sealed class ClassicDeathmatchConfig : IGamemodeConfig
    {           
        public bool IsEnabled { get; set; } = true;

        [Description("Event duration in seconds (float)")]
        public float Duration { get; set; } = 3600f;

        [Description("Items which will added after start of the event")]
        public List<ItemType> StartItems { get; set; } = new List<ItemType>()
        {
             ItemType.GunCOM15,
             ItemType.Ammo9x19,
             ItemType.Ammo12gauge,
             ItemType.KeycardZoneManager,
             ItemType.GunShotgun
        };

        [Description("Score to win")]
        public int WinScore { get; set; } = 40;

        [Description("Scoreboard message settings")]
        public ScoreboardHint ScoreboardMessage { get; set; } = new ScoreboardHint()
        {
            HintLocation = HintLocation.Right,
            HintSize = 50
        };

        [Description("Player died settings")]
        public Died DiedSettings { get; set; } = new Died()
        {
            DiedMessageByAttacker = "Вас убил %attacker%\nВы возродитесь через %time% секунд",
            DiedMessageByPlayerSelf = "Вы приняли ислам\nВы ворзродитесь через %time% секунд",
            JoinedPlayerSpawnMessage = "Добро пожаловать!\nСейчас проходит игровой режим ClassicDeathmatch\nВы возвродитесь через %time% секунд",
            SecondsToRespawn = 3
        };

        [Description("Win message (%winner% is player who win)")]
        public WinMessage WinMessage { get; set; } = new WinMessage()
        {
            Content = "%winner% выиграл",
            Duration = 3f,
            Type = WinMessageType.Broadcast
        };

        [Description("Boss settings")]
        public Boss BossSettings { get; set; } = new Boss()
        {
            BossMaxHealth = -1,
            BossShieldHealth = -1,
            BossSpawnChance = 50
        };

        [Description("Item which will random spawns in random positions while game(chance(Double) and list of item which will spawns with this chance")]
        public Dictionary<ItemType, double> PickupItems { get; set; } = new Dictionary<ItemType, double>()
        {
            {
                ItemType.Flashlight,
                100
            },
            {
                ItemType.Coin,
                50
            }
        };
    }
}