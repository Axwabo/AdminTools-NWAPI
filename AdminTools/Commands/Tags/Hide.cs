using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Tags
{
    public sealed class Hide : ICommand, IDefaultPermissions
    {
        public string Command => "hide";

        public string[] Aliases { get; } =
            { };

        public string Description => "Hides staff tags on the server";

        public PlayerPermissions Permissions => PlayerPermissions.SetGroup;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 0)
            {
                response = "Usage: tags hide";
                return false;
            }

            foreach (Player player in Player.GetPlayers().Where(p => p.ReferenceHub.serverRoles.RemoteAdmin && !p.IsBadgeHidden()))
                player.SetBadgeVisibility(true);

            response = "All staff tags are hidden now";
            return true;
        }
    }
}
