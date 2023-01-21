using CommandSystem;
using System;

namespace AdminTools.Commands.Tags
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Tags : ParentCommand
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

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.SetGroup))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            response = "Invalid subcommand. Available ones: hide, show";
            return false;
        }
    }
}
