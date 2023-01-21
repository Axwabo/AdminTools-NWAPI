using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Mute
{
    public sealed class All : ICommand
    {
        public string Command => "all";

        public string[] Aliases { get; } =
        {
            "*"
        };

        public string Description => "Mutes everyone from speaking at all in the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 0)
            {
                response = "Usage: pmute all";
                return false;
            }

            foreach (Player player in Player.GetPlayers().Where(player => !player.ReferenceHub.serverRoles.RemoteAdmin && player.IsMuted))
                player.Unmute(true);

            response = "Everyone from the server who is not a staff has been muted completely";
            return true;
        }
    }
}
