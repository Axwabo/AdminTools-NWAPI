using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Tags
{
    public sealed class Show : ICommand
    {
        public string Command => "show";

        public string[] Aliases { get; } =
            { };

        public string Description => "Shows staff tags on the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.SetGroup))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 0)
            {
                response = "Usage: tags show";
                return false;
            }

            foreach (Player player in Player.GetPlayers().Where(player => player.ReferenceHub.serverRoles.RemoteAdmin && !player.ReferenceHub.serverRoles.RaEverywhere && player.IsBadgeHidden()))
                player.SetBadgeVisibility(false);

            response = "All staff tags are now visible";
            return true;
        }
    }
}
