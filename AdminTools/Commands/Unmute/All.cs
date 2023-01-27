using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;
using VoiceChat;

namespace AdminTools.Commands.Unmute
{
    public sealed class All : ICommand, IDefaultPermissions
    {
        public string Command => "all";

        public string[] Aliases { get; } =
        {
            "*"
        };

        public string Description => "Removes all mutes from everyone in the server";

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            foreach (Player p in Player.GetPlayers().Where(p => !p.ReferenceHub.serverRoles.RemoteAdmin))
                p.SetMuteFlag(VcMuteFlags.LocalRegular | VcMuteFlags.LocalIntercom, false);

            response = "Everyone from the server who is not a staff can now speak freely";
            return true;
        }
    }
}
