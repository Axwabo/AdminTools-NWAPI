using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Kill : ICommand, IDefaultPermissions
    {
        public string Command => "atkill";

        public string[] Aliases { get; } =
            { };

        public string Description => "Kills everyone or a user instantly";

        public PlayerPermissions Permissions => PlayerPermissions.ForceclassToSpectator;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 1)
            {
                response = "Usage: kill ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "*" or "all":
                {
                    foreach (Player p in Player.GetPlayers().Where(Extensions.IsAlive))
                    {
                        p.Kill("Killed by admin.");
                    }

                    response = "Everyone has been game ended (killed) now";
                    return true;
                }
                default:
                {
                    Player p = Extensions.GetPlayer(arguments.At(0));
                    if (p == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!p.IsAlive)
                    {
                        response = $"Player {p.Nickname} is not a valid class to kill";
                        return false;
                    }

                    p.Kill("Killed by admin.");
                    response = $"Player {p.Nickname} has been game ended (killed) now";
                    return true;
                }
            }
        }
    }
}
