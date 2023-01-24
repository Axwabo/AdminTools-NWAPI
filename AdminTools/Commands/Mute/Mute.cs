using CommandSystem;
using System;

namespace AdminTools.Commands.Mute
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Mute : ParentCommand, IDefaultPermissions
    {
        public Mute() => LoadGeneratedCommands();

        public override string Command => "pmute";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Mutes everyone from speaking or by intercom in the server";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new All());
            RegisterCommand(new IntercomMute());
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
