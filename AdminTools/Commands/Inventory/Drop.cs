using CommandSystem;
using PluginAPI.Core;
using System;

namespace AdminTools.Commands.Inventory
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Drop : ParentCommand, IDefaultPermissions
    {
        public Drop() => LoadGeneratedCommands();

        public override string Command => "drop";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Drops the items in a players inventory";

        public override void LoadGeneratedCommands() { }

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 1)
            {
                response = "Usage: inventory drop ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "*" or "all":
                {
                    foreach (Player p in Player.GetPlayers())
                        p.DropEverything();

                    response = "All items from everyones inventories has been dropped";
                    return true;
                }
                default:
                {
                    Player p = Extensions.GetPlayer(arguments.At(0));
                    if (p == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    p.DropEverything();
                    response = $"All items from {p.Nickname}'s inventory has been dropped";
                    return true;
                }
            }
        }
    }
}
