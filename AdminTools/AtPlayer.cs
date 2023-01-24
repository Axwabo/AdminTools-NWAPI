using PluginAPI.Core;
using PluginAPI.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools
{
    public sealed class AtPlayer : Player
    {
        public static float RegenerationAmount { get; set; }

        public static float RegenerationInterval { get; set; }

        public AtPlayer(IGameComponent component) : base(component) { }

        public bool PryGateEnabled { get; set; }

        public bool BreakDoorsEnabled { get; set; }

        public bool InstantKillEnabled { get; set; }

        public bool RegenerationEnabled { get; set; }

        public readonly List<GameObject> Workbenches = new();

        private float _regenTime;

        public override void OnUpdate()
        {
            if (!RegenerationEnabled)
                return;
            _regenTime += Time.deltaTime;
            if (_regenTime < RegenerationInterval)
                return;
            _regenTime = 0;
            Health = Mathf.Min(MaxHealth, Health + RegenerationAmount);
        }
    }
}
