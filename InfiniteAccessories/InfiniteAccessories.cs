using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using BepInEx.Logging;

namespace InfiniteAccessories
{
    [BepInPlugin(GUID, "Infinite Accessories", Version)]
    public class InfiniteAccessories : BaseUnityPlugin
    {
        public const string GUID = "infiniteaccessories";
        public const string Version = "1.0.0";

        internal new static ManualLogSource Logger;
        internal static Harmony Harmony;

        private void Awake()
        {
            Logger = base.Logger;
            Harmony = new Harmony($"{GUID}.harmony");
            RemoveRangeChecks.Patch();
        }
    }
}
