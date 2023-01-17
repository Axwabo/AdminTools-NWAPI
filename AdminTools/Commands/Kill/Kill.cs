﻿using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Kill
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Kill : ParentCommand
    {
        public Kill() => LoadGeneratedCommands();

        public override string Command => "atkill";

        public override string[] Aliases { get; } = { };

        public override string Description => "Kills everyone or a user instantly";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission(PlayerPermissions.ForceclassToSpectator))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: kill ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    foreach (Player ply in Player.GetPlayers().Where(ply => ply.Role != RoleTypeId.Spectator && ply.Role != RoleTypeId.None))
                    {
                        ply.Kill("Killed by admin.");
                    }

                    response = "Everyone has been game ended (killed) now";
                    return true;
                default:
                    Player pl = int.TryParse(arguments.At(0), out int id) ? Player.GetPlayers().FirstOrDefault(x => x.PlayerId == id) : Player.GetByName(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (pl.Role is RoleTypeId.Spectator or RoleTypeId.None)
                    {
                        response = $"Player {pl.Nickname} is not a valid class to kill";
                        return false;
                    }

                    pl.Kill("Killed by admin.");
                    response = $"Player {pl.Nickname} has been game ended (killed) now";
                    return true;
            }
        }
    }
}
