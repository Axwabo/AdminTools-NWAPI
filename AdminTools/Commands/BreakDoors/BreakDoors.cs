using CommandSystem;
using NorthwoodLib.Pools;
using System;
using System.Linq;
using System.Text;

namespace AdminTools.Commands.BreakDoors
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class BreakDoors : ParentCommand
    {
        public BreakDoors() => LoadGeneratedCommands();

        public override string Command => "breakdoors";

        public override string[] Aliases { get; } =
        {
            "bd"
        };

        public override string Description => "Manage break door properties for users";

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
                    "clear" => HandleClear(arguments, out response),
                    "list" => HandleList(out response),
                    "remove" => HandleRemove(arguments, out response),
                    "*" => HandleAll(arguments, out response),
                    "all" => HandleAll(arguments, out response),
                    _ => HandleDefault(arguments, out response)
                };

            response = "Usage:\nbreakdoors ((player id / name) or (all / *))" +
                "\nbreakdoors clear" +
                "\nbreakdoors list" +
                "\nbreakdoors remove (player id / name)";
            return false;

        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: breakdoors (player id / name)";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(0));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            p.BreakDoorsEnabled = !p.BreakDoorsEnabled;
            response = $"Break doors is now {(p.BreakDoorsEnabled ? "on" : "off")} for {p.Nickname}";
            return true;
        }
        private static bool HandleAll(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: breakdoors all / *";
                return false;
            }

            foreach (AtPlayer ply in Extensions.Players.Where(ply => !ply.BreakDoorsEnabled))
                ply.BreakDoorsEnabled = true;

            response = "Everyone on the server can instantly kill other users now";
            return true;
        }
        private static bool HandleRemove(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: breakdoors remove (player id / name)";
                return false;
            }

            AtPlayer p = Extensions.GetPlayer(arguments.At(1));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            if (p.BreakDoorsEnabled)
            {
                p.BreakDoorsEnabled = false;
                response = $"Break doors turned off for {p.Nickname}";
            }
            else
                response = $"Player {p.Nickname} does not have the ability to break doors";
            return true;
        }
        private static bool HandleList(out string response)
        {
            AtPlayer[] list = Extensions.Players.Where(p => p.BreakDoorsEnabled).ToArray();
            StringBuilder playerLister = StringBuilderPool.Shared.Rent(list.Length != 0 ? "Players with break doors on:\n" : "No players currently online have break doors on");
            if (list.Length == 0)
            {
                response = playerLister.ToString();
                return true;
            }

            playerLister.Append(list.JoinNicknames());
            response = StringBuilderPool.Shared.ToStringReturn(playerLister);
            return true;
        }
        private static bool HandleClear(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: breakdoors clear";
                return false;
            }

            foreach (AtPlayer ply in Extensions.Players)
                ply.BreakDoorsEnabled = false;

            response = "Door breaking has been removed from everyone";
            return true;
        }

    }
}
