#if !DEBUG
using AdvancedHints;
using AdvancedHints.Enums;
#endif

using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using GamemodesManager.API;
using GamemodesManager.API.Extensions;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Data;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Enums;
using GamemodesManager.Gamemodes.ClassicDeathmatchMode.Extensions;
using GamemodesManager.Gamemodes.Configs;
using Interactables.Interobjects.DoorUtils;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs.Scp049;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode
{
    internal sealed class ClassicDeathmatch : Gamemode<ClassicDeathmatchConfig>
    {
        public override string Name => "Classic Deathmatch";
        public override string Rules => "Вы появляетесь в случайной комнате в определенной зоне" +
            $"\nВаша задача достичь максимального количества убийств, а именно {config?.WinScore}" +
            "\nВы не можете покинуть свою зону, после смерти вы появляетесь в ней же" +
            "\nРаз в некоторое время на карте появляется дроп, это может быть оружие или другие полезные вещи" +
            $"\nС шансом {config?.BossSettings.BossSpawnChance}% вы можете появиться как мини босс ввиде SCP";
        public override string Author => "SveloutDevelops";
        public override Version Version => new Version(1, 0, 0);

        internal static ClassicDeathmatchConfig config;

        private CoroutineHandle itemsGiver;
        private CoroutineHandle scoreboardCorountine;
        private Dictionary<Player, ZoneType> playersZones = new Dictionary<Player, ZoneType>();
        private Dictionary<Player, Vector3> lockedSpawns = new Dictionary<Player, Vector3>();
        private static Scoreboard scoreboard;
        internal static ClassicDeathmatch Instance;
        internal static Player Winner { get; private set; } = null;

        private static float BossMaxHealth => config.BossSettings.BossMaxHealth;
        private static float BossMaxShieldHealth => config.BossSettings.BossShieldHealth;

        public override void OnRegistered() => config = (ClassicDeathmatchConfig)this.GetGamemodeConfig();

        public override void OnUnregistered() => config = null;

        public override void Load()
        {           
            scoreboard = new Scoreboard();
            Instance = this;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Player.Joined += OnPlayerJoined;
            Exiled.Events.Handlers.Scp049.StartingRecall += OnStartingRecall;
            DataExtensions.Init();
            Round.IsLocked = true;
            Server.FriendlyFire = true;
            Round.IsLobbyLocked = true;
        }

        public override void Unload()
        {
            scoreboard.DeleteScoreboard();
            scoreboard = null;
            Instance = null;
            DataExtensions.Clear();
            Round.IsLocked = false;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.Joined -= OnPlayerJoined;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Scp049.StartingRecall -= OnStartingRecall;
        }

        public override void End()
        {
            Timing.KillCoroutines(scoreboardCorountine);
            Timing.KillCoroutines(itemsGiver);
            EndGame();
        }

        public void OnRoundStarted() => StartGame();

        public void OnPlayerDied(DiedEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Attacker != null)
            {
                if (ev.Attacker.Role.Team != Team.SCPs && ev.Player.Role.Team == Team.SCPs)
                {
                    if (ev.Player.Role == RoleTypeId.Scp096)
                    {
                        scoreboard.Stats.Find(obj => obj.Player == ev.Attacker).Kills += 3;
                    }
                    else
                    {
                        scoreboard.Stats.Find(obj => obj.Player == ev.Attacker).Kills += 2;
                    }
                }
                else
                {
                    scoreboard.Stats.Find(obj => obj.Player == ev.Attacker).Kills += 1;
                }
                scoreboard.UpdateScoreboard();
                if (scoreboard.Stats.Find(obj => obj.Player == ev.Attacker).Kills == config.WinScore)
                {
                    API.Features.Gamemodes.StopGamemode(this);
                }
            }
            ev.Player.ClearDeadModel();
            Timing.RunCoroutine(DiedMessage(ev.Player, ev.Attacker != null ? ev.Attacker : null));
        }

        public void OnStartingRecall(StartingRecallEventArgs ev) => ev.IsAllowed = false;

        public void OnJumping(JumpingEventArgs ev)
        {
            
        }

        public void OnPlayerJoined(JoinedEventArgs ev) => Timing.CallDelayed(3f,
            () => Timing.RunCoroutine(DiedMessage(ev.Player, isJoinedPlayer: true)));

        private void EndGame()
        {
            if (Winner != null)
            {
                switch (config.WinMessage.Type)
                {
                    case WinMessageType.Broadcast:
                        Map.Broadcast((ushort)config.WinMessage.Duration, config.WinMessage.Content.Replace("%winner%", Winner?.Nickname));
                        break;
                    case WinMessageType.Hint:
                        foreach (Player player in Player.List.Where(pl => pl != null))
                        {
                            #if !DEBUG
                            player.ShowManagedHint(config.WinMessage.Content.Replace("%winner%", Winner?.Nickname), config.WinMessage.Duration, displayLocation: DisplayLocation.MiddleTop);
                            #else
                            player.ShowHint(config.WinMessage.Content.Replace("%winner%", Winner?.Nickname), config.WinMessage.Duration);
                            #endif
                        }
                        break;
                }
            }
            foreach (Player player in Player.List.Where(pl => pl != null))
            {
                if (Winner != null && Winner == player) continue;
                player.ClearInventory();
            }
            Round.EndRound();
        }

        private void StartGame()
        { 
            var boss_chance = Random.Range(0f, 100f);
            Player boss = null;
            foreach (Player player in Player.List.Where(pl => pl != null && !pl.IsOverwatchEnabled))
                player.Role.Set(RoleTypeId.ClassD);
            foreach (Player player in Player.List.Where(pl => pl != null && pl.Role.Team == Team.ClassD))
                scoreboard.Stats.Add(new Stats(player));
            scoreboardCorountine = Timing.RunCoroutine(ShowScoreboard());
            if (boss_chance <= config.BossSettings.BossSpawnChance)
            {
                boss = Player.List.ToList().RandomChoice();
                SpawnBoss(boss);
            }
            foreach (Player player in Player.List.Where(pl => pl != null && !pl.IsOverwatchEnabled && pl.Role.Team == Team.ClassD))
            {
                var zone = Random.Range(0, Zones.zones.Length-1);
                if (boss != null && player == boss) continue;
                playersZones.Add(player, Zones.zones[zone]);
                player.IsBypassModeEnabled = true;
                SpawnPlayer(player);
            }
            itemsGiver = Timing.RunCoroutine(ItemsGiver());
            BlockDoors();
        }

        private IEnumerator<float> DiedMessage(Player dead, Player attacker = null, bool isJoinedPlayer = false)
        {
            bool isHimselfDead = attacker == null;
            var coroutine = Timing.CallDelayed(0.1f, delegate
            {
                Vector3 finalPosition = playersZones[dead].RandomSpawnPosition();
                while (lockedSpawns.ContainsValue(finalPosition))
                    finalPosition = playersZones[dead].RandomSpawnPosition();

                lockedSpawns[dead] = finalPosition;
            });
            yield return Timing.WaitUntilDone(coroutine);
            for (int i = config.DiedSettings.SecondsToRespawn; i > 0; i--)
            {
#if !DEBUG
                if (!isHimselfDead) dead.ShowManagedHint(config.DiedSettings.DiedMessageByAttacker.Replace("%time%", i.ToString()).Replace("%attacker%", attacker.Nickname), 1.2f);
                else if (isJoinedPlayer) dead.ShowManagedHint(config.DiedSettings.JoinedPlayerSpawnMessage.Replace("%time%", i.ToString()), 1.25f);
                else dead.ShowManagedHint(config.DiedSettings.DiedMessageByPlayerSelf.Replace("%time%", i.ToString()), 1.25f);
#else
                if (!isHimselfDead) dead.ShowHint(config.DiedSettings.DiedMessageByAttacker.Replace("%time%", i.ToString()).Replace("%attacker%", attacker.Nickname), 1.2f);
                else if (IsJoinedPlayer) dead.ShowHint(config.DiedSettings.JoinedPlayerSpawnMessage.Replace("%time%", i.ToString()), 1.25f);
                else dead.ShowHint(config.DiedSettings.DiedMessageByPlayerSelf.Replace("%time%", i.ToString()), 1.25f);
#endif
                yield return Timing.WaitForSeconds(1f);
            }
            var boss_chance = Random.Range(0f, 100f);
            if (boss_chance <= config.BossSettings.BossSpawnChance)
            {
                Player boss = Player.List.ToList().RandomChoice();
                SpawnBoss(boss, true);
            }
            else SpawnPlayer(dead, true);
        }

        private void SpawnPlayer(Player player, bool respawn = false, bool IsScp = false)
        {
            Vector3 door;
            if (player == null) return;
            if (respawn)
            {                
                if (!IsScp) player.Role.Set(RoleTypeId.ClassD);
                door = lockedSpawns[player];
            }
            else door = playersZones[player].RandomSpawnPosition();
            if (!IsScp)
                for (int i = 0; i < (config.StartItems.Count <= 8 ? config.StartItems.Count : 8); i++)
                    player.AddItem(config.StartItems[i]);
            lockedSpawns.Remove(player);
            player.Teleport(door);
        }

        private void SpawnBoss(Player player, bool respawn = false)
        {
            var role = Scps.scpRoles.RandomChoice();
            player.Role.Set(role);
            if (BossMaxHealth > -1) player.MaxHealth = BossMaxHealth;
            if (BossMaxShieldHealth > -1) player.HumeShield = BossMaxShieldHealth;
            SpawnPlayer(player, respawn == true ? true : false, IsScp: true);
        }      

        private static IEnumerator<float> ShowScoreboard()
        {
            for (; ;)
            {
                foreach ((Player player, int kills) in scoreboard.Stats.Where(pl => pl.Player != null && pl.Player.IsAlive))
                {
                    string __scoreboard = scoreboard.StringScoreboard;
#if !DEBUG
                    player.ShowManagedHint(__scoreboard, 1.25f, displayLocation: DisplayLocation.Top);
#else
                    player.ShowHint(__scoreboard, 1.25f);
#endif
                }
                yield return Timing.WaitForSeconds(1f);
            }
        } 

        private static void BlockDoors()
        {
            if (Rooms.IgnoreSpawnRooms.Length > 0)
                foreach (var room in Rooms.IgnoreSpawnRooms)
                    Room.Get(room).LockDown(-1, DoorLockType.NoPower);
            Lift.Get(ElevatorType.LczA).ChangeLock(DoorLockReason.AdminCommand);
            Lift.Get(ElevatorType.LczB).ChangeLock(DoorLockReason.NoPower);
            Door.Get(DoorType.CheckpointEzHczA).ChangeLock(DoorLockType.NoPower);
            Door.Get(DoorType.CheckpointEzHczB).ChangeLock(DoorLockType.NoPower);
            Door.Get(DoorType.GateA).ChangeLock(DoorLockType.NoPower);
            Door.Get(DoorType.GateB).ChangeLock(DoorLockType.NoPower);
        }

        private static IEnumerator<float> ItemsGiver()
        {
            for (; ;)
            {
                var update_time = Random.Range(1, 2);
                var do_count = Random.Range(3, 6);
                var chance = Random.Range(0f, 100f);
                for (int i = 0; i <= do_count; i++)
                {
                    foreach (var obj in config.PickupItems)
                    {
                        if (chance <= obj.Value)
                        {
                            var item = Item.Get(obj.Key.GetItemBase());
                            item.CreateRandomPickup();
                            break;
                        }
                    } 
                }
                yield return Timing.WaitForSeconds(update_time * 60);
            }
        }
    }    
}
