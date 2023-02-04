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
            if (arguments.Count > 1)
                return arguments.At(0).ToLower() switch
                {
                    "all" or "*" => All(arguments, out response),
                    _ => HandleDefault(arguments, out response)
                };
            response = "Usage: targetghost <player to hide or (all/*)> <target or (all/*)>";
            return false;

        }
        private static bool All(ArraySegment<string> arguments, out string response)
        {
            if (HideAllFully(arguments, out response))
                return true;

            AtPlayer player = Extensions.GetPlayer(arguments.At(1));
            if (player == null)
            {
                response = $"Player {arguments.At(1)} not found.";
                return false;
            }

            bool makeInvisible = true;
            List<AtPlayer> players = Extensions.Players;
            for (int i = 0; i < players.Count; i++)
            {
                GhostController controller = GetController(players[i]);
                if (i == 0 && controller.InvisibleTo.Contains(player.UserId))
                    makeInvisible = false;
                if (makeInvisible)
                    controller.InvisibleTo.Add(player.UserId);
                else
                    controller.InvisibleTo.Remove(player.UserId);
            }
            response = $"Everyone is now {(makeInvisible ? "invisible" : "visible")} to player {player.Nickname}";
            return true;
        }
        private static bool HideAllFully(ArraySegment<string> arguments, out string response)
        {
            if (arguments.At(1).ToLower() is not ("all" or "*"))
            {
                response = "";
                return false;
            }
            bool makeInvisible = false;
            List<AtPlayer> list = Extensions.Players;
            for (int i = 0; i < list.Count; i++)
            {
                AtPlayer p = list[i];
                GhostController control = GetController(p);
                if (i == 0)
                    makeInvisible = !control.IsFullyInvisible;
                control.IsFullyInvisible = makeInvisible;
            }

            response = $"All players are now {(makeInvisible ? "invisible" : "visible")}.";
            return true;
        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            AtPlayer player = Extensions.GetPlayer(arguments.At(0));
            if (player == null)
            {
                response = $"Player {arguments.At(0)} not found.";
                return false;
            }

            if (arguments.At(1).ToLower() is "all" or "*")
            {
                GhostController controller = GetController(player);
                controller.IsFullyInvisible = !controller.IsFullyInvisible;
                response = $"{player.Nickname} is now {(controller.IsFullyInvisible ? "invisible" : "visible")} to everyone";
                return true;
            }

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

        public static GhostController GetController(AtPlayer player) => player.GameObject.TryGetComponent(out GhostController controller)
            ? controller
            : player.GameObject.AddComponent<GhostController>();

    }
}
