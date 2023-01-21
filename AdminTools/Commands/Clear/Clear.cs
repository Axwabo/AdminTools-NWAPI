using CommandSystem;
using System;

namespace AdminTools.Commands.Clear
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Clear : ParentCommand
    {
        public Clear() => LoadGeneratedCommands();

        public override string Command => "clear";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Clears up items and ragdolls from the server";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Items());
            RegisterCommand(new Ragdolls());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.FacilityManagement))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            response = "Invalid subcommand. Available ones: items, ragdolls";
            return false;
        }
    }
}
