﻿using CommandSystem;
using System;

namespace AdminTools.Commands.Configuration
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Configuration : ParentCommand
    {
        public Configuration() => LoadGeneratedCommands();

        public override string Command => "cfig";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Manages reloading permissions and configs";

        public override void LoadGeneratedCommands() => RegisterCommand(new Reload());

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommmand. Available ones: reload";
            return false;
        }
    }
}
