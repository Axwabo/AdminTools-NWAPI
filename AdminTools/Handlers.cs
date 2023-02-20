using AdminTools.Enums;
using Footprinting;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using PluginAPI.Core;
using RemoteAdmin;
using Scp914;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AdminTools
{
    public static class Handlers
    {
        private const int RecontainmentDamageTypeID = 27;
        private const int WarheadDamageTypeID = 2;
        private const int MicroHidTypeID = 18;
        private const int GrenadeTypeID = 19;
        private const int Scp018TypeID = 38;
        private const int DisruptorTypeID = 46;

        private static readonly Dictionary<ItemType, int> FirearmDamageTypeIDs = new()
        {
            {
                ItemType.GunCOM15, 12
            },
            {
                ItemType.GunE11SR, 14
            },
            {
                ItemType.GunLogicer, 16
            },
            {
                ItemType.GunCOM18, 32
            },
            {
                ItemType.GunAK, 33
            },
            {
                ItemType.GunShotgun, 34
            },
            {
                ItemType.GunCrossvec, 35
            },
            {
                ItemType.GunFSP9, 36
            },
            {
                ItemType.MicroHID, 18
            },
            {
                ItemType.GunRevolver, 31
            },
            {
                ItemType.ParticleDisruptor, 45
            },
            {
                ItemType.GunCom45, 50
            }
        };

        public static readonly Dictionary<DeathTranslation, int> UniversalDamageTypeIDs = new();

        private static readonly Dictionary<RoleTypeId, int> RoleDamageTypeIDs = new()
        {
            {
                RoleTypeId.Scp173, 24
            },
            {
                RoleTypeId.Scp106, 23
            },
            {
                RoleTypeId.Scp049, 20
            },
            {
                RoleTypeId.Scp096, 22
            },
            {
                RoleTypeId.Scp0492, 21
            },
            {
                RoleTypeId.Scp939, 25
            }
        };

        private static readonly Dictionary<Scp096DamageHandler.AttackType, int> Scp096DamageTypeIDs = new()
        {
            {
                Scp096DamageHandler.AttackType.GateKill, 40
            },
            {
                Scp096DamageHandler.AttackType.SlapLeft, 22
            },
            {
                Scp096DamageHandler.AttackType.SlapRight, 22
            },
            {
                Scp096DamageHandler.AttackType.Charge, 39
            }
        };

        private static readonly Dictionary<Scp939DamageType, int> Scp939DamageTypeIDs = new()
        {
            {
                Scp939DamageType.None, 46
            },
            {
                Scp939DamageType.Claw, 47
            },
            {
                Scp939DamageType.LungeTarget, 48
            },
            {
                Scp939DamageType.LungeSecondary, 49
            }
        };

        public static bool IsWeapon(this ItemBase item) =>
            item.ItemTypeId switch
            {
                ItemType.GunCrossvec or ItemType.GunLogicer or ItemType.GunRevolver or ItemType.GunShotgun or ItemType.GunAK
                    or ItemType.GunCOM15 or ItemType.GunCOM18 or ItemType.GunE11SR
                    or ItemType.GunFSP9 or ItemType.ParticleDisruptor or ItemType.MicroHID => true,
                _ => false
            };

        public static int ToId(this DamageHandlerBase damageHandler)
        {
            switch (damageHandler)
            {
                case RecontainmentDamageHandler:
                    return RecontainmentDamageTypeID;
                case MicroHidDamageHandler:
                    return MicroHidTypeID;
                case ExplosionDamageHandler:
                    return GrenadeTypeID;
                case WarheadDamageHandler:
                    return WarheadDamageTypeID;
                case Scp018DamageHandler:
                    return Scp018TypeID;
                case DisruptorDamageHandler:
                    return DisruptorTypeID;
                case FirearmDamageHandler firearmDamageHandler:
                {
                    ItemType id = firearmDamageHandler.WeaponType;

                    return FirearmDamageTypeIDs.TryGetValue(id, out int output) ? output : -1;
                }
                case UniversalDamageHandler universalDamageHandler:
                {
                    byte id = universalDamageHandler.TranslationId;
                    if (!DeathTranslations.TranslationsById.TryGetValue(id, out DeathTranslation translation)) return -1;

                    return UniversalDamageTypeIDs.TryGetValue(translation, out int output) ? output : -1;
                }
                case ScpDamageHandler scpDamageHandler:
                {
                    RoleTypeId id = scpDamageHandler.Attacker.Role;

                    return RoleDamageTypeIDs.TryGetValue(id, out int output) ? output : -1;
                }
                case Scp096DamageHandler scp096DamageHandler:
                {
                    Scp096DamageHandler.AttackType id = scp096DamageHandler._attackType;

                    return Scp096DamageTypeIDs.TryGetValue(id, out int output) ? output : -1;
                }
                case Scp939DamageHandler scp939DamageHandler:
                {
                    Scp939DamageType id = scp939DamageHandler._damageType;

                    return Scp939DamageTypeIDs.TryGetValue(id, out int output) ? output : -1;
                }
                default:
                    return -1;
            }
        }

        public static float Amount(this DamageHandlerBase damageHandler) => damageHandler switch
        {
            RecontainmentDamageHandler handler => handler.Damage,
            MicroHidDamageHandler handler => handler.Damage,
            ExplosionDamageHandler handler => handler.Damage,
            WarheadDamageHandler handler => handler.Damage,
            Scp018DamageHandler handler => handler.Damage,
            DisruptorDamageHandler handler => handler.Damage,
            FirearmDamageHandler firearmDamageHandler => firearmDamageHandler.Damage,
            UniversalDamageHandler universalDamageHandler => universalDamageHandler.Damage,
            ScpDamageHandler scpDamageHandler => scpDamageHandler.Damage,
            Scp096DamageHandler scp096DamageHandler => scp096DamageHandler.Damage,
            Scp939DamageHandler scp939DamageHandler => scp939DamageHandler.Damage,
            _ => -1
        };

        public static void SetAmount(this DamageHandlerBase damageHandler, float amount)
        {
            switch (damageHandler)
            {
                case RecontainmentDamageHandler handler:
                    handler.Damage = amount;
                    return;
                case MicroHidDamageHandler handler:
                    handler.Damage = amount;
                    return;
                case ExplosionDamageHandler handler:
                    handler.Damage = amount;
                    return;
                case WarheadDamageHandler handler:
                    handler.Damage = amount;
                    return;
                case Scp018DamageHandler handler:
                    handler.Damage = amount;
                    return;
                case DisruptorDamageHandler handler:
                    handler.Damage = amount;
                    return;
                case FirearmDamageHandler firearmDamageHandler:
                    firearmDamageHandler.Damage = amount;
                    return;
                case UniversalDamageHandler universalDamageHandler:
                    universalDamageHandler.Damage = amount;
                    return;
                case ScpDamageHandler scpDamageHandler:
                    scpDamageHandler.Damage = amount;
                    return;
                case Scp096DamageHandler scp096DamageHandler:
                    scp096DamageHandler.Damage = amount;
                    return;
                case Scp939DamageHandler scp939DamageHandler:
                    scp939DamageHandler.Damage = amount;
                    break;
            }
        }

        public static string DamageType(this int id) => id switch
        {
            RecontainmentDamageTypeID => "Recontainment",
            WarheadDamageTypeID => "Warhead",
            MicroHidTypeID => "Micro",
            GrenadeTypeID => "Grenade",
            Scp018TypeID => "Balls",
            DisruptorTypeID => "Disruptor",
            _ => FirearmDamageTypeIDs.ContainsValue(id)
                ? FirearmDamageTypeIDs.First(p => p.Value == id).Key.ToString()
                : UniversalDamageTypeIDs.ContainsValue(id)
                    ? UniversalDamageTypeIDs.First(p => p.Value == id).Key.LogLabel
                    : RoleDamageTypeIDs.ContainsValue(id)
                        ? RoleDamageTypeIDs.First(p => p.Value == id).Key.ToString()
                        : Scp096DamageTypeIDs.ContainsValue(id)
                            ? Scp096DamageTypeIDs.First(p => p.Value == id).Key.ToString()
                            : Scp939DamageTypeIDs.ContainsValue(id)
                                ? Scp939DamageTypeIDs.First(p => p.Value == id).Key.ToString()
                                : "Unknown"
        };

        public static Team Team(this Player player) => player.ReferenceHub.GetTeam();

        public static Side Side(this Player player) => player.Team() switch
        {
            PlayerRoles.Team.SCPs => Enums.Side.Scp,
            PlayerRoles.Team.FoundationForces or PlayerRoles.Team.Scientists => Enums.Side.Mtf,
            PlayerRoles.Team.ChaosInsurgency or PlayerRoles.Team.ClassD => Enums.Side.ChaosInsurgency,
            PlayerRoles.Team.OtherAlive => Enums.Side.Tutorial,
            _ => Enums.Side.None
        };

        public static void RemoteAdminMessage(this Player player, string message, bool success = true, string pluginName = null) =>
            player.ReferenceHub.queryProcessor._sender.RaReply((pluginName ?? Assembly.GetCallingAssembly().GetName().Name) + "#" + message, success, true, string.Empty);

        public static UserGroup GetGroup(this Player player) => player.ReferenceHub.serverRoles.Group;

        public static void SetGroup(this Player player, UserGroup group) =>
            player.ReferenceHub.serverRoles.SetGroup(group, false);

        public static string GetRankName(this Player player) => player.ReferenceHub.serverRoles.Network_myText;
        public static void SetRankName(this Player player, string name) => player.ReferenceHub.serverRoles.SetText(name);

        public static string GetRankColor(this Player player) => player.ReferenceHub.serverRoles.Network_myColor;

        public static void SetRankColor(this Player player, string color) =>
            player.ReferenceHub.serverRoles.SetColor(color);

        public static bool IsBadgeHidden(this Player player) =>
            !string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge);

        public static void SetBadgeVisibility(this Player player, bool hidden)
        {
            if (hidden)
                player.ReferenceHub.characterClassManager.UserCode_CmdRequestHideTag();
            else
                player.ReferenceHub.characterClassManager.UserCode_CmdRequestShowTag(false);

        }

        public static Scp914KnobSetting GetKnobSetting() => Scp914Controller.Singleton.Network_knobSetting;

        public static Scp914KnobSetting SetKnobSetting(Scp914KnobSetting state) =>
            Scp914Controller.Singleton.Network_knobSetting = state;

        public static PlayerCommandSender GetSender(this Player player) =>
            player.ReferenceHub.queryProcessor._sender;

        public static double GetServerTps() => Math.Round(1f / Time.smoothDeltaTime);

        public static bool TryPryOpen(this DoorVariant door, Player player) => door is PryableDoor gate && gate.TryPryGate(player.ReferenceHub);

        public static void SpawnActive(this ThrowableItem item, Vector3 position, float fuseTime = -1f,
            Player owner = null)
        {
            TimeGrenade grenade = (TimeGrenade) Object.Instantiate(item.Projectile, position, Quaternion.identity);
            if (fuseTime >= 0)
                grenade._fuseTime = fuseTime;
            grenade.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, position, Quaternion.identity, item.Weight, item.ItemSerial);
            grenade.PreviousOwner = new Footprint(owner != null ? owner.ReferenceHub : ReferenceHub.HostHub);
            if (grenade is Scp018Projectile scp018)
                scp018.RigidBody.velocity = new Vector3(Random.value, Random.value, Random.value); // add some force to make the ball bounce
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
        }

        public static Dictionary<AmmoType, ushort> Ammo(this Player player) => Enum.GetValues(typeof(ItemType))
            .Cast<ItemType>().Where(x => x.IsAmmo()).ToDictionary(itemType => itemType.GetAmmoType(), player.GetAmmo);

        public static bool IsAmmo(this ItemType item) =>
            item is ItemType.Ammo9x19 or ItemType.Ammo12gauge or ItemType.Ammo44cal or ItemType.Ammo556x45 or ItemType.Ammo762x39;

        public static AmmoType GetAmmoType(this ItemType item) => item switch
        {
            ItemType.Ammo9x19 => AmmoType.Nato9,
            ItemType.Ammo12gauge => AmmoType.Ammo12Gauge,
            ItemType.Ammo44cal => AmmoType.Ammo44Cal,
            ItemType.Ammo556x45 => AmmoType.Nato556,
            ItemType.Ammo762x39 => AmmoType.Nato762,
            _ => AmmoType.None
        };

        public static ItemType GetItemType(this AmmoType item) => item switch
        {
            AmmoType.Nato9 => ItemType.Ammo9x19,
            AmmoType.Ammo12Gauge => ItemType.Ammo12gauge,
            AmmoType.Ammo44Cal => ItemType.Ammo44cal,
            AmmoType.Nato556 => ItemType.Ammo556x45,
            AmmoType.Nato762 => ItemType.Ammo762x39,
            _ => ItemType.None
        };

        public static void RemoveItem(this Player player, ItemBase item, bool destroy = true)
        {
            Inventory inventory = player.ReferenceHub.inventory;

            if (destroy)
            {
                inventory.ServerRemoveItem(item.ItemSerial, null);
            }
            else
            {
                if (player.CurrentItem is not null && player.CurrentItem.ItemSerial == item.ItemSerial)
                    inventory.NetworkCurItem = ItemIdentifier.None;

                inventory.UserInventory.Items.Remove(item.ItemSerial);
                inventory.SendItemsNextFrame = true;
            }
        }

        public static void ClearInventory(this Player player)
        {
            Inventory inv = player.ReferenceHub.inventory;
            InventoryInfo ui = inv.UserInventory;
            ItemType[] ammoTypes = ui.ReserveAmmo.Keys.ToArray();
            foreach (ItemType type in ammoTypes)
            {
                ui.ReserveAmmo[type] = 0;
            }
            ui.Items.Clear();
            inv.NetworkCurItem = ItemIdentifier.None;
            inv.SendItemsNextFrame = true;
            inv.SendAmmoNextFrame = true;
        }

        public static ItemBase AddItem(this Player player, ItemType itemType)
        {
            ItemBase item = player.ReferenceHub.inventory.ServerAddItem(itemType);
            if (item is not Firearm firearm)
                return item;

            if (AttachmentsServerHandler.PlayerPreferences[player.ReferenceHub].TryGetValue(itemType, out uint attachments))
            {
                firearm.ApplyAttachmentsCode(attachments, true);
            }

            FirearmStatusFlags flags = FirearmStatusFlags.MagazineInserted;
            if (firearm.Attachments.Any(a => a.Name == AttachmentName.Flashlight))
                flags |= FirearmStatusFlags.FlashlightEnabled;
            firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, flags, firearm.GetCurrentAttachmentsCode());
            return item;
        }

        public static void ResetInventory(this Player player, List<ItemType> items)
        {
            player.ClearInventory();
            if (items is not { Count: not 0 })
                return;
            foreach (ItemType type in items)
            {
                player.AddItem(type);
            }
        }

        public static bool BreakDoor(this DoorVariant door, DoorDamageType type = DoorDamageType.ServerCommand)
        {
            if (door is not IDamageableDoor { IsDestroyed: false } dmg)
                return false;

            dmg.ServerDamage(float.MaxValue, type);
            return true;

        }

        public static ThrowableItem CreateThrowable(ItemType type, Player player = null) => (player != null ? player.ReferenceHub : ReferenceHub.HostHub)
            .inventory.CreateItemInstance(new ItemIdentifier(type, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;

        public static ReadOnlyCollection<ItemPickupBase> GetPickups() => Object.FindObjectsOfType<ItemPickupBase>().ToList().AsReadOnly();
    }
}
