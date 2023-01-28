using CommandSystem;
using PluginAPI.Core;
using System;
using Utils.NonAllocLINQ;

namespace AdminTools.Commands.Basic
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Explode : ICommand, IDefaultPermissions
    {
        public string Command => "expl";

        public string[] Aliases { get; } =
        {
            "boom"
        };

        public string Description => "Explodes a specified user or everyone instantly";

        public PlayerPermissions Permissions => PlayerPermissions.ForceclassToSpectator;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count >= 1)
                return arguments.At(0) switch
                {
                    "*" or "all" => All(out response),
                    _ => HandleDefault(arguments, out response)
                };
            response = "Usage: expl ((player id / name) or (all / *))";
            return false;

        }
        private static bool All(out string response)
        {
            ListExtensions.ForEach(Player.GetPlayers(), DoExplosion);
            response = "Everyone exploded, Hubert cannot believe you have done this";
            return true;
        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: expl (player id / name)";
                return false;
            }

            Player p = Extensions.GetPlayer(arguments.At(0));
            if (p == null)
            {
                response = $"Invalid target to explode: {arguments.At(0)}";
                return false;
            }

            if (!p.IsAlive)
            {
                response = $"Player \"{p.Nickname}\" is not a valid class to explode";
                return false;
            }

            p.Kill("Exploded by admin.");
            Handlers.CreateThrowable(ItemType.GrenadeHE).SpawnActive(p.Position, .1f, p);
            response = $"Player \"{p.Nickname}\" game ended (exploded)";
            return true;
        }
        private static void DoExplosion(Player p)
        {
            if (!p.IsAlive)
                return;
            p.Kill("Exploded by admin.");
            Handlers.CreateThrowable(ItemType.GrenadeHE).SpawnActive(p.Position, .5f, p);
        }
    }
}
