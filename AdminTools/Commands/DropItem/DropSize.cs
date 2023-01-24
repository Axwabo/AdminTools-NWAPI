using CommandSystem;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Mirror;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools.Commands.DropItem
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class DropSize : ParentCommand, IDefaultPermissions
    {
        public DropSize() => LoadGeneratedCommands();

        public override string Command => "dropsize";

        public override string[] Aliases { get; } =
        {
            "drops"
        };

        public override string Description => "Drops a selected amount of a selected item on a user or all users";

        public override void LoadGeneratedCommands() { }

        public PlayerPermissions Permissions => PlayerPermissions.GivingItems;

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(this, out response))
                return false;

            if (arguments.Count < 3)
            {
                response = "Usage:\ndropsize ((player id / name) or (all / *)) (ItemType) (size)\ndropsize ((player id / name) or (all / *)) (ItemType) (x size) (y size) (z size)";
                return false;
            }
            List<Player> players;
            if (arguments.At(0).ToLower() is "*" or "all")
                players = Player.GetPlayers();
            else
            {
                AtPlayer p = Extensions.GetPlayer(arguments.At(0));
                if (p == null)
                {
                    response = $"Player not found: {arguments.At(0)}";
                    return false;
                }
                players = new List<Player>
                {
                    p
                };
            }
            if (arguments.Count < 3)
            {
                response = "Usage: dropsize (all / * id) (ItemType) ((size) or (x size) (y size) (z size))";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out ItemType type))
            {
                response = $"Invalid value for item name: {arguments.At(1)}";
                return false;
            }

            if (InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase item))
                return ParseSize(players, item, arguments, out response);
            response = $"Could not find item with type: {type}";
            return false;

        }
        private static bool ParseSize(List<Player> players, ItemBase itemBase, ArraySegment<string> arguments, out string response)
        {
            switch (arguments.Count)
            {
                case 3:
                    if (float.TryParse(arguments.At(2), out float size))
                        return SpawnItem(players, itemBase, Vector3.one * size, out response);
                    response = $"Invalid value for item scale: {arguments.At(2)}";
                    return false;
                case >= 5:
                    if (!float.TryParse(arguments.At(2), out float x))
                    {
                        response = $"Invalid value for item scale: {arguments.At(2)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(3), out float y))
                    {
                        response = $"Invalid value for item scale: {arguments.At(3)}";
                        return false;
                    }

                    if (float.TryParse(arguments.At(4), out float z))
                        return SpawnItem(players, itemBase, new Vector3(x, y, z), out response);
                    response = $"Invalid value for item scale: {arguments.At(4)}";
                    return false;
                default:
                    response = "\nUsage:\ndrops (all / *) (ItemType) (size) \ndrops (all / *) (ItemType) (x size) (y size) (z size)";
                    return false;
            }
        }
        private static bool SpawnItem(List<Player> players, ItemBase itemBase, Vector3 size, out string response)
        {
            foreach (Player p in players)
            {
                ItemPickupBase item = DropItem.CreatePickup(p.Position, itemBase);
                item.transform.localScale = size;
                NetworkServer.Spawn(item.gameObject);
            }
            response = players.Count == 1
                ? $"Spawned in a {itemBase.ItemTypeId} that is a size of {size} at {players[0].Nickname}'s position (\"Yay! Items with sizes!\" - Galaxy119)"
                : $"Spawned in a {itemBase.ItemTypeId} that is {size} at every player's position (\"Yay! Items with sizes!\" - Galaxy119)";
            return true;

        }
    }
}
