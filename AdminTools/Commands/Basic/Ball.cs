using CommandSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands.Basic
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Ball : ParentCommand, IDefaultPermissions
    {
        public Ball() => LoadGeneratedCommands();

        public override string Command => "ball";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Spawns a bouncy ball (SCP-018) on a user or all users";

        public override void LoadGeneratedCommands() { }

        public PlayerPermissions Permissions => PlayerPermissions.GivingItems;

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 1)
            {
                response = "Usage: ball ((player id / name) or (all / *))";
                return false;
            }

            List<Player> players = new();
            if (!AddPlayers(arguments, out response, players))
                return false;

            response = players.Count == 1
                ? $"{players[0].Nickname} has received a bouncing ball!"
                : $"The balls are bouncing for {players.Count} players!";
            if (players.Count > 1)
                Cassie.Message("pitch_1.5 xmas_bouncyballs", false, false);

            foreach (Player p in players)
                Handlers.CreateThrowable(ItemType.SCP018).SpawnActive(p.Position, owner: p);
            return true;
        }
        private static bool AddPlayers(ArraySegment<string> arguments, out string response, List<Player> players)
        {
            switch (arguments.At(0).ToLower())
            {
                case "*" or "all":
                    players.AddRange(Player.GetPlayers().Where(Extensions.IsAlive));
                    break;
                default:
                    Player p = Extensions.GetPlayer(arguments.At(0));
                    if (p == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!p.IsAlive())
                    {
                        response = "You cannot spawn a ball on that player right now";
                        return false;
                    }

                    players.Add(p);
                    break;
            }
            response = "";
            return true;
        }
    }
}
