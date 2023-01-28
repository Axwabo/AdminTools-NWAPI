using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class TeleportX : ICommand, IDefaultPermissions
    {
        public string Command => "teleportx";

        public string[] Aliases { get; } =
        {
            "tpx"
        };

        public string Description => "Teleports all users or a user to another user";

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count >= 2)
                return arguments.At(0) switch
                {
                    "*" or "all" => All(arguments, out response),
                    _ => HandleDefault(arguments, out response)
                };
            response = "Usage: teleportx (People teleported: (player id / name) or (all / *)) (Teleported to: (player id / name) or (all / *))";
            return false;

        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            Player toTarget = Extensions.GetPlayer(arguments.At(0));
            if (toTarget == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            Player playerToTeleport = Extensions.GetPlayer(arguments.At(1));
            if (playerToTeleport == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            toTarget.Position = playerToTeleport.Position;
            response = $"Player {toTarget.Nickname} has been teleported to Player {playerToTeleport.Nickname}";
            return true;
        }
        private static bool All(ArraySegment<string> arguments, out string response)
        {
            Player target = Extensions.GetPlayer(arguments.At(1));
            if (target == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }


            foreach (Player p in Player.GetPlayers().Where(Extensions.IsAlive))
                p.Position = target.Position;

            response = $"Everyone has been teleported to Player {target.Nickname}";
            return true;
        }
    }
}
