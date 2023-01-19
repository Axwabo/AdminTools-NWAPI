using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdminTools
{
    public static class Extensions
    {
        public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.Public;
            MethodInfo info = type.GetMethod(methodName, flags);
            info?.Invoke(null, param);
        }

        public static List<AtPlayer> Players => Player.GetPlayers<AtPlayer>();
        
        public static AtPlayer GetPlayer(string arg) => int.TryParse(arg, out int id) ? Players.FirstOrDefault(x => x.PlayerId == id) : Player.GetByName<AtPlayer>(arg);
        
        public static bool IsAlive(this Player p) => p.Role is not (RoleTypeId.Spectator or RoleTypeId.None);
    }
}
