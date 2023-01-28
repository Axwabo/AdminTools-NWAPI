using CommandSystem;
using PluginAPI.Core;
using System;
using Utils.NonAllocLINQ;

namespace AdminTools.Commands.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Strip : ICommand, IDefaultPermissions
    {
        public string Command => "atstrip";

        public string[] Aliases { get; } =
        {
            "stp"
        };

        public string Description => "Clears a user or users inventories instantly";

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 1)
            {
                response = "Usage: strip ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "*" or "all":
                    ListExtensions.ForEach(Player.GetPlayers(), Handlers.ClearInventory);
                    response = "Everyone's inventories have been cleared now";
                    return true;
                default:
                {
                    Player p = Extensions.GetPlayer(arguments.At(0));
                    if (p == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    p.ClearInventory();
                    response = $"Player {p.Nickname}'s inventory have been cleared now";
                    return true;
                }
            }
        }
    }
}
