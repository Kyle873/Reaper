using System.Collections.Generic;
using System.Linq;

using DataStats;

using DPII;
using DPII.InGame;

using HarmonyLib;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Reaper.Utils;

namespace Reaper.Patches
{
    [HarmonyPatch(typeof(Game1), "Update", typeof(GameTime))]
    internal class ShortcutKeys
    {
        const Keys ConsoleKey     = Keys.OemTilde;
        const Keys SaveMenuKey    = Keys.F5;
        const Keys ToggleAutoLoot = Keys.B;
        const Keys ShopKey        = Keys.N;
        const Keys BuyAmmoKey     = Keys.K;

        static readonly Dictionary<Keys, storeType> StoreModKeys = new Dictionary<Keys, storeType>
        {
            { Keys.NumPad1, storeType.docNorth },
            { Keys.NumPad2, storeType.smith },
            { Keys.NumPad3, storeType.sal },
            { Keys.NumPad4, storeType.locke },
            { Keys.NumPad5, storeType.wong }
        };

        static readonly Dictionary<string, int> AmmoValues = new Dictionary<string, int>
        {
            { "762mm", 10 },
            { "shells", 10 },
            { "9mm", 10 },
            { "gas", 10 },
            { "arrow", 10 },
            { "launcherGrenade", 10 },
            { "powerCells", 20 },
            { "magnumRounds", 100 },
            { "rocket", 1000 },
            { "unholyRounds", 10000 }
        };

        static void Postfix(GameTime gameTime)
        {
            if (!Get.InGameplay)
                return;

            if (Input.KeyPressed(ConsoleKey))
            {
                Game1.CurrentState.finished  = true;
                Game1.CurrentState.nextState = new ConsoleMenu(Game1.CurrentState);
            }

            if (Input.KeyPressed(SaveMenuKey))
            {
                Game1.CurrentState.finished  = true;
                Game1.CurrentState.nextState = new SaveMenu((Gameplay)Game1.CurrentState);
            }

            if (Input.KeyPressed(ShopKey))
                OpenShop();

            if (Input.KeyPressed(BuyAmmoKey))
                BuyAmmo();

            if (Input.KeyPressed(ToggleAutoLoot))
            {
                KMod.AutoLootActive.Value = !KMod.AutoLootActive.Value;

                Audio.playSound(KMod.AutoLootActive.Value ? "beep" : "swoosh");
            }
        }

        static void OpenShop()
        {
            const int Max = 999;

            Vector2   pos  = new Vector2(-1000, -1000);
            storeType type = storeType.none;

            foreach (KeyValuePair<Keys, storeType> keyMod in StoreModKeys.Where(kvp => Input.KeyDown(kvp.Key)))
            {
                type = keyMod.Value;

                break;
            }

            TraderNPC npc = new TraderNPC(type, true, pos);

            foreach (KeyValuePair<string, ItemStats> item in Stats.GetItemStats())
                npc.inventory.removeItem(item.Key, Max);

            foreach (KeyValuePair<string, ItemStats> item in Stats.GetItemStats())
                npc.inventory.addItem(item.Key, Max);

            foreach (KeyValuePair<string, ItemStats> item in Stats.GetItemStats())
            {
                int amount = npc.inventory.getItemAmount(item.Key);

                if (amount > Max)
                    npc.inventory.removeItem(item.Key, amount - Max);
            }

            Get.Character.store = npc;

            StoreInGameMenu menu = new StoreInGameMenu(Get.Character);

            Get.Character.inGameMenu = menu;

            Game1.CurrentState.addCursorHotspots();
        }

        static void BuyAmmo()
        {
            string primaryWeapon     = Get.Character.equipedPrimaryWeapon;
            string secondaryWeapon   = Get.Character.equipedSecondaryWeapon;
            string primaryAmmoType   = Stats.GetWeaponStats(primaryWeapon).ammoType;
            string secondaryAmmoType = Stats.GetWeaponStats(secondaryWeapon).ammoType;
            int    primaryAmmoMax    = !string.IsNullOrEmpty(primaryAmmoType) ? Get.Character.inventory.getLimit(primaryAmmoType) : 0;
            int    secondaryAmmoMax  = !string.IsNullOrEmpty(secondaryAmmoType) ? Get.Character.inventory.getLimit(secondaryAmmoType) : 0;

            if (!string.IsNullOrEmpty(primaryAmmoType))
                while (Get.Character.getPrimaryAmmo < primaryAmmoMax && Get.Character.inventory.money >= AmmoValues[primaryAmmoType])
                {
                    Get.Character.inventory.addItem(primaryAmmoType, 1);
                    Get.Character.inventory.adjustMoney(-AmmoValues[primaryAmmoType]);
                }

            if (!string.IsNullOrEmpty(secondaryAmmoType))
                while (Get.Character.getSecondaryAmmo < secondaryAmmoMax && Get.Character.inventory.money >= AmmoValues[secondaryAmmoType])
                {
                    Get.Character.inventory.addItem(secondaryAmmoType, 1);
                    Get.Character.inventory.adjustMoney(-AmmoValues[secondaryAmmoType]);
                }
        }
    }
}
