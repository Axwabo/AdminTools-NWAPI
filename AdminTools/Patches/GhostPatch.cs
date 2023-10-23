using AdminTools.Commands.Ghost;
using HarmonyLib;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using PlayerRoles.Visibility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace AdminTools.Patches
{
    [HarmonyPatch(typeof(FpcServerPositionDistributor), nameof(FpcServerPositionDistributor.WriteAll))]
    public static class GhostPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new(instructions);
            int index = list.FindIndex(i => i.operand is MethodInfo { Name: nameof(VisibilityController.ValidateVisibility) }) + 6;
            list.InsertRange(index, new[]
            {
                new(OpCodes.Ldarg_0), new(OpCodes.Ldloc, 5), new(OpCodes.Ldloc, 7), CodeInstruction.Call(typeof(GhostPatch), nameof(OverrideVisibility)), new(OpCodes.Stloc, 7)
            });
            return list;
        }
        public static bool OverrideVisibility(ReferenceHub receiver, ReferenceHub target, bool isInvisible)
            => isInvisible
                || target.TryGetComponent(out GhostController controller)
                && (controller.IsFullyInvisible || controller.InvisibleTo.Contains(receiver.authManager.UserId));
    }
}
