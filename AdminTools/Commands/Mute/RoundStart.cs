using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;
using VoiceChat;

namespace AdminTools.Commands.Mute
{
    public sealed class RoundStart : ICommand, IDefaultPermissions
    {
        public string Command => "roundstart";

        public string[] Aliases { get; } =
        {
            "rs"
        };

        public string Description => "Mutes everyone from speaking until the round starts.";

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;
            if (Round.IsRoundStarted)
            {
                response = "You cannot use this command after the round has started!";
                return false;
            }

            foreach (Player player in Player.GetPlayers().Where(MuteApplicable))
            {
                player.SetMuteFlag(VcMuteFlags.LocalRegular, true);
                Plugin.RoundStartMutes.Add(player.UserId);
            }

            response = "All non-staff players have been muted until the round starts.";
            return true;
        }
        private static bool MuteApplicable(Player player) => !player.ReferenceHub.serverRoles.RemoteAdmin && !player.IsMuted;
    }
}
