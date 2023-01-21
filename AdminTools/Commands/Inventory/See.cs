using CommandSystem;
using InventorySystem.Items;
using NorthwoodLib.Pools;
using PluginAPI.Core;
using System;
using System.Linq;
using System.Text;

namespace AdminTools.Commands.Inventory
{

    public sealed class See : ICommand
    {
        public string Command => "see";

        public string[] Aliases { get; } =
            { };

        public string Description => "Sees the inventory items a user has";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: inventory see (player id / name)";
                return false;
            }

            Player ply = int.TryParse(arguments.At(0), out int id) ? Player.GetPlayers().FirstOrDefault(x => x.PlayerId == id) : Player.GetByName(arguments.At(0));
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            StringBuilder invBuilder = StringBuilderPool.Shared.Rent();
            if (ply.ReferenceHub.inventory.UserInventory.Items.Count != 0)
            {
                invBuilder.Append("Player ");
                invBuilder.Append(ply.Nickname);
                invBuilder.AppendLine(" has the following items in their inventory:");
                foreach (ItemBase item in ply.ReferenceHub.inventory.UserInventory.Items.Select(x => x.Value))
                {
                    invBuilder.Append("- ");
                    invBuilder.AppendLine(item.ItemTypeId.ToString());
                }
            }
            else
            {
                invBuilder.Append("Player ");
                invBuilder.Append(ply.Nickname);
                invBuilder.Append(" does not have any items in their inventory");
            }
            string msg = invBuilder.ToString();
            StringBuilderPool.Shared.Return(invBuilder);
            response = msg;
            return true;
        }
    }
}
