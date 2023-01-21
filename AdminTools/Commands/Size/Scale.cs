using CommandSystem;
using PluginAPI.Core;
using System;

namespace AdminTools.Commands.Size
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Scale : ParentCommand
    {
        public Scale() => LoadGeneratedCommands();

        public override string Command => "scale";

        public override string[] Aliases { get; } =
            { };

        public override string Description => "Scales all users or a user by a specified value";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.Effects))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count >= 1)
                return arguments.At(0) switch
                {
                    "reset" => Reset(arguments, out response),
                    "*" or "all" => All(arguments, out response),
                    _ => HandleDefault(arguments, out response)
                };
            response = "Usage:\nscale ((player id / name) or (all / *)) (value)" +
                "\nscale reset";
            return false;

        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: scale (player id / name) (value)";
                return false;
            }

            Player p = Extensions.GetPlayer(arguments.At(0));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return true;
            }

            if (!float.TryParse(arguments.At(1), out float val))
            {
                response = $"Invalid value for scale: {arguments.At(1)}";
                return false;
            }

            EventHandlers.SetPlayerScale(p, val);
            response = $"Player {p.Nickname}'s scale has been set to {val}";
            return true;
        }
        private static bool All(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: scale (all / *) (value)";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float value))
            {
                response = $"Invalid value for scale: {arguments.At(1)}";
                return false;
            }

            foreach (Player p in Player.GetPlayers())
                EventHandlers.SetPlayerScale(p, value);

            response = $"Everyone's scale has been set to {value}";
            return true;
        }
        private static bool Reset(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage: scale reset";
                return false;
            }
            foreach (Player p in Player.GetPlayers())
                EventHandlers.SetPlayerScale(p, 1);

            response = "Everyone's scale has been reset";
            return true;
        }
    }
}
