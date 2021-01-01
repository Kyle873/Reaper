using System.Reflection;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.NetLauncher;

using DPII;

using HarmonyLib;

#pragma warning disable 649

namespace Reaper
{
    [BepInPlugin("tech.kyl873.reaper", "Reaper", "1.0.0.0")]
    internal class Reaper : BasePlugin
    {
        public static ConfigEntry<bool> AutoLootActive;

        public override void Load()
        {
            AutoLootActive = Config.Bind("AutoLoot", "Active", false);

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            EnableDebugMenu();
        }

        static void EnableDebugMenu()
        {
            Debug.menu       = true;
            Global.skipIntro = true;
        }
    }
}
