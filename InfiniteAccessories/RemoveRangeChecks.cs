using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using BepInEx.Harmony;
using HarmonyLib;

namespace InfiniteAccessories
{
    internal static class RemoveRangeChecks
    {
        public static void Patch()
        {
            var methodInfo_RemoveRangeCheck = AccessTools.Method(typeof(RemoveRangeChecks), nameof(RemoveRangeCheck));
            foreach(var method in methods)
                InfiniteAccessories.Harmony.Patch(method, null, null, new HarmonyMethod(methodInfo_RemoveRangeCheck));

            var methodInfo_FindIterator = AccessTools.Method(typeof(RemoveRangeChecks), nameof(FindIterator));
            var methodInfo_ChangeAccessoryAsync = AccessTools.Method(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryAsync), new[] { typeof(int), typeof(int), typeof(int), typeof(string), typeof(bool), typeof(bool) });
            InfiniteAccessories.Harmony.Patch(methodInfo_ChangeAccessoryAsync, null, null, new HarmonyMethod(methodInfo_FindIterator));
        }

        private static MethodInfo[] methods = new[]
        {
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryColor)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryParent)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.GetAccessoryDefaultColor)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.IsAccessory)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.SetAccessoryDefaultColor)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.SetAccessoryPos)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.SetAccessoryRot)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.SetAccessoryScl)),
            AccessTools.Method(typeof(ChaControl), nameof(ChaControl.UpdateAccessoryMoveFromInfo)),
        };

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
        
        private static IEnumerable<CodeInstruction> FindIterator(IEnumerable<CodeInstruction> instructions)
        {
            foreach(var code in instructions)
            {
                if(code.opcode == OpCodes.Newobj)
                {
                    var patch = AccessTools.Method(typeof(RemoveRangeChecks), nameof(IteratorPatch));
                    var targetType = Traverse.Create(code.operand).Property("DeclaringType").GetValue<Type>();
                    var target = AccessTools.Method(targetType, "MoveNext");
                    InfiniteAccessories.Harmony.Patch(target, null, null, new HarmonyMethod(patch));
                }
                else
                {
                    InfiniteAccessories.Logger.LogError($"Could not find iterator from method {nameof(ChaControl.ChangeAccessoryAsync)}");
                }

                break;
            }

            return instructions;
        }

        private static IEnumerable<CodeInstruction> IteratorPatch(IEnumerable<CodeInstruction> instructions)
        {
            bool intFound = false;
            bool patched = false;

            foreach(var code in instructions)
            {
                if(code.opcode == OpCodes.Ldc_I4_S && (sbyte)code.operand == 19)
                {
                    intFound = true;
                    InfiniteAccessories.Logger.LogInfo("Marker found in iterator");
                }

                if(intFound)
                {
                    if(code.opcode == OpCodes.Br)
                    {
                        code.opcode = OpCodes.Nop;
                        InfiniteAccessories.Logger.LogInfo($"Iterator patched in {nameof(ChaControl.ChangeAccessoryAsync)}");
                        patched = true;
                        break;
                    }
                }
            }

            if(!patched)
                InfiniteAccessories.Logger.LogInfo("Could not patch iterator");

            return instructions;
        }
    }
}
