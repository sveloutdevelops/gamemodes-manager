using System.ComponentModel;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Structs
{
    public class Boss
    {
        public Boss() : this(default, default, default) { }
        public Boss(float bossMaxHealth, float bossShieldHealth, double bossSpawnChance)
        {
            BossMaxHealth = bossMaxHealth;
            BossShieldHealth = bossShieldHealth;
            BossSpawnChance = bossSpawnChance;
        }

        [Description("Boss max health (float) (You can set the value to -1, then the max health of the boss will be equal to the max health of his role) default = -1")]
        public float BossMaxHealth { get; set; }

        [Description("Boss max shield heatlh (float) (You can set the value to -1, then the max shield health of the boss will be equal to the max shield health of his role) default = -1")]
        public float BossShieldHealth { get; set; }

        [Description("Chance of spawn player as boss (double)")]
        public double BossSpawnChance { get; set; }
    }
}
