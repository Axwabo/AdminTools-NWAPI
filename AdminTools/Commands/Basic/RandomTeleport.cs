using CommandSystem;
using MapGeneration;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using System;
using UnityEngine;

namespace AdminTools.Commands.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class RandomTeleport : ICommand, IDefaultPermissions
    {
        public string Command => "randomtp";

        public string[] Aliases { get; } =
            { };

        public string Description => "Randomly teleports a user or all users to a random room in the facility";

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 1)
            {
                response = "Usage: randomtp ((player id / name) or (all / *))";
                return false;
            }
            switch (arguments.At(0).ToLower())
            {
                case "*" or "all":
                    foreach (Player p in Player.GetPlayers())
                    {
                        Teleport(p);
                    }

                    response = "Everyone was teleported to a random room in the facility";
                    return true;
                default:
                {
                    Player p = Extensions.GetPlayer(arguments.At(0));
                    if (p == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    RoomIdentifier room = Teleport(p);
                    response = $"Player {p.Nickname} was teleported to {room.gameObject.name}";
                    return true;
                }
            }
        }
        private static RoomIdentifier Teleport(Player p)
        {
            RoomIdentifier room = UnityEngine.Object.FindObjectsOfType<RoomIdentifier>().RandomItem();
            if (p.ReferenceHub.roleManager.CurrentRole is IFpcRole fpc)
                fpc.FpcModule.Motor.ResetFallDamageCooldown();
            p.Position = room.TryGetComponent(out SafeTeleportPosition safe) && safe.SafePositions is { Length: not 0 } pos
                ? pos[0].position + Vector3.up
                : room.transform.position + Vector3.up;
            return room;
        }
    }
}
