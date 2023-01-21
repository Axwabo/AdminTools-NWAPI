using CommandSystem;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using Utils.NonAllocLINQ;

namespace AdminTools.Commands.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Strip : ParentCommand
    {
        public Strip() => LoadGeneratedCommands();

        public override string Command => "atstrip";

        public override string[] Aliases { get; } =
        {
            "stp"
        };

        public override string Description => "Clears a user or users inventories instantly";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions((CommandSender) sender, "strip", PlayerPermissions.PlayersManagement, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

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
