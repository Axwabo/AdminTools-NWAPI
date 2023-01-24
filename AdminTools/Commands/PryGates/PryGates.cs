using CommandSystem;
using NorthwoodLib.Pools;
using System;
using System.Linq;
using System.Text;
using Utils.NonAllocLINQ;

namespace AdminTools.Commands.PryGates
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class PryGates : ParentCommand
    {
        public PryGates() => LoadGeneratedCommands();

        public override string Command => "prygate";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Gives the ability to pry gates to players, clear the ability from players, and shows who has the ability";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.FacilityManagement))
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

            response = "Usage:\nprygate ((player id / name) or (all / *))" +
                "\nprygate clear" +
                "\nprygate list" +
                "\nprygate remove (player id / name)";
            return false;

        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: prygate (player id / name)";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(0));
            if (p == null)
            {
                response = $"Player \"{arguments.At(0)}\" not found";
                return false;
            }

            if (!p.PryGateEnabled)
            {
                p.PryGateEnabled = true;
                response = $"Player \"{p.Nickname}\" can now pry gates open";
                return true;
            }

            p.PryGateEnabled = false;
            response = $"Player \"{p.Nickname}\" cannot pry gates open now";
            return true;
        }
        private static bool All(out string response)
        {
            ListExtensions.ForEach(Extensions.Players, p => p.PryGateEnabled = true);
            response = "The ability to pry gates open is on for all players now";
            return true;
        }
        private static bool Remove(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: prygate remove (player id / name)";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(1));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            if (p.PryGateEnabled)
            {
                p.PryGateEnabled = false;
                response = $"Player \"{p.Nickname}\" cannot pry gates open now";
            }
            else
                response = $"Player {p.Nickname} does not have the ability to pry gates open";
            return true;
        }
        private static bool List(out string response)
        {
            AtPlayer[] active = Extensions.Players.Where(p => p.PryGateEnabled).ToArray();

            StringBuilder playerLister = StringBuilderPool.Shared.Rent(active.Length != 0 ? "Players with the ability to pry gates:\n" : "No players currently online have the ability to pry gates");
            if (active.Length > 0)
            {
                foreach (AtPlayer p in active)
                    playerLister.Append(p.Nickname + ", ");

                int length = playerLister.ToString().Length;
                response = playerLister.ToString().Substring(0, length - 2);
                StringBuilderPool.Shared.Return(playerLister);
                return true;
            }
            response = playerLister.ToString();
            StringBuilderPool.Shared.Return(playerLister);
            return true;
        }
        private static bool Clear(out string response)
        {
            ListExtensions.ForEach(Extensions.Players, p => p.PryGateEnabled = false);
            response = "The ability to pry gates is cleared from all players now";
            return true;
        }
    }
}
