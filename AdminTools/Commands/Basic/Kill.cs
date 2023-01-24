using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;

namespace AdminTools.Commands.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Kill : ParentCommand
    {
        public Kill() => LoadGeneratedCommands();

        public override string Command => "atkill";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Kills everyone or a user instantly";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.ForceclassToSpectator))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: kill ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "*" or "all":
                {
                    foreach (Player p in Player.GetPlayers().Where(Extensions.IsAlive))
                    {
                        p.Kill("Killed by admin.");
                    }

                    response = "Everyone has been game ended (killed) now";
                    return true;
                }
                default:
                {
                    Player p = Extensions.GetPlayer(arguments.At(0));
                    if (p == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!p.IsAlive)
                    {
                        response = $"Player {p.Nickname} is not a valid class to kill";
                        return false;
                    }

                    p.Kill("Killed by admin.");
                    response = $"Player {p.Nickname} has been game ended (killed) now";
                    return true;
                }
            }
        }
    }
}
