using CommandSystem;
using System;

namespace AdminTools.Commands.DropItem
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class DropItem : ParentCommand
    {
        public DropItem() => LoadGeneratedCommands();

        public override string Command => "dropitem";

        public override string[] Aliases { get; } =
        {
            "drop", "dropi"
        };

        public override string Description => "Drops a specified amount of a specified item on either all users or a user";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "This command is not currently available.";
            return false;

            // if (!((CommandSender)sender).CheckPermission(PlayerPermissions.GivingItems))
            // {
            //     response = "You do not have permission to use this command";
            //     return false;
            // }
            //
            // if (arguments.Count < 3)
            // {
            //     response = "Usage: dropitem ((player id/ name) or (all / *)) (ItemType) (amount (200 max for one user, 15 max for all users))";
            //     return false;
            // }
            //
            // switch (arguments.At(0))
            // {
            //     case "*":
            //     case "all":
            //         if (arguments.Count < 3)
            //         {
            //             response = "Usage: dropitem (all / *) (ItemType) (amount (15 max))";
            //             return false;
            //         }
            //
            //         if (!Enum.TryParse(arguments.At(1), true, out ItemType item))
            //         {
            //             response = $"Invalid value for item type: {arguments.At(1)}";
            //             return false;
            //         }
            //
            //         if (!uint.TryParse(arguments.At(2), out uint amount) || amount > 15)
            //         {
            //             response = $"Invalid amount of item to drop: {arguments.At(2)} {(amount > 15 ? "(\"Try a lower number that won't crash my servers, ty.\" - Galaxy119)" : "")}";
            //             return false;
            //         }
            //
            //         foreach (Player ply in Player.GetPlayers())
            //             for (int i = 0; i < amount; i++)
            //                 Item.Create(item).Spawn(ply.Position);
            //
            //         response = $"{amount} of {item.ToString()} was spawned on everyone (\"Hehexd\" - Galaxy119)";
            //         return true;
            //     default:
            //         if (arguments.Count < 3)
            //         {
            //             response = "Usage: dropitem (player id / name) (ItemType) (amount (200 max))";
            //             return false;
            //         }
            //
            //         Player pl = int.TryParse(arguments.At(0), out int id) ? Player.GetPlayers().FirstOrDefault(x => x.PlayerId == id) : Player.GetByName(arguments.At(0));
            //         if (pl == null)
            //         {
            //             response = $"Player not found: {arguments.At(0)}";
            //             return false;
            //         }
            //
            //         if (!Enum.TryParse(arguments.At(1), true, out ItemType it))
            //         {
            //             response = $"Invalid value for item type: {arguments.At(1)}";
            //             return false;
            //         }
            //
            //         if (!uint.TryParse(arguments.At(2), out uint am) || am > 200)
            //         {
            //             response = $"Invalid amount of item to drop: {arguments.At(2)}";
            //             return false;
            //         }
            //
            //         for (int i = 0; i < am; i++)
            //             Item.Create(it).Spawn(pl.Position);
            //         response = $"{am} of {it.ToString()} was spawned on {pl.Nickname} (\"Hehexd\" - Galaxy119)";
            //         return true;
            // }
        }
    }
}
