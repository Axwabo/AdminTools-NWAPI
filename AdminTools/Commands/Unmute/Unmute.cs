using CommandSystem;
using System;

namespace AdminTools.Commands.Unmute
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Unmute : ParentCommand, IDefaultPermissions
    {
        public Unmute() => LoadGeneratedCommands();

        public override string Command => "punmute";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Unmutes everyone from speaking or by intercom in the server";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new All());
            RegisterCommand(new IntercomUnmute());
            RegisterCommand(new RoundStart());
        }

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            response = "Invalid subcommand. Available ones: icom, all, roundstart";
            return false;
        }
    }
}
