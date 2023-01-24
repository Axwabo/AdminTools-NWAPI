using CommandSystem;
using PluginAPI.Core;
using System;

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

            if (arguments.Count < 0)
            {
                response = "Usage: punmute icom";
                return false;
            }

            foreach (Player ply in Player.GetPlayers())
                ply.IntercomUnmute(true);

            response = "Everyone from the server who is not a staff can speak in the intercom now";
            return true;
        }
    }
}
