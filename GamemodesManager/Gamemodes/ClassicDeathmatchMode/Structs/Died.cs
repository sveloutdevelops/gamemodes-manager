using System.ComponentModel;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Structs
{
    public sealed class Died
    {
        public Died() : this(default, default, default, default) { }
        public Died(string diedMessageWithAttacker, string diedMessageByPlayerSelf, string joinedPlayerSpawnMessage, int secondsToRespawn)
        {
            DiedMessageByAttacker = diedMessageWithAttacker;
            DiedMessageByPlayerSelf = diedMessageByPlayerSelf;
            JoinedPlayerSpawnMessage = joinedPlayerSpawnMessage;
            SecondsToRespawn = secondsToRespawn;
        }

        [Description("Message which show died player, if the player was killed by the attacker(%time% is seconds to respawn) (%attacker% is nickname of killer)")]
        public string DiedMessageByAttacker { get; set; }

        [Description("Messsage which show died player, if the player died himself (%time% is seconds to respawn)")]
        public string DiedMessageByPlayerSelf { get; set; }

        [Description("Message which show joined player while running gamemode (%time%) is seconds to respawn")]
        public string JoinedPlayerSpawnMessage { get; set; }

        [Description("Seconds to respawn (int)")]
        public int SecondsToRespawn { get; set; }
    }
}
