using CommandSystem;

namespace AdminTools.Commands
{
    public static class PermissionManager
    {
        private static Config _cfg;

        internal static void InitFromConfig(Config cfg) => _cfg = cfg;

        public static bool CheckPermission(this ICommandSender sender, ICommand command, out string response)
        {
            CommandSender s = (CommandSender) sender;
            if (s.FullPermissions)
            {
                response = "";
                return true;
            }

            if (_cfg.PermissionOverrides.TryGetValue(command.Command, out PlayerPermissions perm))
            {
                if (s.CheckPermission(perm))
                {
                    response = "";
                    return true;
                }

                response = $"You do not have permission to use this command. Required: {perm}";
                return false;
            }
            if (command is IDefaultPermissions def)
            {
                if (s.CheckPermission(def.Permissions))
                {
                    response = "";
                    return true;
                }

                response = $"You do not have permission to use this command. Required: {def.Permissions}";
                return false;
            }
            response = "";
            return false;
        }
    }
}
