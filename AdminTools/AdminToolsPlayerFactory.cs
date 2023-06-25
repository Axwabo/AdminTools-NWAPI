using PluginAPI.Core;
using PluginAPI.Core.Factories;
using PluginAPI.Core.Interfaces;
using System;

namespace AdminTools
{
    public sealed class AdminToolsPlayerFactory : PlayerFactory
    {
        public override Player Create(IGameComponent component) => new AtPlayer(component);

        public override Type BaseType => typeof(AtPlayer);
    }
}
