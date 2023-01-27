using LiteNetLib4Mirror.Open.Nat;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VoiceChat;

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

        public static AtPlayer GetPlayer(string arg) => int.TryParse(arg, out int id) ? Player.Get<AtPlayer>(id) : Players.FirstOrDefault(p => p.Nickname.ContainsIgnoreCase(arg));

        public static bool IsAlive(this Player p) => p.IsAlive;

        public static string JoinNicknames(this IEnumerable<Player> players) => string.Join(", ", players.Select(x => x.Nickname));

        public static void SetMuteFlag(this Player player, VcMuteFlags flag, bool state)
        {
            if (player == null)
                return;
            ReferenceHub hub = player.ReferenceHub;
            VcMuteFlags current = VoiceChatMutes.GetFlags(hub);
            VoiceChatMutes.SetFlags(hub, state ? current | flag : current & ~flag);
        }

    }
}
