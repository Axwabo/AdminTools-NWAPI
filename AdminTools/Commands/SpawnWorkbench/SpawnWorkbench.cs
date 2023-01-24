using CommandSystem;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.NonAllocLINQ;
using Object = UnityEngine.Object;

namespace AdminTools.Commands.SpawnWorkbench
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public sealed class SpawnWorkbench : ParentCommand, IDefaultPermissions
    {
        public SpawnWorkbench() => LoadGeneratedCommands();

        public override string Command => "bench";

        public override string[] Aliases { get; } =
        {
            "sw", "wb", "workbench"
        };

        public override string Description => "Spawns a workbench on all users or a user";

        public override void LoadGeneratedCommands() { }

        public PlayerPermissions Permissions => PlayerPermissions.RespawnEvents;

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (sender is not PlayerCommandSender ps)
            {
                response = "You must be in-game to run this command!";
                return false;
            }

            AtPlayer player = Player.Get<AtPlayer>(ps.ReferenceHub);

            if (arguments.Count >= 1)
                return arguments.At(0).ToLower() switch
                {
                    "clear" => Clear(arguments, out response),
                    "clearall" => ClearAll(out response),
                    "count" => Count(arguments, out response),
                    "*" or "all" => All(arguments, out response, player),
                    _ => HandleDefault(arguments, out response, player)
                };
            response = "Usage: bench ((player id / name) or (all / *)) (x value) (y value) (z value)" +
                "\nbench clear (player id / name) (minimum index) (maximum index)" +
                "\nbench clearall" +
                "\nbench count (player id / name)";
            return false;

        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response, AtPlayer player)
        {
            if (arguments.Count < 4)
            {
                response = "Usage: bench (player id / name) (x value) (y value) (z value)";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(0));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }
            if (!p.IsAlive)
            {
                response = "This player is not a valid class to spawn a workbench on";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float x))
            {
                response = $"Invalid value for x size: {arguments.At(1)}";
                return false;
            }

            if (!float.TryParse(arguments.At(2), out float y))
            {
                response = $"Invalid value for y size: {arguments.At(2)}";
                return false;
            }

            if (!float.TryParse(arguments.At(3), out float z))
            {
                response = $"Invalid value for z size: {arguments.At(3)}";
                return false;
            }

            GameObject bench = EventHandlers.SpawnWorkbench(player, new Vector3(x, y, z), out int index);
            if (bench == null)
            {
                response = "Failed to spawn workbench! Check server logs";
                return false;
            }
            response = $"A workbench has spawned on Player {p.Nickname}, you now spawned in a total of {(index != 1 ? $"{index} workbenches" : "1 workbench")}";
            return true;
        }
        private static bool All(ArraySegment<string> arguments, out string response, AtPlayer player)
        {
            if (arguments.Count < 4)
            {
                response = "Usage: bench (all / *) (x value) (y value) (z value)";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float x))
            {
                response = $"Invalid value for x size: {arguments.At(1)}";
                return false;
            }

            if (!float.TryParse(arguments.At(2), out float y))
            {
                response = $"Invalid value for y size: {arguments.At(2)}";
                return false;
            }

            if (!float.TryParse(arguments.At(3), out float z))
            {
                response = $"Invalid value for z size: {arguments.At(3)}";
                return false;
            }

            int index = -1;
            foreach (Player p in Player.GetPlayers().Where(Extensions.IsAlive))
            {
                EventHandlers.SpawnWorkbench(player, EventHandlers.CalculateBenchPosition(p), p.GameObject.transform.rotation, new Vector3(x, y, z), out int benchIndex);
                index = benchIndex;
            }

            if (index == -1)
            {
                response = "Failed to spawn workbench! Check server logs";
                return false;
            }

            response = $"A workbench has spawned on everyone, you now spawned in a total of {(index != 1 ? $"{index} workbenches" : $"{index} workbench")}";
            return true;
        }
        private static bool Count(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: bench count (player id / name)";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(1));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            List<GameObject> list = p.Workbenches;
            if (list.Count == 0)
            {
                response = $"{p.Nickname} has not spawned in any workbenches";
                return false;
            }

            response = $"{p.Nickname} has spawned in {(list.Count != 1 ? $"{list.Count} workbenches" : "1 workbench")}";
            return true;
        }
        private static bool ClearAll(out string response)
        {
            foreach (AtPlayer p in Extensions.Players)
            {
                ListExtensions.ForEach(p.Workbenches, Object.Destroy);
                p.Workbenches.Clear();
            }

            response = "All spawned workbenches have now been removed";
            return true;
        }
        private static bool Clear(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 4)
            {
                response = "Usage:\nbench clear (player id / name) (minimum index) (maximum index)\n\nNOTE: Minimum index < Maximum index, You can remove from a range of all the benches you spawned (From 1 to (how many you spawned))";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(1));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            if (!int.TryParse(arguments.At(2), out int min))
            {
                response = $"Invalid value for minimum index: {arguments.At(2)}";
                return false;
            }

            if (!int.TryParse(arguments.At(3), out int max))
            {
                response = $"Invalid value for maximum index: {arguments.At(3)}";
                return false;
            }

            if (max < min)
            {
                response = $"{max} is not greater than {min}";
                return false;
            }

            List<GameObject> list = p.Workbenches;
            if (list.Count == 0)
            {
                response = $"{p.Nickname} has not spawned in any workbenches";
                return false;
            }

            if (min > list.Count)
            {
                response = $"{min} (minimum) is higher than the number of workbenches {p.Nickname} spawned! (Which is {list.Count})";
                return false;
            }

            if (max > list.Count)
            {
                response = $"{max} (maximum) is higher than the number of workbenches {p.Nickname} spawned! (Which is {list.Count})";
                return false;
            }

            min = min <= 0 ? 0 : min - 1;
            max = max <= 0 ? 0 : max - 1;

            for (int i = min; i <= max; i++)
            {
                Object.Destroy(list.ElementAt(i));
                list[i] = null;
            }
            list.RemoveAll(r => r == null);

            response = $"All workbenches from {min + 1} to {max + 1} have been cleared from Player {p.Nickname}";
            return true;
        }
    }
}
