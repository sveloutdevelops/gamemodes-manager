using Exiled.API.Features;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode
{
    internal sealed class Stats
    {       
        public Stats(Player player) 
            : this(player, 0) {}
        public Stats(Player player, int kills)
        {
            Player = player;
            Kills = kills;
        }
        public Player Player { get; set; }
        public int Kills { get; set; }   
        
        public void Deconstruct(out Player player, out int kills)
        {
            player = Player;
            kills = Kills;
        }
    }
}
