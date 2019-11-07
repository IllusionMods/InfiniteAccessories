using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;

namespace InfiniteAccessories
{
    [BepInPlugin(GUID, "Infinite Accessories", Version)]
    public class InfiniteAccessories
    {
        public const string GUID = "infiniteaccessories";
        public const string Version = "1.0.0";

        private void Awake()
        {
            var harmony = new Harmony($"{GUID}.harmony");
            RemoveRangeChecks.Patch(harmony);
        }
    }
}
