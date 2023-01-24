using CommandSystem;
using NorthwoodLib.Pools;
using System;
using System.Linq;
using System.Text;
using Utils.NonAllocLINQ;

namespace AdminTools.Commands.InstantKill
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class InstantKill : ParentCommand
    {
        public InstantKill() => LoadGeneratedCommands();

        public override string Command => "instakill";

        public override string[] Aliases { get; } =
        {
            "ik"
        };

        public override string Description => "Manage instant kill properties for users";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.ForceclassWithoutRestrictions))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count >= 1)
                return arguments.At(0).ToLower() switch
                {
                    "clear" => Clear(out response),
                    "list" => List(out response),
                    "remove" => Remove(arguments, out response),
                    "*" or "all" => All(out response),
                    _ => HandleDefault(arguments, out response)
                };
            response = "Usage:\ninstakill ((player id / name) or (all / *))" +
                "\ninstakill clear" +
                "\ninstakill list" +
                "\ninstakill remove (player id / name)";
            return false;

        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: instakill (player id / name)";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(0));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            p.InstantKillEnabled = !p.InstantKillEnabled;
            response = $"Instant killing is now {(p.InstantKillEnabled ? "on" : "off")} for {p.Nickname}";
            return true;
        }
        private static bool All(out string response)
        {
            ListExtensions.ForEach(Extensions.Players, p => p.InstantKillEnabled = true);
            response = "Everyone on the server can instantly kill other users now";
            return true;
        }
        private static bool Remove(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: instakill remove (player id / name)";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(1));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            if (p.InstantKillEnabled)
            {
                p.InstantKillEnabled = false;
                response = $"Instant killing turned off for {p.Nickname}";
            }
            else
                response = $"Player {p.Nickname} does not have the ability to instantly kill others";
            return true;
        }
        private static bool List(out string response)
        {
            AtPlayer[] list = Extensions.Players.Where(p => p.InstantKillEnabled).ToArray();
            StringBuilder playerLister = StringBuilderPool.Shared.Rent(list.Length != 0 ? "Players with instant killing on:\n" : "No players currently online have instant killing on");
            if (list.Length == 0)
            {
                response = playerLister.ToString();
                return true;
            }

            playerLister.Append(list.JoinNicknames());
            response = StringBuilderPool.Shared.ToStringReturn(playerLister);
            return true;
        }
        private static bool Clear(out string response)
        {
            ListExtensions.ForEach(Extensions.Players, p => p.InstantKillEnabled = false);
            response = "Instant killing has been removed from everyone";
            return true;
        }

    }
}
