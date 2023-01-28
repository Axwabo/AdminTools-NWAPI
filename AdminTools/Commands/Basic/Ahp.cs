using CommandSystem;
using PlayerStatsSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;

namespace AdminTools.Commands.Basic
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Ahp : ICommand, IDefaultPermissions
    {
        public string Command => "ahp";

        public string[] Aliases { get; } =
            { };

        public string Description => "Sets a user or users Artificial HP to a specified value";

        public PlayerPermissions Permissions => PlayerPermissions.PlayersManagement;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

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
            if (!AddPlayers(arguments, out response, players))
                return false;
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
        private static bool AddPlayers(ArraySegment<string> arguments, out string response, List<Player> players)
        {
            switch (arguments.At(0).ToLower())
            {
                case "*" or "all":
                    players.AddRange(Player.GetPlayers());
                    break;
                default:
                    Player p = Extensions.GetPlayer(arguments.At(0));
                    if (p == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    players.Add(p);
                    break;
            }
            response = "";
            return true;
        }
    }
}
