using CommandSystem;
using NorthwoodLib.Pools;
using System;
using System.Text;

namespace AdminTools.Commands.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Enums : ParentCommand
    {
        public Enums() => LoadGeneratedCommands();

        public override string Command => "enums";

        public override string[] Aliases { get; } =
        {
            "enum"
        };

        public override string Description => "Lists all enums AdminTools uses";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder listBuilder = StringBuilderPool.Shared.Rent();
            listBuilder.Append("Here are the following enums you can use in commands:");
            listBuilder.AppendLine();
            listBuilder.Append("VectorAxis: ");
            foreach (VectorAxis va in Enum.GetValues(typeof(VectorAxis)))
            {
                listBuilder.Append(va.ToString());
                listBuilder.Append(" ");
            }
            listBuilder.AppendLine();
            listBuilder.Append("PositionModifier: ");
            foreach (PositionModifier pm in Enum.GetValues(typeof(PositionModifier)))
            {
                listBuilder.Append(pm.ToString());
                listBuilder.Append(" ");
            }
            string message = listBuilder.ToString();
            StringBuilderPool.Shared.Return(listBuilder);
            response = message;
            return true;
        }
    }
}
