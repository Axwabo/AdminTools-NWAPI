using CommandSystem;
using System;

namespace AdminTools.Commands.Unmute
{
    public sealed class RoundStart : ICommand
    {
        public string Command => "roundstart";

        public string[] Aliases { get; } =
        {
            "rs"
        };

        public string Description => "Unmutes everyone from speaking until the round starts.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            EventHandlers.ClearRoundStartMutes();
            response = "All non-staff players that were muted until round start have been unmuted.";
            return true;
        }
    }
}
