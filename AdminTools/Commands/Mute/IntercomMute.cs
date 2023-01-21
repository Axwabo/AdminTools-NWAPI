using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Mute
{
    public sealed class IntercomMute : ICommand
    {
        public string Command => "icom";

        public string[] Aliases { get; } =
            { };

        public string Description => "Intercom mutes everyone in the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 0)
            {
                response = "Usage: pmute icom";
                return false;
            }

            foreach (Player player in Player.GetPlayers().Where(player => !player.ReferenceHub.serverRoles.RemoteAdmin && player.IsIntercomMuted))
                player.IntercomUnmute(true);

            response = "Everyone from the server who is not a staff has been intercom muted";
            return true;
        }
    }
}
