using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Tags
{
    public sealed class Show : ICommand, IDefaultPermissions
    {
        public string Command => "show";

        public string[] Aliases { get; } =
            { };

        public string Description => "Shows staff tags on the server";

        public PlayerPermissions Permissions => PlayerPermissions.SetGroup;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 0)
            {
                response = "Usage: tags show";
                return false;
            }

            foreach (Player p in Player.GetPlayers().Where(HasHiddenBadge))
                p.SetBadgeVisibility(false);

            response = "All staff tags are now visible";
            return true;
        }
        private static bool HasHiddenBadge(Player player) => player.ReferenceHub.serverRoles.RemoteAdmin && !player.ReferenceHub.authManager.RemoteAdminGlobalAccess && player.IsBadgeHidden();
    }
}
