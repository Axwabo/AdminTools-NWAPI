using CommandSystem;
using PluginAPI.Core;
using System;
using VoiceChat;

namespace AdminTools.Commands.Unmute
{
    public sealed class IntercomUnmute : ICommand, IDefaultPermissions
    {
        public string Command => "icom";

        public string[] Aliases { get; } =
            { };

        public string Description => "Removes intercom mutes everyone in the server";

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;
            foreach (Player ply in Player.GetPlayers())
                ply.SetMuteFlag(VcMuteFlags.LocalIntercom, false);
            response = "Everyone from the server who is not a staff can speak in the intercom now";
            return true;
        }
    }
}
