using AdminTools.Enums;
using PlayerRoles;
using RelativePositioning;
using System.Collections.Generic;

namespace AdminTools
{

    public sealed class Jailed
    {
        public required string UserId { get; init; }
        public List<ItemType> Items;
        public RoleTypeId Role;
        public RelativePosition Position;
        public float Health;
        public Dictionary<AmmoType, ushort> Ammo;
        public bool CurrentRound;
        public bool GodMode;
    }
}
