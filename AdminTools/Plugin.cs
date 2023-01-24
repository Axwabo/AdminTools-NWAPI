using AdminTools.Commands;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AdminTools
{
    public sealed class Plugin
    {
        public const string Author = "Galaxy119, Neil, Axwabo";
        public const string Name = "Admin Tools";
        public const string Version = "2.0.0";

        public EventHandlers EventHandlers;

        public static readonly List<Jailed> JailedPlayers = new();
        public static readonly Dictionary<Player, List<GameObject>> DumHubs = new();
        public string OverwatchFilePath;
        public string HiddenTagsFilePath;
        public static readonly HashSet<string> RoundStartMutes = new();

        [PluginConfig] public Config Config = new();

        [PluginEntryPoint(Name, Version, "Tools to better support staff", Author)]
        public void Start()
        {
            PermissionManager.InitFromConfig(Config);
            FactoryManager.RegisterPlayerFactory<AdminToolsPlayerFactory>(this);
            foreach (KeyValuePair<byte, DeathTranslation> translation in DeathTranslations.TranslationsById)
                Handlers.UniversalDamageTypeIDs.Add(translation.Value, translation.Key);

            string path = PluginHandler.Get(this).PluginDirectoryPath;
            string overwatchFileName = Path.Combine(path, "AdminTools-Overwatch.txt");
            string hiddenTagFileName = Path.Combine(path, "AdminTools-HiddenTags.txt");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!File.Exists(overwatchFileName))
                File.Create(overwatchFileName).Close();

            if (!File.Exists(hiddenTagFileName))
                File.Create(hiddenTagFileName).Close();

            OverwatchFilePath = overwatchFileName;
            HiddenTagsFilePath = hiddenTagFileName;

            EventHandlers = new EventHandlers(this);
            EventManager.RegisterEvents(this, EventHandlers);
            Log.Info("AdminTools has been enabled!");
        }

        [PluginUnload]
        public void Stop() => EventManager.UnregisterEvents(this, EventHandlers);

    }
}
