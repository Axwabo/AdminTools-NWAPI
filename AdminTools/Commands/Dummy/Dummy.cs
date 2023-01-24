using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.NonAllocLINQ;
using Object = UnityEngine.Object;

namespace AdminTools.Commands.Dummy
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public sealed class Dummy : ParentCommand, IDefaultPermissions
    {
        public Dummy() => LoadGeneratedCommands();

        public override string Command => "dummy";

        public override string[] Aliases { get; } =
        {
            "dum"
        };

        public override string Description => "Spawns a dummy character on all users on a user";

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

            Player player = Player.Get(ps.ReferenceHub);
            if (arguments.Count >= 1)
                return arguments.At(0).ToLower() switch
                {
                    "clear" => Clear(arguments, out response),
                    "clearall" => ClearAll(out response),
                    "count" => Count(arguments, out response),
                    "*" or "all" => All(arguments, out response, player),
                    _ => HandleDefault(arguments, out response, player)
                };
            response = "Usage:\ndummy ((player id / name) or (all / *)) (RoleType) (x value) (y value) (z value)" +
                "\ndummy clear (player id / name) (minimum index) (maximum index)" +
                "\ndummy clearall" +
                "\ndummy count (player id / name) ";
            return false;

        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response, Player player)
        {
            if (arguments.Count < 5)
            {
                response = "Usage: dummy (player id / name) (RoleType) (x value) (y value) (z value)";
                return false;
            }

            Player p = Extensions.GetPlayer(arguments.At(0));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (!p.IsAlive)
            {
                response = "This player is not a valid class to spawn a dummy on";
                return false;
            }

            if (!TryParseArguments(arguments, out response, out Vector3 scale, out RoleTypeId role))
                return false;

            EventHandlers.SpawnDummyModel(player, p.Position, p.GameObject.transform.localRotation, role, scale, out int dummyIndex);
            response = $"A {role} dummy has spawned on Player {p.Nickname}, you now spawned in a total of {(dummyIndex != 1 ? $"{dummyIndex} dummies" : $"{dummyIndex} dummy")}";
            return true;
        }
        private static bool All(ArraySegment<string> arguments, out string response, Player player)
        {
            if (arguments.Count < 5)
            {
                response = "Usage: dummy (all / *) (RoleType) (x value) (y value) (z value)";
                return false;
            }

            if (!TryParseArguments(arguments, out response, out Vector3 scale, out RoleTypeId role))
                return false;
            int index = 0;
            foreach (Player p in Player.GetPlayers().Where(Extensions.IsAlive))
            {
                EventHandlers.SpawnDummyModel(player, p.Position, p.GameObject.transform.rotation, role, scale, out int dIndex);
                index = dIndex;
            }

            response = $"A {role} dummy has spawned on everyone, you now spawned in a total of {(index != 1 ? $"{index} dummies" : "1 dummy")}";
            return true;
        }
        private static bool Count(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: dummy count (player id / name)";
                return false;
            }

            Player p = Extensions.GetPlayer(arguments.At(1));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            if (!Plugin.DumHubs.TryGetValue(p, out List<GameObject> obj) || obj.Count == 0)
            {
                response = $"{p.Nickname} has not spawned in any dummies in";
                return false;
            }

            response = $"{p.Nickname} has spawned in {(obj.Count != 1 ? $"{obj.Count} dummies" : $"{obj.Count} dummy")}";
            return true;
        }
        private static bool ClearAll(out string response)
        {
            foreach (KeyValuePair<Player, List<GameObject>> dummy in Plugin.DumHubs)
            {
                ListExtensions.ForEach(dummy.Value, Object.Destroy);
                dummy.Value.Clear();
            }

            Plugin.DumHubs.Clear();
            response = "All spawned dummies have now been removed";
            return true;
        }
        private static bool Clear(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 4)
            {
                response = "Usage: dummy clear (player id / name) (minimum index) (maximum index)\nNote: Minimum < Maximum, you can remove from a range of dummies a user spawns";
                return false;
            }

            Player p = Extensions.GetPlayer(arguments.At(1));
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

            if (!Plugin.DumHubs.TryGetValue(p, out List<GameObject> objs))
            {
                response = $"{p.Nickname} has not spawned in any dummies in";
                return false;
            }

            if (min > objs.Count)
            {
                response = $"{min} (minimum) is higher than the number of dummies {p.Nickname} spawned! (Which is {objs.Count})";
                return false;
            }

            if (max > objs.Count)
            {
                response = $"{max} (maximum) is higher than the number of dummies {p.Nickname} spawned! (Which is {objs.Count})";
                return false;
            }

            min = min <= 0 ? 0 : min - 1;
            max = max <= 0 ? 0 : max - 1;
            for (int i = min; i <= max; i++)
            {
                Object.Destroy(objs.ElementAt(i));
                objs[i] = null;
            }
            objs.RemoveAll(r => r == null);

            response = $"All dummies from {min + 1} to {max + 1} have been cleared from Player {p.Nickname}";
            return true;
        }
        private static bool TryParseArguments(ArraySegment<string> arguments, out string response, out Vector3 scale, out RoleTypeId role)
        {
            if (!Enum.TryParse(arguments.At(1), true, out role))
            {
                response = $"Invalid value for role type: {arguments.At(1)}";
                scale = Vector3.zero;
                return false;
            }
            if (!float.TryParse(arguments.At(2), out float x))
            {
                response = $"Invalid x value for dummy size: {arguments.At(2)}";
                scale = Vector3.zero;
                return false;
            }

            if (!float.TryParse(arguments.At(3), out float y))
            {
                response = $"Invalid y value for dummy size: {arguments.At(3)}";
                scale = Vector3.zero;
                return false;
            }

            if (!float.TryParse(arguments.At(4), out float z))
            {
                response = $"Invalid z value for dummy size: {arguments.At(4)}";
                scale = Vector3.zero;
                return false;
            }

            scale = new Vector3(x, y, z);
            response = "";
            return true;
        }
    }
}
