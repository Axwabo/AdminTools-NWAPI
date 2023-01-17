﻿using CommandSystem;
using PlayerStatsSystem;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands.Ahp
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Ahp : ParentCommand
    {
        public Ahp() => LoadGeneratedCommands();

        public override string Command => "ahp";

        public override string[] Aliases { get; } = { };

        public override string Description => "Sets a user or users Artificial HP to a specified value";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions((CommandSender)sender, "ahp", PlayerPermissions.PlayersManagement, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: ahp ((player id / name) or (all / *)) (value)";
                return false;
            }

            List<Player> players = new();
            if (!float.TryParse(arguments.At(1), out float value))
            {
                response = $"Invalid value for AHP: {value}";
                return false;
            }
            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    players.AddRange(Player.GetPlayers());
                    break;
                default:
                    Player player = int.TryParse(arguments.At(0), out int id) ? Player.GetPlayers().FirstOrDefault(x => x.PlayerId == id) : Player.GetByName(arguments.At(0));
                    if (player == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    players.Add(player);
                    break;
            }
            response = "";
            foreach (Player p in players)
            {
                AhpStat stat = p.GetStatModule<AhpStat>();
                List<AhpStat.AhpProcess> processes = stat._activeProcesses;
                if (processes.Count < 1)
                    stat.ServerAddProcess(value);
                else
                    processes[0].CurrentAmount = value;
                response += $"\n{p.Nickname}'s AHP has been set to {value}";
            }

            return true;
        }
    }
}
