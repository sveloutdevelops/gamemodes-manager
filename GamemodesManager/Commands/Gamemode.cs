using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using GamemodesManager.API.Extensions;
using GamemodesManager.API.Interfaces;
using System;
using System.Linq;

namespace GamemodesManager.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class Gamemode : ICommand, IUsageProvider
	{
		public string Command { get; } = "gamemodes";

		public string[] Aliases { get; } = { "gm" };

		public string Description { get; } = "Управление автоматизированными режимами игры";

		public string[] Usage { get; } = { "идентификатор режима (опционально)" };

		public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission("gamemodesmanager.commands.gamemode"))
			{
				response = "У вас недостаточно прав для исполнения этой команды.\nТребуемые права: gamemodesmanager.commands.gamemode";
				return false;
			}
			
			if (API.Features.Gamemodes.RegisteredGamemodes.Count == 0)
			{
				response = "Не зарегистрировано ни одного ивента.";
				return false;
			}
			
			if (Round.IsStarted)
            {
				response = "Вы можете использовать эту команду только до начала раунда.";
				return false;
            }
			
			if (API.Features.Gamemodes.CurrentGamemode != null)
            {
				response = $"На сервере уже запущен режим {API.Features.Gamemodes.CurrentGamemode.Name}.";
				return false;
            }
			
			if (args.Count == 0)
			{
				string responseText = $"Список зарегистрированных ивентов ({API.Features.Gamemodes.RegisteredGamemodes.Count}):\n";
				foreach (var gamemode in API.Features.Gamemodes.RegisteredGamemodes)
                {
					responseText += $"[{API.Features.Gamemodes.RegisteredGamemodes.IndexOf(gamemode)}] {gamemode.Name}\n";
                }
				response = responseText;
				return true;
			}
			
			if (!int.TryParse(args.At(0), out int index))
            {
				response = "Аргумент 1 должен быть целым числом.";
				return false;
            }

			if (Math.Abs(index) > API.Features.Gamemodes.RegisteredGamemodes.Count - 1)
			{
				response = "Не существует режима с таким индексом.";
				return false;
			}
			
			var gamemodeToRun = API.Features.Gamemodes.RegisteredGamemodes[Math.Abs(index)];
            if (Player.List.Count() < gamemodeToRun.PlayersCountToStart)
			{
				response = "Недостаточно игроков на сервере для начала этого игрового режима";
				return false;
			}

            API.Features.Gamemodes.RunGamemode(gamemodeToRun);

			response = $"Вы успешно запустили режим {gamemodeToRun.Name}.";
			return true;
		}
	}
}
