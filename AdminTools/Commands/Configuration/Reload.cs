using CommandSystem;
using GameCore;
using System;

namespace AdminTools.Commands.Configuration
{
    public sealed class Reload : ICommand
    {
        public string Command => "reload";

        public string[] Aliases { get; } =
        {
            "rld"
        };

        public string Description => "Reloads all permissions and configs";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.ServerConfigs))
            {
                response = "You don't have permission to run this command.";
                return false;
            }

            if (arguments.Count < 0)
            {
                response = "Usage: cfig reload";
                return false;
            }

            ServerStatic.PermissionsHandler.RefreshPermissions();
            ConfigFile.ReloadGameConfigs();
            response = "Configuration files reloaded!";
            return true;
        }
    }
}
