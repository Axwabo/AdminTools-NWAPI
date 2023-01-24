using CommandSystem;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Basic
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class SpawnRagdoll : ParentCommand, IDefaultPermissions
    {
        public SpawnRagdoll() => LoadGeneratedCommands();

        public override string Command => "spawnragdoll";

        public override string[] Aliases { get; } =
        {
            "ragdoll", "rd", "rag", "doll"
        };

        public override string Description => "Spawns a specified number of ragdolls on a user";

        public override void LoadGeneratedCommands() { }

        public PlayerPermissions Permissions => PlayerPermissions.RespawnEvents;

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!ValidateArguments(this, arguments, sender, out response, out RoleTypeId type, out int amount, out IRagdollRole ragdollRole))
                return false;

            switch (arguments.At(0).ToLower())
            {
                case "*" or "all":
                    foreach (Player p in Player.GetPlayers().Where(Extensions.IsAlive))
                    {
                        Timing.RunCoroutine(EventHandlers.SpawnBodies(p, ragdollRole, amount));
                    }

                    break;
                default:
                {
                    Player p = Extensions.GetPlayer(arguments.At(0));
                    if (p is null)
                    {
                        response = $"Player {arguments.At(0)} not found.";
                        return false;
                    }

                    Timing.RunCoroutine(EventHandlers.SpawnBodies(p, ragdollRole, amount));
                    break;
                }
            }

            response = $"{amount} {type} ragdoll(s) have been spawned on {arguments.At(0)}.";
            return true;
        }
        private static bool ValidateArguments(ICommand command, ArraySegment<string> arguments, ICommandSender sender, out string response, out RoleTypeId type, out int amount, out IRagdollRole ragdollRole)
        {
            type = RoleTypeId.None;
            amount = 0;
            ragdollRole = null;
            if (!sender.CheckPermission(command, out response))
                return false;

            if (arguments.Count < 3)
            {
                response = "Usage: spawnragdoll ((player id / name) or (all / *)) (RoleTypeId) [amount]";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out type))
            {
                response = $"Invalid RoleTypeId for ragdoll: {arguments.At(1)}";
                return false;
            }

            if (!PlayerRoleLoader.TryGetRoleTemplate(type, out PlayerRoleBase role) || role is not IRagdollRole ragdoll)
            {
                response = $"Cannot find ragdoll role for role type {type}";
                return false;
            }


            amount = 0;
            if (arguments.Count > 2 && !int.TryParse(arguments.At(2), out amount))
            {
                response = $"Invalid amount of ragdolls to spawn: {arguments.At(2)}";
                return false;
            }
            response = "";
            ragdollRole = ragdoll;
            return true;
        }
    }
}
