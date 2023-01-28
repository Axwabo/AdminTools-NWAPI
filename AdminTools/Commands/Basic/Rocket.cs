using CommandSystem;
using MEC;
using PluginAPI.Core;
using System;

namespace AdminTools.Commands.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Rocket : ICommand, IDefaultPermissions
    {
        public string Command => "rocket";

        public string[] Aliases { get; } =
            { };

        public string Description => "Sends players high in the sky and explodes them";

        public PlayerPermissions Permissions => PlayerPermissions.ForceclassWithoutRestrictions;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count >= 2)
                return arguments.At(0) switch
                {
                    "all" => All(arguments, out response),
                    _ => HandleDefault(arguments, out response)
                };
            response = "Usage: rocket ((player id / name) or (all / *)) (speed)";
            return false;

        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            Player p = Extensions.GetPlayer(arguments.At(0));

            if (p == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }
            if (!p.IsAlive)
            {
                response = $"Player {p.Nickname} is not a valid class to rocket";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float spd) && spd <= 0)
            {
                response = $"Speed argument invalid: {arguments.At(1)}";
                return false;
            }

            Timing.RunCoroutine(EventHandlers.DoRocket(p, spd));
            response = $"Player {p.Nickname} has been rocketed into the sky (We're going on a trip, in our favorite rocketship)";
            return true;
        }
        private static bool All(ArraySegment<string> arguments, out string response)
        {
            if (!float.TryParse(arguments.At(1), out float speed) && speed <= 0)
            {
                response = $"Speed argument invalid: {arguments.At(1)}";
                return false;
            }

            foreach (Player ply in Player.GetPlayers())
                Timing.RunCoroutine(EventHandlers.DoRocket(ply, speed));

            response = "Everyone has been rocketed into the sky (We're going on a trip, in our favorite rocketship)";
            return true;
        }
    }
}
