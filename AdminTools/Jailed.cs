using AdminTools.Enums;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools
{

    public sealed class Jailed
    {
        public required string UserId { get; init; }
        public List<ItemType> Items;
        public RoleTypeId Role;
        public Vector3 Position;
        public float Health;
        public Dictionary<AmmoType, ushort> Ammo;
        public bool CurrentRound;
        public bool GodMode;

        public int Scp079Exp;
    }
}
