using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Tags
{
    public sealed class Hide : ICommand
    {
        public string Command => "hide";

        public string[] Aliases { get; } =
            { };

        public string Description => "Hides staff tags on the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.SetGroup))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 0)
            {
                response = "Usage: tags hide";
                return false;
            }

            foreach (Player player in Player.GetPlayers().Where(player => player.ReferenceHub.serverRoles.RemoteAdmin && !player.IsBadgeHidden()))
                player.SetBadgeVisibility(true);

            response = "All staff tags are hidden now";
            return true;
        }
    }
}
