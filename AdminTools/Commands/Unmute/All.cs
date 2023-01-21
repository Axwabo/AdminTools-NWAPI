using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Unmute
{
    public sealed class All : ICommand
    {
        public string Command => "all";

        public string[] Aliases { get; } =
        {
            "*"
        };

        public string Description => "Removes all mutes from everyone in the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 0)
            {
                response = "Usage: punmute all";
                return false;
            }

            foreach (Player ply in Player.GetPlayers().Where(ply => !ply.ReferenceHub.serverRoles.RemoteAdmin))
            {
                ply.IntercomUnmute(true);
                ply.Unmute(true);
            }

            response = "Everyone from the server who is not a staff can now speak freely";
            return true;
        }
    }
}
