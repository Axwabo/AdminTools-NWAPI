using CommandSystem;
using MapGeneration;
using MEC;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands.Basic
{
    public sealed class Jail : ParentCommand, IDefaultPermissions
    {
        public Jail() => LoadGeneratedCommands();

        public override string Command => "jail";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Jails or unjails a user";

        public override void LoadGeneratedCommands() { }

        public PlayerPermissions Permissions => PlayerPermissions.KickingAndShortTermBanning;

        internal static Plugin Plugin;

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 1)
            {
                response = "Usage: jail (player id / name) [jail index]";
                return false;
            }

            Player p = Extensions.GetPlayer(arguments.At(0));

            if (p == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (Plugin.JailedPlayers.Any(j => j.UserId == p.UserId))
            {
                try
                {
                    Timing.RunCoroutine(EventHandlers.DoUnJail(p));
                    response = $"Player {p.Nickname} has been unjailed now";
                }
                catch (Exception e)
                {
                    Log.Error($"{e}");
                    response = "Command failed. Check server log.";
                    return false;
                }
            }
            else
            {
                int result = JailWithIndex(p, arguments, out response);
                if (result > 0)
                    return result == 1;
                Timing.RunCoroutine(EventHandlers.DoJail(p));
                response = $"Player {p.Nickname} has been jailed now";
            }
            return true;
        }
        private static int JailWithIndex(Player player, ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "";
                return 0;
            }

            if (!int.TryParse(arguments.At(1), out int index))
            {
                response = "Invalid jail index";
                return 2;
            }
            if (index <= 0)
            {
                response = "";
                return 0;
            }

            List<SerializableVector> positions = Plugin.Config.CustomJailPositions;
            if (index > positions.Count)
            {
                response = $"Jail index must be at most {positions.Count}";
                return 2;
            }

            RoomIdentifier outside = RoomIdentifier.AllRoomIdentifiers.FirstOrDefault(e => e.Name == RoomName.Outside);
            if (outside == null)
            {
                response = "Failed to find position";
                return 2;
            }

            Timing.RunCoroutine(EventHandlers.DoJail(player, outside.transform.root.TransformPoint(positions[index - 1])));
            response = $"Player {player.Nickname} has been jailed now";
            return 1;
        }
    }
}
