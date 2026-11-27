using System;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace GamemodesManager.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AutoMod : ICommand, IUsageProvider
    {
        public string Command => "automod";

        public string[] Aliases { get; } = { };

        public string Description => "Включает режим AutoMod";

        public string[] Usage { get; } = { "Значение AutoMod 1 или 0" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("gamemodesmanager.commands.automod"))
            {
                response = "У вас недостаточно прав для исполнения этой команды.\nТребуемые права: gamemodesmanager.commands.automod";
                return false;
            }

            if (int.TryParse(arguments.At(0), out int result))
            {
                switch (result)
                {
                    case 1:
                        if (API.Features.Gamemodes.AutoMode)
                        {
                            response = "AutoMod уже включен!";
                        }
                }
            }
        }

    }
}