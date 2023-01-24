using CommandSystem;
using System;

namespace AdminTools.Commands.Tags
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Tags : ParentCommand, IDefaultPermissions
    {
        public Tags() => LoadGeneratedCommands();

        public override string Command => "tags";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Hides or shows staff tags in the server";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Hide());
            RegisterCommand(new Show());
        }

        public PlayerPermissions Permissions => PlayerPermissions.SetGroup;

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            response = "Invalid subcommand. Available ones: hide, show";
            return false;
        }
    }
}
