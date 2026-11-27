using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdvancedHints;
using AdvancedHints.Enums;
using Exiled.API.Features;
using MEC;
using GamemodesManager.API.Attributes;
using GamemodesManager.API.Extensions;
using Random = UnityEngine.Random;

namespace GamemodesManager.API.Features
{
    internal static class Gamemodes
    {
        internal static readonly List<GamemodeElement> RegisteredGamemodes = new List<GamemodeElement>();
        internal static GamemodeElement CurrentGamemode { get; private set; } = null;
        internal static CoroutineHandle TimerCoroutine { get; private set; } = default;
        internal static bool AutoSpawnStatus { get; private set; } = true;

        internal static bool AutoMode { get; set; } = false;

        private static CoroutineHandle autoModeCoroutine;
        private static bool isEnabledToRunGamemode = true;
        private static GamemodeElement preGamemode;
        private static GamemodeElement nextGamemode;

        private const float _endTime = 1.2f;
        private const float _addTime = 0.01f;
        private const float _startTime = 0.1f;

        internal static void AutoMod(bool status, out string result)
        {
            if (status && autoModeCoroutine.Tag != "controller")
            {
                autoModeCoroutine = Timing.RunCoroutine(AutoModeController());
                autoModeCoroutine.Tag = "controller";
                SendInAllConsole("Внимание был включен режим AutoMode!", "red");
                result = "";
            }

            if (!status && autoModeCoroutine.Tag == "controller")
            {
                Timing.KillCoroutines(autoModeCoroutine);
                autoModeCoroutine.Tag = null;
                SendInAllConsole("Внимание! AutoMode был выключен!", "red");
                result = "";
            }

            result = "";
        }
        
        internal static void RegisterGamemodes()
        {
            int registered_gm = 0;
            var assembly = Assembly.GetExecutingAssembly();
            var gamemodes = assembly.GetTypes().Where(t => t.BaseType?.BaseType == typeof(GamemodeElement) && t.GetCustomAttribute(typeof(IgnoreRegisterAttribute)) == null);
            if (gamemodes.Count() == 0)
            {
                Log.Info("No gamemodes wasn't founded to register");
                return;
            }
            foreach (var gamemode in gamemodes)
            {
                try
                {
                    var instance = (GamemodeElement)Activator.CreateInstance(gamemode);
                    if (!instance.GetGamemodeConfig().IsEnabled) continue;
                    RegisteredGamemodes.Add(instance);
                    instance.OnRegistering();
                    Log.Info(gamemode.Name + (instance.GamemodeVersion != null ? $" v{instance.GamemodeVersion.Major}.{instance.GamemodeVersion.Minor}.{instance.GamemodeVersion.Build}" : string.Empty) + (instance.GamamemodeAuthor != null ? $" by {instance.GamamemodeAuthor}" : string.Empty) + " has been registered!");
                    registered_gm++;
                }
                catch (Exception ex)
                {
                    Error(gamemode, ex.Message);
                }
            }
        }        

        internal static void UnregisterGamemodes()
        {
            if (RegisteredGamemodes.Count > 0)
            {
                foreach (var gamemode in RegisteredGamemodes)
                {
                    gamemode.OnUnregistering();
                    RegisteredGamemodes.Remove(gamemode);
                }
            }
        }

        internal static bool RunGamemode(GamemodeElement gamemode)
        {
            if (isEnabledToRunGamemode && CurrentGamemode != null) return false;
            foreach (var player in Player.List.Where(p => p != null))
            {
                #if !DEBUG
                player.ShowManagedHint(GamemodesManager.Singleton.Config.GamemodeRoundMessage.Content.Replace("%name%", gamemode.Name).Replace("%rules%", gamemode.Rules), GamemodesManager.Singleton.Config.GamemodeRoundMessage.Duration);
                #else
                player.ShowHint(GamemodesManager.Singleton.Config.GamemodeRoundMessage.Content.Replace("%name%", gamemode.Name).Replace("%rules%", gamemode.Rules), GamemodesManager.Singleton.Config.GamemodeRoundMessage.Duration);
                #endif
                player.SendConsoleMessage(gamemode.Rules, "red");
            }
            
            gamemode.Load();
            TimerCoroutine = gamemode.Time > 0 ? Timing.RunCoroutine(GamemodeTimer(gamemode)) : default;
            CurrentGamemode = gamemode;
            AutoSpawnStatus = false;
            return true;
        }     
        
