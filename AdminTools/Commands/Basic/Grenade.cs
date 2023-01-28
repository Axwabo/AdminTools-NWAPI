using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Basic
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Grenade : ICommand, IDefaultPermissions
    {
        public string Command => "grenade";

        public string[] Aliases { get; } =
        {
            "gn"
        };

        public string Description => "Spawns a HE grenade/flashbang/SCP-018 on a user or users";

        public PlayerPermissions Permissions => PlayerPermissions.GivingItems;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!ValidateArguments(this, arguments, sender, out response, out ItemType type, out float fuseTime))
                return false;

            switch (arguments.At(0).ToLower())
            {
                case "*" or "all":
                    if (type == ItemType.SCP018)
                        Cassie.Message("pitch_1.5 xmas_bouncyballs", false, false);

                    foreach (Player p in Player.GetPlayers().Where(Extensions.IsAlive))
                        Handlers.CreateThrowable(type).SpawnActive(p.Position, fuseTime);

                    response = "Grenade has been sent to everyone";
                    return true;
                default:
                {
                    Player p = Extensions.GetPlayer(arguments.At(0));
                    if (p is null)
                    {
                        response = $"Player {arguments.At(0)} not found.";
                        return false;
                    }

                    Handlers.CreateThrowable(type).SpawnActive(p.Position, fuseTime);
                    response = $"Grenade has been sent to {p.Nickname}";
                    return true;
                }
            }
        }
        private static bool ValidateArguments(ICommand command, ArraySegment<string> arguments, ICommandSender sender, out string response, out ItemType type, out float fuseTime)
        {
            type = ItemType.None;
            fuseTime = -1f;
            if (!sender.CheckPermission(command, out response))
                return false;

            if (arguments.Count < 2)
            {
                response = "Usage: grenade ((player id / name) or (all / *)) (GrenadeType) [fuse time]";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out type) || type is not (ItemType.SCP018 or ItemType.GrenadeHE or ItemType.GrenadeFlash))
            {
                response = $"Invalid value for grenade type: {arguments.At(1)}, expected SCP018 or GrenadeHE or GrenadeFlash";
                return false;
            }

            if (arguments.Count > 2 && !float.TryParse(arguments.At(2), out fuseTime))
            {
                response = $"Invalid fuse time for grenade: {arguments.At(2)}! Set to negative to use default fuse time";
                return false;
            }
            response = "";
            return true;
        }
    }
}
