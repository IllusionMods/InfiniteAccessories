using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;

namespace InfiniteAccessories
{
    internal static class RemoveRangeChecks
    {
        private static MethodInfo[] methods = new[]
        {
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryColor)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryParent)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.GetAccessoryDefaultColor)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.GetAccessoryDefaultParentStr)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.IsAccessory)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.SetAccessoryDefaultColor)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.SetAccessoryPos)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.SetAccessoryRot)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.SetAccessoryScl)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.UpdateAccessoryMoveFromInfo)),
        };

        public static void Patch(Harmony harmony)
        {
            foreach(var method in methods)
                harmony.Patch(method, null, null, new HarmonyMethod(MethodInfo_RemoveRangeCheck));
        }

        private static MethodInfo MethodInfo_RemoveRangeCheck = AccessTools.Method(typeof(RemoveRangeChecks), nameof(RemoveRangeCheck));
        private static IEnumerable<CodeInstruction> RemoveRangeCheck(IEnumerable<CodeInstruction> instructions)
        {
            foreach(var code in instructions)
            {
                bool breakNow = code.opcode == OpCodes.Ret;
                code.opcode = OpCodes.Nop;
                code.operand = null;
                if(breakNow) break;
            }

            return instructions;
        }
    }
}
