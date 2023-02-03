using CommandSystem;
using System;
using System.Collections.Generic;

namespace AdminTools.Commands.Ghost
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public sealed class TargetGhost : ICommand, IDefaultPermissions
    {

        public string Command => "targetghost";

        public string[] Aliases { get; } =
        {
            "tghost"
        };

        public string Description => "Makes a player invisible to another player or all players";

        public PlayerPermissions Permissions => PlayerPermissions.Effects;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 2)
            {
                response = "Usage: targetghost <player to hide> <target or (all/*)>";
                return false;
            }

            AtPlayer player = Extensions.GetPlayer(arguments.At(0));
            if (player != null)
                return arguments.At(1).ToLower() switch
                {
                    "all" or "*" => All(player, out response),
                    _ => HandleDefault(player, arguments, out response)
                };

            response = $"Player {arguments.At(0)} not found.";
            return false;
        }
        private static bool All(AtPlayer player, out string response)
        {
            GhostController controller = GetController(player);
            controller.IsFullyInvisible = !controller.IsFullyInvisible;
            response = $"Player {player.Nickname} is now {(controller.IsFullyInvisible ? "invisible" : "visible")} to everyone.";
            return true;
        }
        private static bool HandleDefault(AtPlayer player, ArraySegment<string> arguments, out string response)
        {
            AtPlayer hideFor = Extensions.GetPlayer(arguments.At(1));
            if (hideFor == null)
            {
                response = $"Player {arguments.At(1)} not found.";
                return false;
            }

            HashSet<string> set = GetController(player).InvisibleTo;
            if (set.Add(hideFor.UserId))
            {
                response = $"{player.Nickname} is now invisible to {hideFor.Nickname}.";
                return true;
            }
            set.Remove(hideFor.UserId);
            response = $"{hideFor.Nickname} can now see {player.Nickname}.";
            return true;
        }


        private static GhostController GetController(AtPlayer player) => player.GameObject.TryGetComponent(out GhostController controller)
            ? controller
            : player.GameObject.AddComponent<GhostController>();

    }
}
