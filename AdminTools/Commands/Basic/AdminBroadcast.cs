using CommandSystem;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Linq;

namespace AdminTools.Commands.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class AdminBroadcast : ParentCommand
    {
        public AdminBroadcast() => LoadGeneratedCommands();

        public override string Command => "adminbroadcast";

        public override string[] Aliases { get; } =
        {
            "abc"
        };

        public override string Description => "Sends a message to all currently online staff on the server";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions((CommandSender) sender, "broadcast", PlayerPermissions.Broadcasting, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: adminbroadcast (time) (message)";
                return false;
            }

            if (!ushort.TryParse(arguments.At(0), out ushort t))
            {
                response = $"Invalid value for broadcast time: {arguments.At(0)}";
                return false;
            }

            foreach (Player pl in Player.GetPlayers().Where(HasAdminChatAccess))
                pl.SendBroadcast(EventHandlers.FormatArguments(arguments, 1) + $" ~{((CommandSender) sender).Nickname}",
                    t, Broadcast.BroadcastFlags.AdminChat);

            response = "Message sent to all currently online staff";
            return true;
        }
        private static bool HasAdminChatAccess(Player pl) => pl.ReferenceHub.serverRoles != null && pl.ReferenceHub.serverRoles.RemoteAdmin;
    }
}