        internal static void StopGamemode(GamemodeElement gamemode)
        {
            gamemode.End();
            gamemode.Unload();
            if (TimerCoroutine != null) Timing.KillCoroutines(TimerCoroutine);
            CurrentGamemode = null;
            AutoSpawnStatus = true;
        }
        
        internal static void ShowGlobalManagedHint(string message, float duration = 3f, bool overrideQueue = false, DisplayLocation displayLocation = DisplayLocation.MiddleBottom)
        {
            foreach (Player player in Player.List.Where(p => p != null))
                player.ShowManagedHint(message, duration, overrideQueue, displayLocation);
        }
        
        private static IEnumerator<float> GamemodeTimer(GamemodeElement gamemode)
        {
            while (gamemode.Time > 0)
            {
                yield return Timing.WaitForSeconds(1f);
                if (Round.IsStarted)
                {
                    if (gamemode.Time - 1 <= 0) StopGamemode(gamemode);
                    gamemode.Time--;
                    foreach (var player in Player.List)
                        player.ShowManagedHint($"<align=left>\n\n       До конца режима: <color=#B30202>{Math.Floor(gamemode.Time / 60).ToString("00")}</color>:<color=#B30202>{Math.Floor(gamemode.Time % 60).ToString("00")}</color></align>", 1f, displayLocation: DisplayLocation.Bottom);
                }
            }
        }

        private static IEnumerator<float> RandomGamemode()
        {
            int gamemodeCount = RegisteredGamemodes.Count;
            GamemodeElement gamemode = null;
            for (float i = _startTime; i <= _endTime; i += _addTime)
            {
                gamemode = RegisteredGamemodes[Random.Range(0, gamemodeCount)];
                ShowGlobalManagedHint(gamemode.Name, 1.25f, overrideQueue: true);

                yield return Timing.WaitForSeconds(i);
            }

            nextGamemode = gamemode;
        } 

        private static IEnumerator<float> AutoModeController()
        {
            for (;;)
            {
                var randomCoroutine = Timing.RunCoroutine(RandomGamemode());
                yield return Timing.WaitUntilDone(randomCoroutine);
                ShowGlobalManagedHint("Был выбран " + nextGamemode?.Name);
                yield return Timing.WaitForSeconds(1.25f);
                if (nextGamemode?.PlayersCountToStart >
                    Player.List.Where(p => p != null && !p.IsOverwatchEnabled).Count())
                {
                    var waitingCoroutine = Timing.RunCoroutine(WaitingPlayers(nextGamemode));
                    yield return Timing.WaitUntilDone(waitingCoroutine);
                }
                for (int i = 3; i > 0; i--)
                {
                    ShowGlobalManagedHint($"Игровой режим начнется через {i}");
                    yield return Timing.WaitForSeconds(1f);
                }
                
                RunGamemode(nextGamemode); 
                //yield return Timing.WaitUntilFalse(CurrentGamemode != null);
                // Реализовать ожидание окончания геймода
            }
        }

        private static IEnumerator<float> WaitingPlayers(GamemodeElement gamemode)
        {
            for (;;)
            {
                if (gamemode.PlayersCountToStart <= Player.List.Where(p => p != null && !p.IsOverwatchEnabled).Count())
                    yield break;
                
                ShowGlobalManagedHint("Ожидание игроков...", 1.25f, overrideQueue: true);
                yield return Timing.WaitForSeconds(1.2f);
            }
        }
        
        // Реализовать ивент на рестарт раунпда и адаптирвать это в карантин
        
        private static void Error(Type gamemode, string error) => Log.Error($"Failed to reigster {gamemode.Name} gamemode due to an error: " + error);

        private static void SendInAllConsole(string message, string color)
        {
            Log.Info(message);
            foreach (var player in Player.List.Where(p => p != null))
                player.SendConsoleMessage(message, color);
        }
    }    
}