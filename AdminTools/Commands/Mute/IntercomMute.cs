﻿using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;
using VoiceChat;

namespace AdminTools.Commands.Mute
{
    public sealed class IntercomMute : ICommand, IDefaultPermissions
    {
        public string Command => "icom";

        public string[] Aliases { get; } =
            { };

        public string Description => "Intercom mutes everyone in the server";

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 0)
            {
                response = "Usage: pmute icom";
                return false;
            }

            foreach (Player player in Player.GetPlayers().Where(IsIntercomMuteApplicable))
                player.SetMuteFlag(VcMuteFlags.LocalIntercom, true);

            response = "Everyone from the server who is not a staff has been intercom muted";
            return true;
        }
        private static bool IsIntercomMuteApplicable(Player player) => !player.ReferenceHub.serverRoles.RemoteAdmin && !player.IsIntercomMuted;
    }
}
