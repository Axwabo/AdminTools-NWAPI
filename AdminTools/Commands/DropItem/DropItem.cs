using CommandSystem;
using Footprinting;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Mirror;
using PluginAPI.Core;
using System;
using UnityEngine;

namespace AdminTools.Commands.DropItem
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class DropItem : ParentCommand, IDefaultPermissions
    {
        public DropItem() => LoadGeneratedCommands();

        public override string Command => "dropitem";

        public override string[] Aliases { get; } =
        {
            "drop", "dropi"
        };

        public override string Description => "Drops a specified amount of a specified item on either all users or a user";

        public override void LoadGeneratedCommands() { }

        public PlayerPermissions Permissions => PlayerPermissions.GivingItems;

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count >= 3)
                return arguments.At(0).ToLower() switch
                {
                    "*" or "all" => All(arguments, out response),
                    _ => HandleDefault(arguments, out response)
                };
            response = "Usage: dropitem ((player id/ name) or (all / *)) (ItemType) (amount (200 max for one user, 15 max for all users))";
            return false;

        }
        private static bool HandleDefault(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 3)
            {
                response = "Usage: dropitem (player id / name) (ItemType) (amount (200 max))";
                return false;
            }

            Player p = Extensions.GetPlayer(arguments.At(0));
            if (p == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out ItemType type))
            {
                response = $"Invalid value for item type: {arguments.At(1)}";
                return false;
            }

            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase item))
            {
                response = $"Could not find item with type: {type}";
                return false;
            }

            if (!ushort.TryParse(arguments.At(2), out ushort amount) || amount > 200)
            {
                response = $"Invalid amount of item to drop: {arguments.At(2)}";
                return false;
            }

            Vector3 pos = p.Position;
            for (int i = 0; i < amount; i++)
                NetworkServer.Spawn(CreatePickup(pos, item).gameObject);
            response = $"{amount} of {type.ToString()} was spawned on {p.Nickname} (\"Hehexd\" - Galaxy119)";
            return true;
        }
        private static bool All(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 3)
            {
                response = "Usage: dropitem (all / *) (ItemType) (amount (15 max))";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out ItemType type))
            {
                response = $"Invalid value for item type: {arguments.At(1)}";
                return false;
            }

            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase item))
            {
                response = $"Could not find item with type: {type}";
                return false;
            }

            if (!ushort.TryParse(arguments.At(2), out ushort amount) || amount > 15)
            {
                response = $"Invalid amount of item to drop: {arguments.At(2)} {(amount > 15 ? "(\"Try a lower number that won't crash my servers, ty.\" - Galaxy119)" : "")}";
                return false;
            }

            foreach (Player p in Player.GetPlayers())
            {
                Vector3 pos = p.Position;
                for (int i = 0; i < amount; i++)
                    NetworkServer.Spawn(CreatePickup(pos, item).gameObject);
            }

            response = $"{amount} of {type.ToString()} was spawned on everyone (\"Hehexd\" - Galaxy119)";
            return true;
        }

        public static ItemPickupBase CreatePickup(Vector3 position, ItemBase prefab)
        {
            ItemPickupBase clone = UnityEngine.Object.Instantiate(prefab.PickupDropModel, position, Quaternion.identity);
            clone.NetworkInfo = new PickupSyncInfo(prefab.ItemTypeId, position, Quaternion.identity, prefab.Weight);
            clone.PreviousOwner = new Footprint(ReferenceHub.HostHub);
            return clone;
        }
    }
}
