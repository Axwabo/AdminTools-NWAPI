using CommandSystem;
using NorthwoodLib.Pools;
using System;
using System.Linq;
using System.Text;
using Utils.NonAllocLINQ;

namespace AdminTools.Commands.Regeneration
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Regeneration : ParentCommand
    {
        public Regeneration() => LoadGeneratedCommands();

        public override string Command => "reg";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Manages regeneration properties for users";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count >= 1)
                return arguments.At(0).ToLower() switch
                {
                    "clear" => Clear(out response),
                    "list" => List(arguments, out response),
                    "heal" => Heal(arguments, out response),
                    "time" => Time(arguments, out response),
                    "*" or "all" => All(arguments, out response),
                    _ => HandleDefault(arguments, out response)
                };
            response = "Usage:\nreg ((player id / name) or (all / *)) ((doors) or (all))" +
                "\nreg clear" +
                "\nreg list" +
                "\nreg heal (value)" +
                "\nreg time (value)";
            return false;

        }
        private static bool Clear(out string response)
        {
            foreach (AtPlayer ply in Extensions.Players)
                ply.RegenerationEnabled = false;

            response = "Regeneration has been removed from everyone";
            return true;
        }
        private static bool List(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: regen list";
                return false;
            }
            AtPlayer[] list = Extensions.Players.Where(p => p.RegenerationEnabled).ToArray();
            StringBuilder playerLister = StringBuilderPool.Shared.Rent(list.Length != 0 ? "Players with regeneration on:\n" : "No players currently online have regeneration on");
            if (list.Length == 0)
            {
                response = playerLister.ToString();
                return true;
            }

            playerLister.Append(list.JoinNicknames());
            response = StringBuilderPool.Shared.ToStringReturn(playerLister);
            return true;
        }
        private static bool Heal(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: reg heal (value)";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float heal) || heal <= 0f)
            {
                response = $"Invalid value for healing: {arguments.At(1)}";
                return false;
            }

            AtPlayer.RegenerationAmount = heal;
            response = $"Players with regeneration will heal {heal} HP per interval";
            return true;
        }
        private static bool Time(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: reg time (value)";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float interval) || interval <= 0f)
            {
                response = $"Invalid value for healing time interval: {arguments.At(1)}";
                return false;
            }

            AtPlayer.RegenerationInterval = interval;
            response = $"Players with regeneration will heal every {interval} seconds";
            return true;
        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: reg (player id / name)";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(0));

            if (p == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            p.RegenerationEnabled = !p.RegenerationEnabled;
            response = p.RegenerationEnabled ? $"Regeneration is on for {p.Nickname}" : $"Regeneration is off for {p.Nickname}";
            return true;
        }
        private static bool All(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: reg (all / *)";
                return false;
            }

            ListExtensions.ForEach(Extensions.Players, p => p.RegenerationEnabled = true);
            response = "Everyone on the server can regenerate health now";
            return true;
        }
    }
}
