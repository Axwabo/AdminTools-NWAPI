using System.Collections.Generic;
using UnityEngine;

namespace AdminTools.Commands.Ghost
{
    [DisallowMultipleComponent]
    public sealed class GhostController : MonoBehaviour
    {
        public bool IsFullyInvisible { get; set; }

        public HashSet<string> InvisibleTo { get; } = new();
    }
}
