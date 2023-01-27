using CommandSystem;
using System;

namespace AdminTools.Commands.Unmute
{
    public sealed class RoundStart : ICommand, IDefaultPermissions
    {
        public string Command => "roundstart";

        public string[] Aliases { get; } =
        {
            "rs"
        };

        public string Description => "Unmutes everyone from speaking until the round starts.";

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;
            EventHandlers.ClearRoundStartMutes();
            response = "All non-staff players that were muted until round start have been unmuted.";
            return true;
        }
    }
}
